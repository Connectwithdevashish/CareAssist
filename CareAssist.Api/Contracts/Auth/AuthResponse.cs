namespace CareAssist.Api.Contracts.Auth;

public record AuthResponse(string AccessToken, DateTime ExpiresAt)
{
}
