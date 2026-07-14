using Microsoft.Extensions.Configuration;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Exceptions;

namespace ProductApi.Application.Services;

public class UserService : IUserService
{
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;

    // In-memory user store for development
    private static readonly Dictionary<string, (string PasswordHash, string Email)> _users = new();

    public UserService(IJwtTokenService jwtTokenService, IConfiguration configuration)
    {
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var configUser = _configuration.GetSection("DefaultUser");

        // Check config-based default user first
        if (configUser.Exists())
        {
            var configUsername = configUser["Username"];
            var configPassword = configUser["Password"];

            if (request.Username == configUsername && request.Password == configPassword)
            {
                var accessToken = _jwtTokenService.GenerateAccessToken(request.Username, new[] { "user" });
                var refreshToken = _jwtTokenService.GenerateRefreshToken();

                return new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15)
                };
            }
        }

        // Check in-memory registered users
        if (_users.TryGetValue(request.Username, out var user))
        {
            if (user.PasswordHash == request.Password) // Plain text for dev simplicity
            {
                var accessToken = _jwtTokenService.GenerateAccessToken(request.Username, new[] { "user" });
                var refreshToken = _jwtTokenService.GenerateRefreshToken();

                return new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(15)
                };
            }
        }

        throw new NotFoundException("User", $"Invalid username or password.");
    }

    public async Task<TokenResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        if (_users.ContainsKey(request.Username))
        {
            throw new ConflictException($"User '{request.Username}' already exists.");
        }

        _users[request.Username] = (request.Password, request.Email);

        var accessToken = _jwtTokenService.GenerateAccessToken(request.Username, new[] { "user" });
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };
    }
}
