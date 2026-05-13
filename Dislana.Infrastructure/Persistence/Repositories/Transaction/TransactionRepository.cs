using Dislana.Domain.Common.Enums;
using Dislana.Domain.Transaction.Entities;
using Dislana.Domain.Transaction.Interfaces;
using Dislana.Infrastructure.Persistence.Dapper;
using System.Data;

namespace Dislana.Infrastructure.Persistence.Repositories.Transaction
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IContextualDbExecutor _dbExecutor;
        private const DatabaseContext Context = DatabaseContext.Ecommerce;

        public TransactionRepository(IContextualDbExecutor dbExecutor) => _dbExecutor = dbExecutor;

        public async Task<IEnumerable<TransactionEntity>> GetTransactionListAsync(string login, CancellationToken cancellationToken)
        {
            const string spName = "usp_getDocumentList";

            var result = await _dbExecutor.QueryAsync<TransactionEntity>(
                Context,
                spName,
                new { login },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result;
        }
    }
}
