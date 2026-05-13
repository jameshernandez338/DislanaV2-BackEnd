using Dislana.Domain.Common.Enums;
using Dislana.Domain.Quote.Entities;
using Dislana.Domain.Quote.Interfaces;
using Dislana.Infrastructure.Persistence.Dapper;
using System.Data;

namespace Dislana.Infrastructure.Persistence.Repositories.Quote
{
    public class QuoteRepository : IQuoteRepository
    {
        private readonly IContextualDbExecutor _dbExecutor;
        private const DatabaseContext Context = DatabaseContext.Ecommerce;

        public QuoteRepository(IContextualDbExecutor dbExecutor) => _dbExecutor = dbExecutor;

        public async Task<IEnumerable<QuoteEntity>> GetQuotesAsync(string userId, CancellationToken cancellationToken)
        {
            const string spName = "usp_getQuotes";

            var result = await _dbExecutor.QueryAsync<QuoteEntity>(
                Context,
                spName,
                new { login = userId },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result;
        }

        public async Task<CustomerTaxEntity?> GetCustomerTaxesAsync(string login, CancellationToken cancellationToken)
        {
            const string spName = "usp_getCustomerTaxes";

            var result = await _dbExecutor.QueryAsync<CustomerTaxEntity>(
                Context,
                spName,
                new { login },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            var row = result.FirstOrDefault();

            return row;
        }

        public async Task<IEnumerable<CustomerBalanceEntryEntity>> GetCustomerOverdueBalance(string login, CancellationToken cancellationToken)
        {
            const string spName = "usp_getCustomerOverdueBalance";

            var result = await _dbExecutor.QueryAsync<CustomerBalanceEntryEntity>(
                Context,
                spName,
                new { login = login },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result;
        }

        public async Task<IEnumerable<CustomerBalanceEntryEntity>> GetCustomerCreditBalance(string login, CancellationToken cancellationToken)
        {
            const string spName = "usp_getCustomerCreditBalance";

            var result = await _dbExecutor.QueryAsync<CustomerBalanceEntryEntity>(
                Context,
                spName,
                new { login = login },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result;
        }

        public async Task<IEnumerable<CustomerBalanceEntryEntity>> GetCustomerApin(string login, CancellationToken cancellationToken)
        {
            const string spName = "usp_getCustomerApin";

            var result = await _dbExecutor.QueryAsync<CustomerBalanceEntryEntity>(
                Context,
                spName,
                new { login = login },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result;
        }
    }
}
