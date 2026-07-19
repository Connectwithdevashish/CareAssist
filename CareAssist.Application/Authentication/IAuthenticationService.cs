using CareAssist.Contracts.Auth;
using CareAssist.Domain.Identity;

namespace CareAssist.Application.Authentication;

public interface IAuthenticationService
{
    Task RegisterUserAsync(RegisterRequest request);
    Task<AuthResponse> LoginUserAsync(LoginRequest request);
}
