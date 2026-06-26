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

    public AuthController(UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    // To do - Introduce repository pattern here

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegisterRequest request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
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
            return BadRequest("Failed to create user.");
        }

        return Ok(result);

    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> LoginUser(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null) {
            return BadRequest("Invalid email or password.");
        }

        var passwordValidated = await _userManager.CheckPasswordAsync(user, request.Password);

        if (!passwordValidated)
        {
            return BadRequest("Invalid email or password.");
        }

        var token = await _jwtTokenService.GenerateToken(user);

        return Ok(new AuthResponse(
            token.AccessToken,
            token.ExpiresAt
        ));
    }

    [Authorize]
    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("Authenticated access successful!");
    }
}
