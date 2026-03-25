using Dislana.Domain.Auth.Entities;
using Dislana.Domain.Auth.Interfaces;
using Dislana.Infrastructure.Persistence.Dapper;
using System.Data;

namespace Dislana.Infrastructure.Persistence.Repositories.Auth
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IDbExecutor _dbExecutor;

        public RefreshTokenRepository(IDbExecutor dbExecutor) => _dbExecutor = dbExecutor;

        public async Task<RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken)
        {
            const string spName = "usp_getRefreshToken";

            var result = await _dbExecutor.QuerySingleOrDefaultAsync<RefreshTokenDto>(
                spName,
                new { token = token },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            if (result == null) return null;

            return new RefreshTokenEntity(
                result.Token,
                result.UserId,
                result.ExpiresAt,
                result.CreatedAt,
                result.IsRevoked);
        }

        public async Task SaveRefreshTokenAsync(string token, long userId, DateTime expiresAt, CancellationToken cancellationToken)
        {
            const string spName = "usp_saveRefreshToken";

            await _dbExecutor.ExecuteAsync(
                spName,
                new { token = token, userId = userId, expiresAt = expiresAt },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);
        }

        public async Task RevokeTokenAsync(string token, CancellationToken cancellationToken)
        {
            const string spName = "usp_revokeRefreshToken";

            await _dbExecutor.ExecuteAsync(
                spName,
                new { token = token },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);
        }

        public async Task RevokeAllUserTokensAsync(long userId, CancellationToken cancellationToken)
        {
            const string spName = "usp_revokeAllUserRefreshTokens";

            await _dbExecutor.ExecuteAsync(
                spName,
                new { userId = userId },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);
        }

        private class RefreshTokenDto
        {
            public string Token { get; set; } = default!;
            public long UserId { get; set; }
            public DateTime ExpiresAt { get; set; }
            public DateTime CreatedAt { get; set; }
            public bool IsRevoked { get; set; }
        }
    }
}
