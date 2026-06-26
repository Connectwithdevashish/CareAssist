using CareAssist.Api.Contracts.Messages;
using CareAssist.Api.Data;
using CareAssist.Api.Entities.Chat;
using CareAssist.Api.Entities.Enum;
using CareAssist.Api.Extensions;
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

    public MessagesController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private async Task<Conversation?> GetConversationAsync(Guid conversationId)
    {
        var userId = User.GetUserId();

        return await _dbContext.Conversations
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == userId &&
            c.Id == conversationId);
    }

    // Post Messages in a conversation

    // Posting in a specific conversation of specific user only
    [HttpPost]
    public async Task<ActionResult<MessageResponse>> CreateMessage(Guid conversationId,
        CreateMessageRequest request)
    {
        if(string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest("Message content cannot be empty.");
        }

        Conversation? conversation = await GetConversationAsync(conversationId);

        if(conversation == null)
        {
            return NotFound();
        }

        var message = new Message
        {
            Content = request.Content.Trim(),
            Role = MessageRole.User,
            CreatedAtUtc = DateTime.UtcNow,
            ConversationId = conversation.Id
        };

        _dbContext.Messages.Add(message);

        await _dbContext.SaveChangesAsync();

        var messageResponse = new MessageResponse(
            message.Id,
            message.Content,
            MessageRole.User.ToString().ToLowerInvariant(),
            message.CreatedAtUtc
        );


        return Ok(messageResponse);
    }

    // Get all messages for a conversation

    // retrive all messages but in order like whatsApp does,
    // so the latest message will be at the bottom and
    // the first message will be at the top
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageResponse>>> GetAllMessages(Guid conversationId)
    {
        Conversation? conversation = await GetConversationAsync(conversationId);

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
                x.Role.ToString().ToLower(),
                x.CreatedAtUtc
            ))
            .ToListAsync();

        return Ok(messages);
    }
}
