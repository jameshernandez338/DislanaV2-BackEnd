using Dislana.Domain.Payment.Interfaces;
using Dislana.Infrastructure.Persistence.Dapper;
using System.Data;

namespace Dislana.Infrastructure.Persistence.Repositories.Payment
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDbExecutor _dbExecutor;

        public PaymentRepository(IDbExecutor dbExecutor) => _dbExecutor = dbExecutor;

        public async Task SavePaymentAsync(string login, string reference, string status, string pedido, decimal valor, CancellationToken cancellationToken)
        {
            const string spName = "usp_saveOrder";

            var message = await _dbExecutor.QuerySingleOrDefaultAsync<string?>(
                spName,
                new { login = login, pedido = pedido },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);
        }
    }
}
