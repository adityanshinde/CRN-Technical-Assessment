namespace ProductApi.Infrastructure.Identity;

public interface IJwtTokenService
{
    string GenerateAccessToken(string userId, string[] roles);
    string GenerateRefreshToken();
}
