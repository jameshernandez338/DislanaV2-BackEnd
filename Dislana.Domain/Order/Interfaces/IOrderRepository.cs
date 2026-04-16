using Dislana.Domain.Order.Entities;
using Dislana.Domain.Order.Results;

namespace Dislana.Domain.Order.Interfaces
{
    /// <summary>
    /// Interfaz del repositorio de órdenes (Domain Layer)
    /// NO depende de Infrastructure
    /// </summary>
    public interface IOrderRepository
    {
        /// <summary>
        /// Guarda una orden completa (Aggregate Root)
        /// </summary>
        Task<OrderSaveResult?> SaveAsync(OrderEntity order, CancellationToken cancellationToken);

        /// <summary>
        /// Obtiene los acabados de tela disponibles para un usuario
        /// </summary>
        Task<IEnumerable<FabricFinishEntity>> GetFabricFinishesAsync(string login, CancellationToken cancellationToken);
    }
}
