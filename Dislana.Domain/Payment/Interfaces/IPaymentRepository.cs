namespace Dislana.Domain.Payment.Interfaces
{
    public interface IPaymentRepository
    {
        Task SavePaymentAsync(int userId, string reference, string status, string pedido, decimal valor, string direccionEntrega, CancellationToken cancellationToken);
        Task UpdatePaymentAsync(string reference, string status, string transactionId, string paymentMethod, string timestamp);
        Task SavePaymentLogAsync(string reference, string payload, string message);
    }
}
