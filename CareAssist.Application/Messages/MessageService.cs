using CareAssist.Application.Abstractions;
using CareAssist.Application.Abstractions.AI;
using CareAssist.Application.Abstractions.Persistence;
using CareAssist.Contracts.AI;
using CareAssist.Contracts.Messages;
using CareAssist.Domain.Chat;
using CareAssist.Domain.Enum;
using CareAssist.Domain.Exceptions.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareAssist.Application.Messages;

internal class MessageService : IMessageService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly ILogger<MessageService> _logger;
    private readonly IApplicationContextService _dbContext;

    public MessageService(ICurrentUserService currentUserService,
        IChatCompletionService chatCompletionService,
        ILogger<MessageService> logger,
        IApplicationContextService dbContext)
    {
        _currentUserService = currentUserService;
        _chatCompletionService = chatCompletionService;
        _logger = logger;
        _dbContext = dbContext;
    }
    public async Task<MessageResponse> CreateMessageAsync(Guid conversationId, 
        CreateMessageRequest request, 
        CancellationToken cancellationToken = default)
    {
        var conversation = await GetConversationAsync(conversationId, cancellationToken);

        if (conversation == null)
        {
            _logger.LogWarning("Conversation with ID {ConversationId} not found for user {UserId}", 
                conversationId, _currentUserService.UserId);
            throw new MessageNotFoundException("Conversation not found");
        }

        var userMessage = new Message
        {
            Content = request.Content.Trim(),
            Role = MessageRole.User,
            CreatedAtUtc = DateTime.UtcNow,
            ConversationId = conversation.Id
        };

        _dbContext.Messages.Add(userMessage);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User message created with ID {MessageId} in conversation {ConversationId}",
            userMessage.Id, conversation.Id);

        var history = await _dbContext.Messages
            .AsNoTracking()
            .Where(x => x.ConversationId == conversationId)
            .OrderBy(x => x.CreatedAtUtc)
            .Select(x => new ChatMessage(
                x.Role.ToString().ToLowerInvariant(),
                x.Content
            ))
            .ToListAsync();

        var chatResponse = await _chatCompletionService.GenerateResponseAsync(history, cancellationToken);

        var assistantMessage = new Message
        {
            Content = chatResponse.Content,
            Role = MessageRole.Assistant,
            CreatedAtUtc = DateTime.UtcNow,
            ConversationId = conversationId
        };

        _dbContext.Messages.Add(assistantMessage);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Assistant message created with ID {MessageId} in conversation {ConversationId}",
            assistantMessage.Id, conversation.Id);

        var messageResponse = new MessageResponse(
            assistantMessage.Id,
            assistantMessage.Content,
            assistantMessage.Role.ToString().ToLowerInvariant(),
            assistantMessage.CreatedAtUtc
        );

        return messageResponse;
    }

    public async Task<IEnumerable<MessageResponse>> GetAllMessagesAsync(Guid conversationId, 
        CancellationToken cancellationToken = default)
    {
        var conversation = await GetConversationAsync(conversationId, cancellationToken);

        if (conversation == null)
        {
            _logger.LogWarning("Conversation with ID {ConversationId} not found for user {UserId}",
                conversationId, _currentUserService.UserId);

            throw new MessageNotFoundException("Conversation not found");
        }

        var messages = await _dbContext.Messages
            .AsNoTracking()
            .Where(x => x.ConversationId == conversation.Id)
            .OrderBy(x => x.CreatedAtUtc)
            .Select(x => new MessageResponse(
                x.Id,
                x.Content,
                x.Role.ToString().ToLowerInvariant(),
                x.CreatedAtUtc
            ))
            .ToListAsync(cancellationToken);

        if (!messages.Any()) {
            _logger.LogWarning("No messages found for conversation {ConversationId}", conversation.Id);
            throw new MessageNotFoundException("No messages found for the specified conversation");
        }

        _logger.LogInformation("Retrieved {MessageCount} messages for conversation {ConversationId}",
            messages.Count, conversation.Id);

        return messages;
    }

    private async Task<Conversations?> GetConversationAsync(Guid conversationId, 
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        _logger.LogInformation("Retrieving conversation with ID {ConversationId} for user {UserId}", conversationId, userId);

        return await _dbContext.Conversations
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == userId &&
            c.Id == conversationId,
            cancellationToken);
    }
}
