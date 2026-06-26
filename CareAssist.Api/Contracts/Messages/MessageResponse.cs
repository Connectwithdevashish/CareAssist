namespace CareAssist.Api.Contracts.Messages;

public record MessageResponse(Guid Id,
    string Message, 
    string UserRole,
    DateTime CreatedAtUtc)
{
}
