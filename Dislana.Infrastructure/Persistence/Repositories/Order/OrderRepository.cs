using Dislana.Domain.Order.Entities;
using Dislana.Domain.Order.Interfaces;
using Dislana.Domain.Order.Results;
using Dislana.Infrastructure.Persistence.Dapper;
using System.Data;

namespace Dislana.Infrastructure.Persistence.Repositories.Order
{
    /// <summary>
    /// Implementación del repositorio de órdenes (Infrastructure Layer)
    /// Traduce entre Domain Entities y base de datos
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly IDbExecutor _dbExecutor;

        public OrderRepository(IDbExecutor dbExecutor) => _dbExecutor = dbExecutor;

        /// <summary>
        /// Guarda una OrderEntity rica en la base de datos
        /// La entidad sabe cómo serializarse a XML
        /// </summary>
        public async Task<OrderSaveResult?> SaveAsync(OrderEntity order, CancellationToken cancellationToken)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            // Validar antes de persistir (reglas de dominio)
            order.ValidateForSave();

            const string spName = "usp_saveOrder";

            // La entidad Domain sabe cómo convertirse a XML
            var pedidoXml = order.ToXml();

            var message = await _dbExecutor.QuerySingleOrDefaultAsync<string?>(
                spName,
                new
                {
                    login = order.CreatedBy,
                    pedido = pedidoXml,
                    observacion = order.Observation
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            if (message == null)
                return null;

            return OrderSaveResult.Success(message);
        }

        /// <summary>
        /// Obtiene acabados de tela desde la base de datos
        /// Dapper mapea automáticamente a FabricFinishEntity
        /// </summary>
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
