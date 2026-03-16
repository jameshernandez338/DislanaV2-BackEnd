using Dislana.Domain.Auth.Entities;
using Dislana.Domain.Auth.Interfaces;
using Dislana.Infrastructure.Persistence.Dapper;

namespace Dislana.Infrastructure.Persistence.Repositories.Auth
{
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

            return await _dbExecutor.QuerySingleOrDefaultAsync<UserCredentialEntity>(
                sql,
                new { UserId = userId },
                commandType: null,
                cancellationToken: cancellationToken);
        }
    }
}

