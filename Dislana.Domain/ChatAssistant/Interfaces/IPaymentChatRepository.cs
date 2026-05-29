using Dislana.Domain.ChatAssistant.Entities;

namespace Dislana.Domain.ChatAssistant.Interfaces
{
    public interface IPaymentChatRepository
    {
        Task<IEnumerable<PaymentEntity>> GetPaymentsByUserIdAsync(string userId, CancellationToken cancellationToken);
    }
}
