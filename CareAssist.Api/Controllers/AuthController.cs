using CareAssist.Api.Contracts.Auth;
using CareAssist.Api.Entities.Identity;
using CareAssist.Api.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CareAssist.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(UserManager<ApplicationUser> userManager, 
        IJwtTokenService jwtTokenService, 
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    // To do - Introduce repository pattern here

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            _logger.LogWarning("Attempt to register with an existing email: {Email}",
                request.Email);

            return BadRequest("User with this email already exists.");
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

            return BadRequest("Failed to create user.");
        }
        
        _logger.LogInformation("User created successfully.");

        return Ok(result);

    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> LoginUser(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null) {
            _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);

            return BadRequest("Invalid email or password.");
        }

        var passwordValidated = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordValidated)
        {
            _logger.LogWarning("Failed login attempt for email: {Email}", request.Email);

            return BadRequest("Invalid email or password.");
        }

        var token = await _jwtTokenService.GenerateToken(user);

        _logger.LogInformation("User logged in successfully.");

        return Ok(new AuthResponse(
            token.AccessToken,
            token.ExpiresAt
        ));
    }

    [Authorize]
    [HttpGet("test")]
    public IActionResult Test()
    {
        _logger.LogInformation("Authenticated access successful.");

        return Ok("Authenticated access successful!");
    }
}
