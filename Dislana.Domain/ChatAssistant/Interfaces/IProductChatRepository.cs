using Dislana.Domain.ChatAssistant.Entities;

namespace Dislana.Domain.ChatAssistant.Interfaces
{
    public interface IProductChatRepository
    {
        Task<IEnumerable<ProductEntity>> GetActiveProductsAsync(CancellationToken cancellationToken);
    }
}
