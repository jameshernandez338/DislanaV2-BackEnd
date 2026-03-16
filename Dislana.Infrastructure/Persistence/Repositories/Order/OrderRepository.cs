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

        public async Task<OrderSaveResult?> SaveOrderAsync(string login, string pedido, string orillo, CancellationToken cancellationToken)
        {
            const string spName = "usp_saveOrder";

            var message = await _dbExecutor.QuerySingleOrDefaultAsync<string?>(
                spName,
                new { login = login, pedido = pedido, orillo = orillo },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            if (message == null) return null;

            return new OrderSaveResult(message);
        }
    }
}
