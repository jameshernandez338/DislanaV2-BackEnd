using Dislana.Domain.Auth.Entities;
using Dislana.Domain.Auth.Interfaces;
using Dislana.Infrastructure.Persistence.Dapper;

namespace Dislana.Infrastructure.Persistence.Repositories.Auth
{
    /// <summary>
    /// Repositorio de Infraestructura: Implementa persistencia de credenciales de usuario
    /// Reconstruye entidades ricas desde la BD
    /// </summary>
    public class UserCredentialRepository : IUserCredentialRepository
    {
        private readonly IDbExecutor _dbExecutor;

        public UserCredentialRepository(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<UserCredentialEntity?> GetCredentialByUserIdAsync(long userId, CancellationToken cancellationToken)
        {
            const string sql = """
                SELECT 
                    u.UserId,
                    u.PasswordHash,
                    u.PasswordChangedAt
                FROM UserCredentials u
                WHERE u.UserId = @UserId
            """;

            var dbCredential = await _dbExecutor.QuerySingleOrDefaultAsync<UserCredentialDto>(
                sql,
                new { UserId = userId },
                commandType: null,
                cancellationToken: cancellationToken);

            if (dbCredential == null)
                return null;

            // Reconstruir entidad rica desde los datos de la BD
            return UserCredentialEntity.Reconstitute(
                userId: dbCredential.UserId,
                passwordHash: dbCredential.PasswordHash,
                passwordChangedAt: dbCredential.PasswordChangedAt
            );
        }

        /// <summary>
        /// DTO para mapear datos desde la BD
        /// </summary>
        private class UserCredentialDto
        {
            public long UserId { get; set; }
            public string PasswordHash { get; set; } = default!;
            public DateTime PasswordChangedAt { get; set; }
        }
    }
}

