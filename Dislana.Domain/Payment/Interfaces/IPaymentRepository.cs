using Dislana.Domain.Order.Entities;

namespace Dislana.Domain.Payment.Interfaces
{
    public interface IPaymentRepository
    {
        Task SavePaymentAsync(string login, string reference, string status, string pedido, decimal valor, CancellationToken cancellationToken);
    }
}
