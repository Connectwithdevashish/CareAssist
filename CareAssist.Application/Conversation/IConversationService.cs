using CareAssist.Contracts.Conversations;
using Microsoft.AspNetCore.Mvc;

namespace CareAssist.Application.Conversation;

public interface IConversationService
{
    Task<ConversationResponse> PostConversationAsync(CreateConversationRequest request,
        CancellationToken cancellationToken);
    Task<IEnumerable<ConversationResponse>> GetAllConversationsAsync();
    Task<ConversationResponse> GetConversationByIdAsync(Guid id);
    Task DeleteConversationByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<ConversationDetailsResponse> GetDetailAsync(Guid id,
        CancellationToken cancellationToken);
}
