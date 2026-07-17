using CareAssist.Api.Contracts.Messages;

namespace CareAssist.Api.Contracts.Conversations;

public record ConversationDetailsResponse(Guid Id,
    string Title,
    DateTime CreatedAtUtc,
    IEnumerable<MessageResponse> Messages)
{
}
