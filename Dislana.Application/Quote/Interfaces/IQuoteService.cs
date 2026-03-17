using Dislana.Application.Quote.DTOs;

namespace Dislana.Application.Quote.Interfaces
{
    public interface IQuoteService
    {
        Task<IReadOnlyList<QuoteDto>> GetQuotesAsync(string login, CancellationToken cancellationToken);
        Task<CustomerBalanceDto?> GetCustomerBalanceAsync(string login, CancellationToken cancellationToken);
    }
}
