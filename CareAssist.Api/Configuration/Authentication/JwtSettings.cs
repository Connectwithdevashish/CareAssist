namespace CareAssist.Api.Configuration.Authentication;

public sealed class JwtSettings
{
    public string Issuer { get; set; } = default!;

    public string Audience { get; set; } = default!;

    public string SecretKey { get; set; } = default!;
    public int ExpiryHours { get; set; } = 1;
}
