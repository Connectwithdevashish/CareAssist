using CareAssist.Application.Abstractions;
using CareAssist.Application.Abstractions.Persistence;
using CareAssist.Contracts.Conversations;
using CareAssist.Contracts.Messages;
using CareAssist.Domain.Chat;
using CareAssist.Domain.Exceptions.Conversations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareAssist.Application.Conversation;

public class ConversationService : IConversationService
{
    private readonly IApplicationContextService _applicationContextService;
    private readonly ICurrentUserService _httpContextServiceFile;
    private readonly ILogger<ConversationService> _logger;

    public ConversationService(IApplicationContextService applicationContextService,
        ILogger<ConversationService> logger,
        ICurrentUserService httpContextServiceFile)
    {
        _applicationContextService = applicationContextService;
        _logger = logger;
        _httpContextServiceFile = httpContextServiceFile;
    }

    public async Task DeleteConversationByIdAsync(Guid id,
        CancellationToken cancellationToken)
    {
        var userId = _httpContextServiceFile.UserId;

        if (userId == null)
        {
            _logger.LogWarning("User ID is null. Cannot delete conversation with ID {ConversationId}", id);
            throw new ConversationAccessDeniedException($"Access denied. User ID is null. " +
                $"Cannot delete conversation with ID {id}");
        }

        var conversation = await _applicationContextService.Conversations
            .Where(x => x.UserId == userId && x.Id == id)
            .FirstOrDefaultAsync();

        if (conversation == null)
        {
            _logger.LogWarning("Conversation with ID {ConversationId} not found for user {UserId}", id, userId);
            throw new ConversationNotFoundException($"Conversation with ID {id} not found");
        }

        _applicationContextService.Conversations.Remove(conversation);
        await _applicationContextService.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Deleted conversation with ID {ConversationId} for user {UserId}",
            conversation.Id, userId);
    }

    public async Task<IEnumerable<ConversationResponse>> GetAllConversationsAsync()
    {
        var userId = _httpContextServiceFile.UserId;

        if (userId == null)
        {
            _logger.LogWarning("User ID is null. Cannot retrieve conversations");
            throw new ConversationAccessDeniedException("Access denied. User ID is null. Cannot retrieve conversations");
        }

        List<ConversationResponse> conversations = await _applicationContextService.Conversations
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new ConversationResponse(
                x.Id,
                x.Title,
                x.CreatedAtUtc
            )).ToListAsync();
        if (!conversations.Any())
        {
            _logger.LogWarning("No conversations found for user {UserId}", userId);
            throw new ConversationNotFoundException($"No conversations found for user {userId}");
        }

        _logger.LogInformation("Retrieved {Count} conversations for user {UserId}", conversations.Count, userId);

        return conversations;
    }

    public async Task<ConversationResponse> GetConversationByIdAsync(Guid id)
    {
        var userId = _httpContextServiceFile.UserId;

        if (userId == null)
        {
            _logger.LogWarning("User ID is null. Cannot retrieve conversations");
            throw new ConversationAccessDeniedException("Access denied. User ID is null. Cannot retrieve conversations");
        }

        ConversationResponse? conversationResponse = await _applicationContextService.Conversations
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.Id == id)
            .Select(x => new ConversationResponse(
                x.Id,
                x.Title,
                x.CreatedAtUtc))
            .FirstOrDefaultAsync();

        if (conversationResponse == null)
        {
            _logger.LogWarning("Conversation with ID {ConversationId} not found for user {UserId}", id, userId);
            throw new ConversationNotFoundException($"Conversation with ID {id} not found");
        }

        _logger.LogInformation("Retrieved conversation with ID {ConversationId} for user {UserId}",
            conversationResponse.Id, userId);

        return conversationResponse;
    }

    public async Task<ConversationDetailsResponse> GetDetailAsync(Guid id, CancellationToken cancellationToken)
    {
        var userId = _httpContextServiceFile.UserId;

        if (userId == null)
        {
            _logger.LogWarning("User ID is null. Cannot retrieve conversations");
            throw new ConversationAccessDeniedException("Access denied. User ID is null. Cannot retrieve conversations");
        }

        var converstion = await _applicationContextService.Conversations
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.Id == id)
            .Select(x => new ConversationDetailsResponse
            (
                x.Id,
                x.Title,
                x.CreatedAtUtc,
                x.Messages
                    .OrderBy(m => m.CreatedAtUtc)
                    .Select(m => new MessageResponse
                    (
                        m.Id,
                        m.Content,
                        m.Role.ToString().ToLowerInvariant(),
                        m.CreatedAtUtc
                    )).ToList())
            ).FirstOrDefaultAsync(cancellationToken);

        if (converstion == null) {
            _logger.LogWarning("Conversation details with ID {ConversationId} not found for user {UserId}", id, userId);
            throw new ConversationNotFoundException($"Conversation details with ID {id} not found");
        }

        _logger.LogInformation("Retrieved conversation details with ID {ConversationId} for user {UserId}",
            converstion.Id, userId);

        return converstion;
    }

    public async Task<ConversationResponse> PostConversationAsync(CreateConversationRequest request, 
        CancellationToken cancellationToken)
    {
        var userId = _httpContextServiceFile.UserId;

        if (userId == null)
        {
            _logger.LogWarning("User ID is null. Cannot retrieve conversations");
            throw new ConversationAccessDeniedException("Access denied. User ID is null. Cannot retrieve conversations");
        }

        Conversations conversation = new Conversations()
        {
            CreatedAtUtc = DateTime.UtcNow,
            Title = string.IsNullOrEmpty(request.Title) ? "New Chat" : request.Title,
            Id = Guid.NewGuid(),
            UserId = userId
        };

        _applicationContextService.Conversations.Add(conversation);
        await _applicationContextService.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Created new conversation with ID {ConversationId} for user {UserId}",
            conversation.Id, userId);

        ConversationResponse response = new ConversationResponse(
            conversation.Id,
            conversation.Title,
            conversation.CreatedAtUtc
        );

        return response;
    }
}
