using Dislana.Domain.Common.Enums;
using System.Data;
using System.Data.Common;

namespace Dislana.Infrastructure.Persistence.Dapper
{
    public interface IContextualDbExecutor
    {
        /// <summary>
        /// Ejecuta una consulta que devuelve una sola fila (o null) en un contexto específico
        /// </summary>
        Task<T?> QuerySingleOrDefaultAsync<T>(
            DatabaseContext context,
            string sql,
            object? parameters = null,
            CommandType? commandType = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ejecuta una consulta que devuelve múltiples filas en un contexto específico
        /// </summary>
        Task<IEnumerable<T>> QueryAsync<T>(
            DatabaseContext context,
            string sql,
            object? parameters = null,
            CommandType? commandType = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ejecuta un comando (INSERT/UPDATE/DELETE) en un contexto específico
        /// </summary>
        Task<int> ExecuteAsync(
            DatabaseContext context,
            string sql,
            object? parameters = null,
            CommandType? commandType = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ejecuta una operación compuesta dentro de una transacción en un contexto específico y devuelve un resultado
        /// </summary>
        Task<T> ExecuteInTransactionAsync<T>(
            DatabaseContext context,
            Func<DbConnection, DbTransaction, CancellationToken, Task<T>> operation,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Ejecuta una operación compuesta dentro de una transacción en un contexto específico (sin resultado)
        /// </summary>
        Task ExecuteInTransactionAsync(
            DatabaseContext context,
            Func<DbConnection, DbTransaction, CancellationToken, Task> operation,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
            CancellationToken cancellationToken = default);
    }
}
