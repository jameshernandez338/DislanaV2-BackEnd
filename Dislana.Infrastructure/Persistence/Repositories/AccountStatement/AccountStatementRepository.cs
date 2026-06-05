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
            string login,
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken)
        {
            const string spName = "usp_getExtracto";

            var result = await _dbExecutor.QueryAsync<AccountStatementDbModel>(
                Context,
                spName,
                new { cliente = login, fecha1 = startDate, fecha2 = endDate },
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
