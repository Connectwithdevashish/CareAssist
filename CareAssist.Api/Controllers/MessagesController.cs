using CareAssist.Api.Contracts.AI;
using CareAssist.Api.Contracts.Messages;
using CareAssist.Api.Data;
using CareAssist.Api.Entities.Chat;
using CareAssist.Api.Entities.Enum;
using CareAssist.Api.Extensions;
using CareAssist.Api.Services.AI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareAssist.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/conversations/{conversationId:guid}/messages")]
public sealed class MessagesController : ControllerBase
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IChatCompletionService _chatCompletionService;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(ApplicationDbContext dbContext,
        IChatCompletionService chatCompletionService, 
        ILogger<MessagesController> logger)
    {
        _dbContext = dbContext;
        _chatCompletionService = chatCompletionService;
        _logger = logger;
    }

    private async Task<Conversation?> GetConversationAsync(Guid conversationId,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        _logger.LogInformation("Retrieving conversation with ID {ConversationId} for user {UserId}", conversationId, userId);

        return await _dbContext.Conversations
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == userId &&
            c.Id == conversationId,
            cancellationToken);
    }

    // Post Messages in a conversation

    // Posting in a specific conversation of specific user only
    [HttpPost]
    public async Task<ActionResult<MessageResponse>> CreateMessage(Guid conversationId,
        CreateMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        var conversation = await GetConversationAsync(conversationId, cancellationToken);

        if(conversation == null)
        {
            return NotFound();
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

        return Ok(messageResponse);
    }

    // Get all messages for a conversation

    // retrive all messages but in order like whatsApp does,
    // so the latest message will be at the bottom and
    // the first message will be at the top
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageResponse>>> GetAllMessages(Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        var conversation = await GetConversationAsync(conversationId, cancellationToken);

        if (conversation == null)
        {
            return NotFound();
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

        _logger.LogInformation("Retrieved {MessageCount} messages for conversation {ConversationId}", 
            messages.Count, conversation.Id);

        return Ok(messages);
    }
}
