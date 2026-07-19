using CareAssist.Application.Abstractions.Authentication;
using CareAssist.Contracts.Auth;
using CareAssist.Domain.Exceptions.Authentication;
using CareAssist.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace CareAssist.Application.Authentication;

internal class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(UserManager<ApplicationUser> userManager,
        IJwtTokenService jwtTokenService,
        ILogger<AuthenticationService> logger)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }
    
    public async Task<AuthResponse> LoginUserAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);

            throw new UserNotFoundException("Invalid email or password");
        }

        var passwordValidated = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordValidated)
        {
            _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);

            throw new UnauthorizedUserAccessException("Invalid email or password");
        }

        var token = _jwtTokenService.GenerateToken(user);

        _logger.LogInformation("User logged in successfully.");

        return new AuthResponse(
            token.AccessToken,
            token.ExpiresAt
        );
    }

    public async Task RegisterUserAsync(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            _logger.LogWarning("Attempt to register with an existing email: {Email}",
                request.Email);

            throw new UserNotFoundException("A user with this email already exists.");
        }

        var result = await _userManager.CreateAsync(new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            CreatedAtUtc = DateTime.UtcNow,
            IsActive = true
        }, request.Password);

        if (!result.Succeeded)
        {
            _logger.LogError("User creation failed for email: {Email}. Errors: {Errors}",
                request.Email, string.Join(", ", result.Errors.Select(e => e.Description)));

            throw new UserCreationFailedException("Failed to create user.");
        }

        _logger.LogInformation("User created successfully.");
    }
}
