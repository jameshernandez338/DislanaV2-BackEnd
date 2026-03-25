using Dapper;
using Dislana.Domain.Auth.Entities;
using Dislana.Domain.Auth.Interfaces;
using Dislana.Domain.Exceptions;
using Dislana.Infrastructure.Persistence.Dapper;

namespace Dislana.Infrastructure.Persistence.Repositories.Auth
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbExecutor _dbExecutor;

        public UserRepository(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<UserEntity?> CreateUserWithCredentialAsync(
            string name,
            string lastName,
            string email,
            string passwordHash,
            CancellationToken cancellationToken)
        {
            const string createUserSql = @"
                DECLARE @UserId BIGINT = NEXT VALUE FOR UserSequence;

                INSERT INTO Users (Id, Email, FirstName, LastName, IsActive, CreatedAt)
                VALUES (@UserId, @Email, @Name, @LastName, 1, GETDATE());

                SELECT 
                    Id,
                    Email,
                    FirstName,
                    LastName,
                    IsActive,
                    CreatedAt
                FROM Users
                WHERE Id = @UserId;
            ";

            const string createCredentialSql = @"
                INSERT INTO UserCredentials (UserId, PasswordHash, PasswordChangedAt)
                VALUES (@UserId, @PasswordHash, GETDATE());
            ";

            return await _dbExecutor.ExecuteInTransactionAsync(async (connection, transaction, ct) =>
            {
                var userCmd = new CommandDefinition(
                    createUserSql,
                    new { Name = name, LastName = lastName, Email = email },
                    transaction: transaction,
                    cancellationToken: ct);

                var user = await connection.QuerySingleOrDefaultAsync<UserEntity>(userCmd);

                if (user is null)
                {
                    throw new DomainException("No se pudo crear el usuario.");
                }

                var credCmd = new CommandDefinition(
                    createCredentialSql,
                    new { UserId = user.Id, PasswordHash = passwordHash },
                    transaction: transaction,
                    cancellationToken: ct);

                await connection.ExecuteAsync(credCmd);

                return user;
            }, cancellationToken: cancellationToken);
        }

        public async Task<UserEntity?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT 
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.IsActive
                FROM Users u
                WHERE u.UserName = @UserName
            ";

            return await _dbExecutor.QuerySingleOrDefaultAsync<UserEntity>(
                sql,
                new { UserName = userName },
                commandType: null,
                cancellationToken: cancellationToken);
        }

        public async Task<UserEntity?> GetUserByIdAsync(long userId, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT 
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.IsActive
                FROM Users u
                WHERE u.Id = @UserId
            ";

            return await _dbExecutor.QuerySingleOrDefaultAsync<UserEntity>(
                sql,
                new { UserId = userId },
                commandType: null,
                cancellationToken: cancellationToken);
        }
    }
}
