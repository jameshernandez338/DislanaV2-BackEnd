using Dislana.Domain.Stock.Entities;
using Dislana.Domain.Stock.Interfaces;
using Dislana.Infrastructure.Persistence.Dapper;
using System.Data;

namespace Dislana.Infrastructure.Persistence.Repositories.Stock
{
    public class StockRepository : IStockRepository
    {
        private readonly IDbExecutor _dbExecutor;

        public StockRepository(IDbExecutor dbExecutor) => _dbExecutor = dbExecutor;

        public async Task<IEnumerable<CommittedInventoryEntity>> GetCommittedInventoryAsync(string login, string itemCode, CancellationToken cancellationToken)
        {
            const string spName = "usp_getCommittedInventory";

            var result = await _dbExecutor.QueryAsync<CommittedInventoryEntity>(
                spName,
                new { login = login, item = itemCode },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result;
        }

        public async Task<IEnumerable<InventoryStatementEntity>> GetInventoryStatementAsync(string login, CancellationToken cancellationToken)
        {
            const string spName = "usp_getInventoryStatement";

            var result = await _dbExecutor.QueryAsync<InventoryStatementEntity>(
                spName,
                new { login = login },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result;
        }
    }
}
