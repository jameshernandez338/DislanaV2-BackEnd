using Dislana.Domain.Auth.Entities;
using Dislana.Domain.Auth.Interfaces;
using Dislana.Infrastructure.Persistence.Dapper;
using System.Data;

namespace Dislana.Infrastructure.Persistence.Repositories.Auth
{
    /// <summary>
    /// Repositorio de Infraestructura: Implementa persistencia de refresh tokens
    /// Reconstruye entidades ricas desde la BD
    /// </summary>
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IDbExecutor _dbExecutor;

        public RefreshTokenRepository(IDbExecutor dbExecutor) => _dbExecutor = dbExecutor;

        public async Task<RefreshTokenEntity?> GetByTokenAsync(string token, CancellationToken cancellationToken)
        {
            const string spName = "usp_getRefreshToken";

            var dbToken = await _dbExecutor.QuerySingleOrDefaultAsync<RefreshTokenDto>(
                spName,
                new { token = token },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            if (dbToken == null) 
                return null;

            // Reconstruir entidad rica desde los datos de la BD
            return RefreshTokenEntity.Reconstitute(
                token: dbToken.Token,
                userId: dbToken.UserId,
                expiresAt: dbToken.ExpiresAt,
                createdAt: dbToken.CreatedAt,
                isRevoked: dbToken.IsRevoked
            );
        }

        public async Task SaveRefreshTokenAsync(
            RefreshTokenEntity refreshToken, 
            CancellationToken cancellationToken)
        {
            const string spName = "usp_saveRefreshToken";

            await _dbExecutor.ExecuteAsync(
                spName,
                new
                {
                    token = refreshToken.Token,
                    userId = refreshToken.UserId,
                    expiresAt = refreshToken.ExpiresAt
                },
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

        /// <summary>
        /// DTO para mapear datos desde la BD
        /// </summary>
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
