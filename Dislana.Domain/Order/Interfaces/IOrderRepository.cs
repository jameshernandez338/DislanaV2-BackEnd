using Dislana.Domain.Order.Entities;

namespace Dislana.Domain.Order.Interfaces
{
    public interface IOrderRepository
    {
        Task<OrderSaveResult?> SaveOrderAsync(string login, string pedido, string orillo, CancellationToken cancellationToken);
    }
}
