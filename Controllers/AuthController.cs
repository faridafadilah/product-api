using Microsoft.AspNetCore.Mvc;
using AuthApi.Application.Interfaces;
using AuthApi.Models.DTOs;
using Microsoft.AspNetCore.RateLimiting;
using AuthApi.Models.Common;

namespace AuthApi.Controllers;

/// Handles authentication related operations such as register and login.
[EnableRateLimiting("AuthLimiter")]
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// Register a new user.
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<object>(400, "Invalid request data"));

        var result = await _authService.RegisterAsync(dto);

        if (!result.Success)
            return BadRequest(new ApiResponse<object>(400, result.Message));

        return Ok(new ApiResponse<object>(200, "Registration successful"));
    }

    /// Authenticate user and return JWT token.
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(new ApiResponse<object>(400, "Invalid request data"));

        var result = await _authService.LoginAsync(dto);

        if (!result.Success)
            return Unauthorized(new ApiResponse<object>(401, result.Message));

        return Ok(new ApiResponse<object>(200, "Login successful", new
        {
            authentication_token = result.Data.AuthToken,
            refresh_token = result.Data.RefreshToken
        }));
    }
}
