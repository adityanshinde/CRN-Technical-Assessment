namespace ProductApi.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(string userId, string[] roles);
    string GenerateRefreshToken();
}
