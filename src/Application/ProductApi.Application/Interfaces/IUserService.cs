using ProductApi.Application.DTOs;

namespace ProductApi.Application.Interfaces;

public interface IUserService
{
    Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<TokenResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}
