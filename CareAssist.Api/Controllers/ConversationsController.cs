using CareAssist.Api.Contracts.Conversations;
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
    public ConversationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    //POST   /api/conversations
    [HttpPost]
    public async Task<ActionResult<ConversationResponse>> PostConversation(CreateConversationRequest request)
    {
        var userId = User.GetUserId();

        Conversation conversation = new Conversation()
        {
            CreatedAtUtc = DateTime.UtcNow,
            Title = request.Title,
            Id = Guid.NewGuid(),
            UserId = userId
        };

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

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
            .Select(x => new ConversationResponse(
                x.Id,
                x.Title,
                x.CreatedAtUtc
            )).ToListAsync();

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
        
        _context.Conversations.Remove(conversation!);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
