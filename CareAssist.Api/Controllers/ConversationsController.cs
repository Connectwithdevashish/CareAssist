using CareAssist.Contracts.Conversations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareAssist.Application.Conversation;

namespace CareAssist.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/conversations")]
public sealed class ConversationsController : ControllerBase
{
    private readonly IConversationService _conversationService;
    private readonly ILogger<ConversationsController> _logger;

    public ConversationsController(IConversationService conversationService, 
        ILogger<ConversationsController> logger)
    {
        _conversationService = conversationService;
        _logger = logger;
    }

    //POST   /api/conversations
    [HttpPost]
    public async Task<ActionResult<ConversationResponse>> PostConversation(CreateConversationRequest request,
        CancellationToken cancellationToken)
    {
        var Response = await _conversationService.PostConversationAsync(request, cancellationToken);

        if(Response == null)
        {
            return BadRequest(new { message = "Failed to create conversation." });
        }

        return CreatedAtAction(nameof(GetConversationById), new { id = Response.Id }, Response);
    }

    //GET    /api/conversations
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ConversationResponse>>> GetAllConversations()
    {
        var Response = await _conversationService.GetAllConversationsAsync();
        if(Response == null) {
            return BadRequest(new { message = "Failed to retrieve conversations." });
        }
        return Ok(Response);
    }

    //GET    /api/conversations/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ConversationResponse>> GetConversationById(Guid id)
    {
        var Response = await _conversationService.GetConversationByIdAsync(id);
        if(Response == null)
        {
            return BadRequest(new { message = "Failed to retrieve conversation." });
        }
        return Ok(Response);
    }

    //DELETE /api/conversations/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteConversationById(Guid id, 
        CancellationToken cancellationToken)
    {
        await _conversationService.DeleteConversationByIdAsync(id, cancellationToken);
        return NoContent();
    }

    // Get all conversations with their details

    [HttpGet("{id:guid}/details")]
    public async Task<ActionResult<ConversationDetailsResponse>> GetDetail(Guid id,
        CancellationToken cancellationToken)
    {
        var Response = await _conversationService.GetDetailAsync(id, cancellationToken);
        if(Response == null)
        {
            return BadRequest(new { message = "Failed to retrieve conversation details." });
        }
        return Ok(Response);
    }
}
