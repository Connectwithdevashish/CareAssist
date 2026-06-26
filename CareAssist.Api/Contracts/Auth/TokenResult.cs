namespace CareAssist.Api.Contracts.Auth;

public record TokenResult(string AccessToken, DateTime ExpiresAt)
{
}
