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

        public async Task UpdatePaymentAsync(string reference, string status, string transactionId, string paymentMethod, string timestamp)
        {
            const string spName = "usp_updatePaymentStatus";

            await _dbExecutor.ExecuteAsync(
                spName,
                new { reference = reference, status = status, transactionId = transactionId, paymentMethod = paymentMethod, timestamp = timestamp },
                commandType: CommandType.StoredProcedure);
        }

        public async Task SavePaymentLogAsync(string reference, string payload, string message)
        {
            const string spName = "usp_savePaymentLog";

            await _dbExecutor.ExecuteAsync(
                spName,
                new { reference = reference, payload = payload, message = message },
                commandType: CommandType.StoredProcedure);
        }
    }
}
