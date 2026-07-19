using CareAssist.Contracts.Messages;

namespace CareAssist.Contracts.Conversations;

public record ConversationDetailsResponse(Guid Id,
    string Title,
    DateTime CreatedAtUtc,
    IEnumerable<MessageResponse> Messages);
