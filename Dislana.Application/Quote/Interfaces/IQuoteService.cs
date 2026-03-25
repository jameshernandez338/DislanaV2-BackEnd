using Dislana.Application.Quote.DTOs;

namespace Dislana.Application.Quote.Interfaces
{
    public interface IQuoteService
    {
        Task<IReadOnlyList<QuoteDto>> GetQuotesAsync(string userId, CancellationToken cancellationToken);
        Task<CustomerTaxDto?> GetCustomerTaxesAsync(string login, CancellationToken cancellationToken);
        Task<IReadOnlyList<CustomerBalanceEntryDto>> GetCustomerBalanceAsync(string login, string type, CancellationToken cancellationToken);
    }
}
