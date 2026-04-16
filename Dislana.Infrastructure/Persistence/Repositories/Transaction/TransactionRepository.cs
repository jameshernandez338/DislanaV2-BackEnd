using Dislana.Domain.Transaction.Entities;
using Dislana.Domain.Transaction.Interfaces;
using Dislana.Infrastructure.Persistence.Dapper;
using System.Data;

namespace Dislana.Infrastructure.Persistence.Repositories.Transaction
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IDbExecutor _dbExecutor;

        public TransactionRepository(IDbExecutor dbExecutor) => _dbExecutor = dbExecutor;

        public async Task<IEnumerable<TransactionEntity>> GetTransactionListAsync(string login, CancellationToken cancellationToken)
        {
            const string spName = "usp_getDocumentList";

            var result = await _dbExecutor.QueryAsync<TransactionEntity>(
                spName,
                new { login },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result;
        }
    }
}
