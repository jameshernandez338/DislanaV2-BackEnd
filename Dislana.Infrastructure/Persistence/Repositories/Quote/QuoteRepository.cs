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

        public async Task<IEnumerable<QuoteDetailEntity>> GetQuoteDetailAsync(string userId, string item, CancellationToken cancellationToken)
        {
            const string spName = "usp_getQuoteDetail";

            var result = await _dbExecutor.QueryAsync<QuoteDetailDbModel>(
                Context,
                spName,
                new { login = userId, item },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result.Select(r => QuoteDetailEntity.Create(
                r.Codigo,
                r.Separados,
                r.Calidad,
                r.Imagen,
                r.Cantidad,
                r.PrecioTotal
            ));
        }

        public async Task<IEnumerable<CustomerAddressEntity>> GetCustomerAddressAsync(int login, CancellationToken cancellationToken)
        {
            const string spName = "usp_getCustomerAddress";

            var result = await _dbExecutor.QueryAsync<CustomerAddressDbModel>(
                Context,
                spName,
                new { login },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result.Select(r => CustomerAddressEntity.Create(
                r.COD_CIU,
                r.NOM_CIU,
                r.direccion
            ));
        }

        public async Task<CustomerTaxEntity?> GetCustomerTaxesAsync(int login, CancellationToken cancellationToken)
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

        public async Task<IEnumerable<CustomerBalanceEntryEntity>> GetCustomerOverdueBalance(int login, CancellationToken cancellationToken)
        {
            const string spName = "usp_getCustomerOverdueBalance";

            var result = await _dbExecutor.QueryAsync<CustomerBalanceEntryEntity>(
                Context,
                spName,
                new { login },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result;
        }

        public async Task<IEnumerable<CustomerBalanceEntryEntity>> GetCustomerCreditBalance(int login, CancellationToken cancellationToken)
        {
            const string spName = "usp_getCustomerCreditBalance";

            var result = await _dbExecutor.QueryAsync<CustomerBalanceEntryEntity>(
                Context,
                spName,
                new { login },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result;
        }

        public async Task<IEnumerable<CustomerBalanceEntryEntity>> GetCustomerApin(int login, CancellationToken cancellationToken)
        {
            const string spName = "usp_getCustomerApin";

            var result = await _dbExecutor.QueryAsync<CustomerBalanceEntryEntity>(
                Context,
                spName,
                new { login },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result;
        }

        private class QuoteDetailDbModel
        {
            public string Codigo { get; set; } = string.Empty;
            public decimal Separados { get; set; }
            public string Calidad { get; set; } = string.Empty;
            public string Imagen { get; set; } = string.Empty;
            public decimal Cantidad { get; set; }
            public decimal PrecioTotal { get; set; }
        }

        private class CustomerAddressDbModel
        {
            public string COD_CIU { get; set; } = string.Empty;
            public string NOM_CIU { get; set; } = string.Empty;
            public string direccion { get; set; } = string.Empty;
        }
    }
}
