using Dislana.Domain.Quote.Entities;

namespace Dislana.Domain.Quote.Interfaces
{
    public interface IQuoteRepository
    {
        Task<IEnumerable<QuoteEntity>> GetQuotesAsync(string login, CancellationToken cancellationToken);
        Task<CustomerTaxEntity?> GetCustomerTaxesAsync(string login, CancellationToken cancellationToken);
        Task<IEnumerable<CustomerBalanceEntryEntity>> GetCustomerOverdueBalance(string login, CancellationToken cancellationToken);
        Task<IEnumerable<CustomerBalanceEntryEntity>> GetCustomerCreditBalance(string login, CancellationToken cancellationToken);
        Task<IEnumerable<CustomerBalanceEntryEntity>> GetCustomerApin(string login, CancellationToken cancellationToken);
    }
}
