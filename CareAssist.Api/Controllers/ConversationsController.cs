using CareAssist.Api.Contracts.Conversations;
using CareAssist.Api.Contracts.Messages;
using CareAssist.Api.Data;
using CareAssist.Api.Entities.Chat;
using CareAssist.Api.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CareAssist.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/conversations")]
public sealed class ConversationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ConversationsController> _logger;

    public ConversationsController(ApplicationDbContext context, 
        ILogger<ConversationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    //POST   /api/conversations
    [HttpPost]
    public async Task<ActionResult<ConversationResponse>> PostConversation(CreateConversationRequest request)
    {
        var userId = User.GetUserId();

        Conversation conversation = new Conversation()
        {
            CreatedAtUtc = DateTime.UtcNow,
            Title = string.IsNullOrEmpty(request.Title) ? "New Chat" : request.Title,
            Id = Guid.NewGuid(),
            UserId = userId
        };

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created new conversation with ID {ConversationId} for user {UserId}",
            conversation.Id, userId);

        ConversationResponse response = new ConversationResponse(
            conversation.Id,
            conversation.Title,
            conversation.CreatedAtUtc
        );


        return CreatedAtAction(
            nameof(GetConversationById),
            new { id = conversation.Id },
            response
        );
    }

    //GET    /api/conversations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConversationResponse>>> GetAllConversations()
    {
        var userId = User.GetUserId();
        List<ConversationResponse> conversations = await _context.Conversations
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new ConversationResponse(
                x.Id,
                x.Title,
                x.CreatedAtUtc
            )).ToListAsync();

        _logger.LogInformation("Retrieved {Count} conversations for user {UserId}", conversations.Count, userId);

        return Ok(conversations);
    }

    //GET    /api/conversations/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ConversationResponse>> GetConversationById(Guid id)
    {
        var userId = User.GetUserId();

        ConversationResponse? conversationResponse = await _context.Conversations
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.Id == id)
            .Select(x => new ConversationResponse(
                x.Id,
                x.Title,
                x.CreatedAtUtc))
            .FirstOrDefaultAsync();

        if(conversationResponse == null)
        {
            return NotFound();
        }

        _logger.LogInformation("Retrieved conversation with ID {ConversationId} for user {UserId}",
            conversationResponse.Id, userId);

        return Ok(conversationResponse);
    }

    //DELETE /api/conversations/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteConversationById(Guid id)
    {
        var userId = User.GetUserId();

        var conversation = await _context.Conversations
            .Where(x => x.UserId == userId && x.Id == id)
            .FirstOrDefaultAsync();

        if(conversation == null) {
            return NotFound();
        }

        _context.Conversations.Remove(conversation);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted conversation with ID {ConversationId} for user {UserId}",
            conversation.Id, userId);

        return NoContent();
    }

    // Get all conversations with their details

    [HttpGet("{id:guid}/details")]
    public async Task<ActionResult<ConversationDetailsResponse>> GetDetail(Guid id,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();

        var converstion = await _context.Conversations
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

        if(converstion == null) { return NotFound(); }

        _logger.LogInformation("Retrieved conversation details with ID {ConversationId} for user {UserId}",
            converstion.Id, userId);

        return Ok(converstion);
    }
}
