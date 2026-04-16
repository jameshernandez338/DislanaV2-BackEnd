using Dapper;
using Dislana.Domain.Auth.Entities;
using Dislana.Domain.Auth.Interfaces;
using Dislana.Domain.Exceptions;
using Dislana.Infrastructure.Persistence.Dapper;

namespace Dislana.Infrastructure.Persistence.Repositories.Auth
{
    /// <summary>
    /// Repositorio de Infraestructura: Implementa persistencia de usuarios
    /// Reconstruye entidades ricas desde la BD
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly IDbExecutor _dbExecutor;

        public UserRepository(IDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<UserEntity?> CreateUserWithCredentialAsync(
            UserEntity user,
            UserCredentialEntity credential,
            CancellationToken cancellationToken)
        {
            const string createUserSql = @"
                DECLARE @UserId BIGINT = NEXT VALUE FOR UserSequence;

                INSERT INTO Users (Id, Email, FirstName, LastName, IsActive, CreatedAt)
                VALUES (@UserId, @Email, @FirstName, @LastName, 1, GETDATE());

                SELECT 
                    Id,
                    Email AS UserName,
                    Email,
                    FirstName,
                    LastName,
                    IsActive
                FROM Users
                WHERE Id = @UserId;
            ";

            const string createCredentialSql = @"
                INSERT INTO UserCredentials (UserId, PasswordHash, PasswordChangedAt)
                VALUES (@UserId, @PasswordHash, GETDATE());
            ";

            return await _dbExecutor.ExecuteInTransactionAsync(async (connection, transaction, ct) =>
            {
                // Crear usuario
                var userCmd = new CommandDefinition(
                    createUserSql,
                    new
                    {
                        Email = user.Email.Value,
                        FirstName = user.FirstName.Value,
                        LastName = user.LastName.Value
                    },
                    transaction: transaction,
                    cancellationToken: ct);

                var dbUser = await connection.QuerySingleOrDefaultAsync<dynamic>(userCmd);

                if (dbUser is null)
                    throw new DomainException("No se pudo crear el usuario.");

                // Crear credenciales
                var credCmd = new CommandDefinition(
                    createCredentialSql,
                    new
                    {
                        UserId = (long)dbUser.Id,
                        PasswordHash = credential.PasswordHash.Value
                    },
                    transaction: transaction,
                    cancellationToken: ct);

                await connection.ExecuteAsync(credCmd);

                // Reconstruir entidad rica desde los datos de la BD
                return UserEntity.Reconstitute(
                    id: (long)dbUser.Id,
                    userName: (string)dbUser.UserName,
                    email: (string)dbUser.Email,
                    firstName: (string)dbUser.FirstName,
                    lastName: (string)dbUser.LastName,
                    isActive: (bool)dbUser.IsActive
                );
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

            var dbUser = await _dbExecutor.QuerySingleOrDefaultAsync<dynamic>(
                sql,
                new { UserName = userName },
                commandType: null,
                cancellationToken: cancellationToken);

            if (dbUser == null)
                return null;

            // Reconstruir entidad rica
            return UserEntity.Reconstitute(
                id: (long)dbUser.Id,
                userName: (string)dbUser.UserName,
                email: (string)dbUser.Email,
                firstName: (string)dbUser.FirstName,
                lastName: (string)dbUser.LastName,
                isActive: (bool)dbUser.IsActive
            );
        }

        public async Task<UserEntity?> GetUserByIdAsync(long userId, CancellationToken cancellationToken)
        {
            const string sql = @"
                SELECT 
                    u.Id,
                    u.Email AS UserName,
                    u.Email,
                    u.FirstName,
                    u.LastName,
                    u.IsActive
                FROM Users u
                WHERE u.Id = @UserId
            ";

            var dbUser = await _dbExecutor.QuerySingleOrDefaultAsync<dynamic>(
                sql,
                new { UserId = userId },
                commandType: null,
                cancellationToken: cancellationToken);

            if (dbUser == null)
                return null;

            // Reconstruir entidad rica
            return UserEntity.Reconstitute(
                id: (long)dbUser.Id,
                userName: (string)dbUser.UserName,
                email: (string)dbUser.Email,
                firstName: (string)dbUser.FirstName,
                lastName: (string)dbUser.LastName,
                isActive: (bool)dbUser.IsActive
            );
        }
    }
}
