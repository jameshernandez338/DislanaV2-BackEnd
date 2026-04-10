using Dislana.Domain.Order.Entities;
using Dislana.Domain.Order.Interfaces;
using Dislana.Infrastructure.Persistence.Dapper;
using System.Data;

namespace Dislana.Infrastructure.Persistence.Repositories.Order
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbExecutor _dbExecutor;

        public OrderRepository(IDbExecutor dbExecutor) => _dbExecutor = dbExecutor;

        public async Task<OrderSaveResult?> SaveOrderAsync(string login, string pedido, string observacion, CancellationToken cancellationToken)
        {
            const string spName = "usp_saveOrder";

            var message = await _dbExecutor.QuerySingleOrDefaultAsync<string?>(
                spName,
                new { login, pedido, observacion },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            if (message == null) return null;

            return new OrderSaveResult(message);
        }

        public async Task<IEnumerable<FabricFinishEntity>> GetFabricFinishesAsync(string login, CancellationToken cancellationToken)
        {
            const string spName = "usp_getFabricFinishes";

            return await _dbExecutor.QueryAsync<FabricFinishEntity>(
                spName,
                new { login },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);
        }
    }
}
