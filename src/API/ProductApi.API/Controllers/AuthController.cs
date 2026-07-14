using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;

namespace ProductApi.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Authenticates a user and returns JWT tokens.
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<TokenResponse>> Login(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var tokenResponse = await _userService.LoginAsync(request, cancellationToken);
        return Ok(tokenResponse);
    }

    /// <summary>
    /// Registers a new user and returns JWT tokens.
    /// </summary>
    [HttpPost("register")]
    public async Task<ActionResult<TokenResponse>> Register(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var tokenResponse = await _userService.RegisterAsync(request, cancellationToken);
        return Ok(tokenResponse);
    }
}
