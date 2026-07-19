namespace CareAssist.Contracts.Auth;

public record AuthResponse(string AccessToken, DateTime ExpiresAt);
