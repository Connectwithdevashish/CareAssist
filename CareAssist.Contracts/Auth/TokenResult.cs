namespace CareAssist.Contracts.Auth;

public record TokenResult(string AccessToken, DateTime ExpiresAt);
