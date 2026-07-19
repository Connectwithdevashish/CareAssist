using CareAssist.Application.Abstractions.Authentication;
using CareAssist.Contracts.Auth;
using CareAssist.Domain.Identity;
using CareAssist.Infrastructure.Configuration.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CareAssist.Infrastructure.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings options;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(IOptions<JwtSettings> options, 
        ILogger<JwtTokenService> logger)
    {
        this.options = options.Value;
        _logger = logger;
    }

    public TokenResult GenerateToken(ApplicationUser user)
    {
        var ExpiresAt = DateTime.UtcNow.AddHours(options.ExpiryHours);

        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
            options.SecretKey));

        var credentials = new SigningCredentials(
            key, 
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: ExpiresAt,
            signingCredentials: credentials
            );

        _logger.LogInformation("Generated JWT token for user {UserId} with expiration at {ExpiresAt}",
            user.Id, ExpiresAt);

        return new TokenResult(
            AccessToken: new JwtSecurityTokenHandler()
                            .WriteToken(token),
            ExpiresAt: ExpiresAt
        );
    }
}
