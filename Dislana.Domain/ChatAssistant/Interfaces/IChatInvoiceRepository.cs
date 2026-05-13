using Dislana.Domain.ChatAssistant.Entities;

namespace Dislana.Domain.ChatAssistant.Interfaces
{
    public interface IChatInvoiceRepository
    {
        Task<IEnumerable<ChatInvoiceEntity>> GetChatInvoiceByUserIdAsync(string userId, CancellationToken cancellationToken);
    }
}
