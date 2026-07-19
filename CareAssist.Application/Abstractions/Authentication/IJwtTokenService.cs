using CareAssist.Contracts.Auth;
using CareAssist.Domain.Identity;

namespace CareAssist.Application.Abstractions.Authentication;

public interface IJwtTokenService
{
    TokenResult GenerateToken(ApplicationUser user);
}
