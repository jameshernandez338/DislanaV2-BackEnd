using Dislana.Application.Auth.DTOs;
using Dislana.Application.Auth.Results;

namespace Dislana.Application.Auth.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResult> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken);
        Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken);
        Task<LoginResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    }
}
