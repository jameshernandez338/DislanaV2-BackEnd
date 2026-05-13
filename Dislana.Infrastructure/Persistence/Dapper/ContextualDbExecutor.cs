using Dapper;
using Dislana.Application.Common.Interfaces;
using Dislana.Domain.Common.Enums;
using System.Data;
using System.Data.Common;

namespace Dislana.Infrastructure.Persistence.Dapper
{
    public class ContextualDbExecutor : IContextualDbExecutor
    {
        private readonly DbConnectionFactory _connectionFactory;
        private readonly IConnectionStringResolver _connectionStringResolver;

        public ContextualDbExecutor(
            DbConnectionFactory connectionFactory,
            IConnectionStringResolver connectionStringResolver)
        {
            _connectionFactory = connectionFactory;
            _connectionStringResolver = connectionStringResolver;
        }

        private static CommandDefinition CreateCommand(
            string sql,
            object? parameters,
            CommandType? commandType,
            DbTransaction? transaction,
            CancellationToken cancellationToken)
            => new CommandDefinition(
                commandText: sql,
                parameters: parameters,
                transaction: transaction,
                commandType: commandType,
                cancellationToken: cancellationToken);

        public async Task<T?> QuerySingleOrDefaultAsync<T>(
            DatabaseContext context,
            string sql,
            object? parameters = null,
            CommandType? commandType = null,
            CancellationToken cancellationToken = default)
        {
            var connectionString = _connectionStringResolver.GetConnectionString(context);
            await using var connection = _connectionFactory.Create(connectionString);
            await connection.OpenAsync(cancellationToken);

            var cmd = CreateCommand(sql, parameters, commandType, null, cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<T>(cmd);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(
            DatabaseContext context,
            string sql,
            object? parameters = null,
            CommandType? commandType = null,
            CancellationToken cancellationToken = default)
        {
            var connectionString = _connectionStringResolver.GetConnectionString(context);
            await using var connection = _connectionFactory.Create(connectionString);
            await connection.OpenAsync(cancellationToken);

            var cmd = CreateCommand(sql, parameters, commandType, null, cancellationToken);
            return await connection.QueryAsync<T>(cmd);
        }

        public async Task<int> ExecuteAsync(
            DatabaseContext context,
            string sql,
            object? parameters = null,
            CommandType? commandType = null,
            CancellationToken cancellationToken = default)
        {
            var connectionString = _connectionStringResolver.GetConnectionString(context);
            await using var connection = _connectionFactory.Create(connectionString);
            await connection.OpenAsync(cancellationToken);

            var cmd = CreateCommand(sql, parameters, commandType, null, cancellationToken);
            return await connection.ExecuteAsync(cmd);
        }

        public async Task<T> ExecuteInTransactionAsync<T>(
            DatabaseContext context,
            Func<DbConnection, DbTransaction, CancellationToken, Task<T>> operation,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            var connectionString = _connectionStringResolver.GetConnectionString(context);
            await using var connection = _connectionFactory.Create(connectionString);
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

        public async Task ExecuteInTransactionAsync(
            DatabaseContext context,
            Func<DbConnection, DbTransaction, CancellationToken, Task> operation,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default)
        {
            await ExecuteInTransactionAsync<object?>(context, async (c, t, ct) =>
            {
                await operation(c, t, ct);
                return null;
            }, isolationLevel, cancellationToken);
        }
    }
}
