using Dislana.Domain.Quote.Entities;

namespace Dislana.Domain.Quote.Interfaces
{
    public interface IQuoteRepository
    {
        Task<IEnumerable<QuoteEntity>> GetQuotesAsync(string userId, CancellationToken cancellationToken);
        Task<IEnumerable<QuoteDetailEntity>> GetQuoteDetailAsync(string userId, string item, CancellationToken cancellationToken);
        Task<IEnumerable<CustomerAddressEntity>> GetCustomerAddressAsync(int login, CancellationToken cancellationToken);
        Task<CustomerTaxEntity?> GetCustomerTaxesAsync(int login, CancellationToken cancellationToken);
        Task<IEnumerable<CustomerBalanceEntryEntity>> GetCustomerOverdueBalance(int login, CancellationToken cancellationToken);
        Task<IEnumerable<CustomerBalanceEntryEntity>> GetCustomerCreditBalance(int login, CancellationToken cancellationToken);
        Task<IEnumerable<CustomerBalanceEntryEntity>> GetCustomerApin(int login, CancellationToken cancellationToken);
    }
}
