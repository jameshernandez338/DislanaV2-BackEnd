using Dislana.Domain.AccountStatement.Entities;
using Dislana.Domain.AccountStatement.Interfaces;
using Dislana.Domain.Common.Enums;
using Dislana.Infrastructure.Persistence.Dapper;
using System.Data;

namespace Dislana.Infrastructure.Persistence.Repositories.AccountStatement
{
    public class AccountStatementRepository : IAccountStatementRepository
    {
        private readonly IContextualDbExecutor _dbExecutor;
        private const DatabaseContext Context = DatabaseContext.Ecommerce;

        public AccountStatementRepository(IContextualDbExecutor dbExecutor) => _dbExecutor = dbExecutor;

        public async Task<IEnumerable<AccountStatementEntity>> GetAccountStatementAsync(
            int userId,
            DateTime startDate,
            DateTime endDate,
            string? documentType,
            CancellationToken cancellationToken)
        {
            const string spName = "usp_getExtracto";

            var result = await _dbExecutor.QueryAsync<AccountStatementDbModel>(
                Context,
                spName,
                new { 
                    cliente = userId, 
                    fecha1 = startDate, 
                    fecha2 = endDate,
                    tipo = documentType
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result.Select(r => AccountStatementEntity.Create(
                r.fch_doc,
                r.fec_ven,
                r.num_doc,
                r.val_doc,
                r.sal_doc,
                r.nom_tip,
                r.tipo
            ));
        }

        public async Task<IEnumerable<AccountStatementDetailEntity>> GetAccountStatementDetailAsync(
            int userId,
            string documentNumber,
            CancellationToken cancellationToken)
        {
            const string spName = "usp_getExtractoDet";

            var result = await _dbExecutor.QueryAsync<AccountStatementDbModel>(
                Context,
                spName,
                new { 
                    cliente = userId,
                    document= documentNumber
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result.Select(r => AccountStatementDetailEntity.Create(
                r.fch_doc,
                r.num_doc,
                r.val_doc,
                r.nom_tip
            ));
        }

        private class AccountStatementDbModel
        {
            public DateTime fch_doc { get; set; }
            public DateTime fec_ven { get; set; }
            public string num_doc { get; set; } = string.Empty;
            public decimal val_doc { get; set; }
            public decimal sal_doc { get; set; }
            public string nom_tip { get; set; } = string.Empty;
            public string tipo { get; set; } = string.Empty;
        }
    }
}
