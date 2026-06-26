namespace CareAssist.Api.Contracts.Conversations;

public sealed record ConversationResponse(Guid Id, 
    string Title, 
    DateTime CreatedAtUtc)
{
}
