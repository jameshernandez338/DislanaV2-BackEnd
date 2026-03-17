using Dislana.Domain.Quote.Entities;
using Dislana.Domain.Quote.Interfaces;
using Dislana.Infrastructure.Persistence.Dapper;
using System.Data;

namespace Dislana.Infrastructure.Persistence.Repositories.Quote
{
    public class QuoteRepository : IQuoteRepository
    {
        private readonly IDbExecutor _dbExecutor;

        public QuoteRepository(IDbExecutor dbExecutor) => _dbExecutor = dbExecutor;

        public async Task<IEnumerable<QuoteEntity>> GetQuotesAsync(string login, CancellationToken cancellationToken)
        {
            const string spName = "usp_getQuotes";

            var result = await _dbExecutor.QueryAsync<QuoteEntity>(
                spName,
                new { login = login },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result;
        }

        public async Task<CustomerBalanceEntity?> GetCustomerBalanceAsync(string login, CancellationToken cancellationToken)
        {
            const string spName = "usp_getCustomerBalance";

            var result = await _dbExecutor.QueryAsync<CustomerBalanceEntity>(
                spName,
                new { login },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken
            );

            var row = result.FirstOrDefault();

            return row;
        }
    }
}
