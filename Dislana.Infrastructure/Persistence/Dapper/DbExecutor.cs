using Dapper;
using System.Data;
using System.Data.Common;

namespace Dislana.Infrastructure.Persistence.Dapper
{
    public class DbExecutor : IDbExecutor
    {
        private readonly DbConnectionFactory _connectionFactory;

        public DbExecutor(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        private static CommandDefinition CreateCommand(string sql, object? parameters, CommandType? commandType, DbTransaction? transaction, CancellationToken cancellationToken)
            => new CommandDefinition(
                commandText: sql,
                parameters: parameters,
                transaction: transaction,
                commandType: commandType,
                cancellationToken: cancellationToken);

        public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            await using var connection = _connectionFactory.Create();
            await connection.OpenAsync(cancellationToken);

            var cmd = CreateCommand(sql, parameters, commandType, null, cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<T>(cmd);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            await using var connection = _connectionFactory.Create();
            await connection.OpenAsync(cancellationToken);

            var cmd = CreateCommand(sql, parameters, commandType, null, cancellationToken);
            return await connection.QueryAsync<T>(cmd);
        }

        public async Task<int> ExecuteAsync(string sql, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            await using var connection = _connectionFactory.Create();
            await connection.OpenAsync(cancellationToken);

            var cmd = CreateCommand(sql, parameters, commandType, null, cancellationToken);
            return await connection.ExecuteAsync(cmd);
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<DbConnection, DbTransaction, CancellationToken, Task<T>> operation, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
        {
            await using var connection = _connectionFactory.Create();
            await connection.OpenAsync(cancellationToken);

            await using var transaction = await connection.BeginTransactionAsync(isolationLevel, cancellationToken);

            try
            {
                var result = await operation(connection, transaction, cancellationToken);
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                try { await transaction.RollbackAsync(cancellationToken); } catch { /* swallow */ }
                throw;
            }
        }

        public async Task ExecuteInTransactionAsync(Func<DbConnection, DbTransaction, CancellationToken, Task> operation, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
        {
            await ExecuteInTransactionAsync<object?>(async (c, t, ct) =>
            {
                await operation(c, t, ct);
                return null;
            }, isolationLevel, cancellationToken);
        }
    }
}
