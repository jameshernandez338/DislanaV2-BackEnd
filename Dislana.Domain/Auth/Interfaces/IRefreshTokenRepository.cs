using Dislana.Domain.Auth.Entities;

namespace Dislana.Domain.Auth.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken);
        Task SaveRefreshTokenAsync(string token, long userId, DateTime expiresAt, CancellationToken cancellationToken);
        Task RevokeTokenAsync(string token, CancellationToken cancellationToken);
        Task RevokeAllUserTokensAsync(long userId, CancellationToken cancellationToken);
    }
}
