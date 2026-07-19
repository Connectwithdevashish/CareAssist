using CareAssist.Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CareAssist.Application.Authentication;

namespace CareAssist.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthenticationService authenticationService, 
        ILogger<AuthController> logger)
    {
        _authenticationService = authenticationService;
        _logger = logger;
    }

    // To do - Introduce repository pattern here

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser(RegisterRequest request)
    {
        await _authenticationService.RegisterUserAsync(request);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> LoginUser(LoginRequest request)
    {
        var authResponse = await _authenticationService.LoginUserAsync(request);

        if (authResponse == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }
        return Ok(authResponse);
    }

    [Authorize]
    [HttpGet("test")]
    public IActionResult Test()
    {
        _logger.LogInformation("Authenticated access successful.");

        return Ok("Authenticated access successful!");
    }
}
