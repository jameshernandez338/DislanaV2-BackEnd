using Dislana.Domain.Order.Entities;

namespace Dislana.Domain.Order.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderSaveResult?> SaveOrderAsync(string login, string pedido, string observacion, CancellationToken cancellationToken);
        Task<IEnumerable<FabricFinishEntity>> GetFabricFinishesAsync(string login, CancellationToken cancellationToken);
    }
}
