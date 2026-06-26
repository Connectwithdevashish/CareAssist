using CareAssist.Api.Contracts.Auth;
using CareAssist.Api.Entities.Identity;

namespace CareAssist.Api.Services.Authentication;

public interface IJwtTokenService
{
    Task<TokenResult> GenerateToken(ApplicationUser user);
}
