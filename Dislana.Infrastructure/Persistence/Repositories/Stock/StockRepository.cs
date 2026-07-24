using Dislana.Domain.Common.Enums;
using Dislana.Domain.Stock.Entities;
using Dislana.Domain.Stock.Interfaces;
using Dislana.Infrastructure.Persistence.Dapper;
using System.Data;

namespace Dislana.Infrastructure.Persistence.Repositories.Stock
{
    public class StockRepository : IStockRepository
    {
        private readonly IContextualDbExecutor _dbExecutor;
        private const DatabaseContext Context = DatabaseContext.Ecommerce;

        public StockRepository(IContextualDbExecutor dbExecutor) => _dbExecutor = dbExecutor;

        public async Task<IEnumerable<CommittedInventoryEntity>> GetCommittedInventoryAsync(int userId, string itemCode, CancellationToken cancellationToken)
        {
            const string spName = "usp_getCommittedInventory";

            var result = await _dbExecutor.QueryAsync<CommittedInventoryEntity>(
                Context,
                spName,
                new { login = userId, itemCode },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result;
        }

        public async Task<IEnumerable<InventoryStatementEntity>> GetInventoryStatementAsync(int userId, CancellationToken cancellationToken)
        {
            const string spName = "usp_getInventoryStatement";

            var result = await _dbExecutor.QueryAsync<InventoryStatementEntity>(
                Context,
                spName,
                new { login = userId },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result;
        }

        public async Task CancelOrderAsync(string document, string item, CancellationToken cancellationToken)
        {
            const string spName = "usp_saveCancelOrder";

            await _dbExecutor.ExecuteAsync(
                Context,
                spName,
                new { document, item },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);
        }
    }
}
