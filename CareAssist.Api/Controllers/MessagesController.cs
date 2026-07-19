using CareAssist.Domain.Chat;
using CareAssist.Contracts.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareAssist.Application.Messages;

namespace CareAssist.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/conversations/{conversationId:guid}/messages")]
public sealed class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;
    private readonly ILogger<MessagesController> _logger;

    public MessagesController(IMessageService messageService,
        ILogger<MessagesController> logger)
    {
        _messageService = messageService;
        _logger = logger;
    }

    // Post Messages in a conversation

    // Posting in a specific conversation of specific user only
    [HttpPost]
    public async Task<ActionResult<MessageResponse>> CreateMessage(Guid conversationId,
        CreateMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _messageService.CreateMessageAsync(conversationId, request, cancellationToken);
        if (response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }

    // Get all messages for a conversation

    // retrive all messages but in order like whatsApp does,
    // so the latest message will be at the bottom and
    // the first message will be at the top
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageResponse>>> GetAllMessages(Guid conversationId,
        CancellationToken cancellationToken = default)
    {
        var response = await _messageService.GetAllMessagesAsync(conversationId, cancellationToken);
        if (response == null) {
            return NotFound();
        }
        return Ok(response);
    }
}
