using CareAssist.Api.Configuration.Authentication;
using CareAssist.Api.Contracts.Auth;
using CareAssist.Api.Entities.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CareAssist.Api.Services.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings options;
    public JwtTokenService(IOptions<JwtSettings> options)
    {
        this.options = options.Value;
    }

    public async Task<TokenResult> GenerateToken(ApplicationUser user)
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

        return new TokenResult(
            AccessToken: new JwtSecurityTokenHandler()
                            .WriteToken(token),
            ExpiresAt: ExpiresAt
        );
    }
}
