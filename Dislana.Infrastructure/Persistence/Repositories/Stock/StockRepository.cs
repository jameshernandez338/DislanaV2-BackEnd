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

        public async Task<IEnumerable<CommittedInventoryEntity>> GetCommittedInventoryAsync(string login, string itemCode, CancellationToken cancellationToken)
        {
            const string spName = "usp_getCommittedInventory";

            var result = await _dbExecutor.QueryAsync<CommittedInventoryEntity>(
                Context,
                spName,
                new { login, itemCode },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result;
        }

        public async Task<IEnumerable<InventoryStatementEntity>> GetInventoryStatementAsync(string login, CancellationToken cancellationToken)
        {
            const string spName = "usp_getInventoryStatement";

            var result = await _dbExecutor.QueryAsync<InventoryStatementEntity>(
                Context,
                spName,
                new { login },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result;
        }
    }
}
