using System.Data;
using System.Data.Common;

namespace Dislana.Infrastructure.Persistence.Dapper
{
    public interface IDbExecutor
    {
        // Ejecuta una consulta que devuelve una sola fila (o null)
        Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default);

        // Ejecuta una consulta que devuelve múltiples filas
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default);

        // Ejecuta un comando (INSERT/UPDATE/DELETE)
        Task<int> ExecuteAsync(string sql, object? parameters = null, CommandType? commandType = null, CancellationToken cancellationToken = default);

        // Ejecuta una operación compuesta dentro de una transacción y devuelve un resultado
        Task<T> ExecuteInTransactionAsync<T>(Func<DbConnection, DbTransaction, CancellationToken, Task<T>> operation, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

        // Ejecuta una operación compuesta dentro de una transacción (sin resultado)
        Task ExecuteInTransactionAsync(Func<DbConnection, DbTransaction, CancellationToken, Task> operation, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);
    }
}
