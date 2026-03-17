using Dislana.Domain.Quote.Entities;

namespace Dislana.Domain.Quote.Interfaces
{
    public interface IQuoteRepository
    {
        Task<IEnumerable<QuoteEntity>> GetQuotesAsync(string login, CancellationToken cancellationToken);
        Task<CustomerBalanceEntity?> GetCustomerBalanceAsync(string login, CancellationToken cancellationToken);
    }
}
