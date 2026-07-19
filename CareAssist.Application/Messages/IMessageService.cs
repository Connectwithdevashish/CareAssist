using CareAssist.Contracts.Messages;
using CareAssist.Domain.Chat;
using Microsoft.AspNetCore.Mvc;

namespace CareAssist.Application.Messages;

public interface IMessageService
{
    Task<MessageResponse> CreateMessageAsync(Guid conversationId,
        CreateMessageRequest request,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<MessageResponse>> GetAllMessagesAsync(Guid conversationId,
        CancellationToken cancellationToken = default);

}
