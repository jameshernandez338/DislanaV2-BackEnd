using Dislana.Domain.Common.Enums;
using Dislana.Domain.Payment.Interfaces;
using Dislana.Infrastructure.Persistence.Dapper;
using System.Data;

namespace Dislana.Infrastructure.Persistence.Repositories.Payment
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IContextualDbExecutor _dbExecutor;
        private const DatabaseContext Context = DatabaseContext.Ecommerce;

        public PaymentRepository(IContextualDbExecutor dbExecutor) => _dbExecutor = dbExecutor;

        public async Task SavePaymentAsync(int userId, string reference, string status, string pedido, decimal valor, string direccionEntrega, CancellationToken cancellationToken)
        {
            const string spName = "usp_savePaymentOrder";

            var message = await _dbExecutor.QuerySingleOrDefaultAsync<string?>(
                Context,
                spName,
                new { 
                    userName = userId,
                    reference,
                    status,
                    detail = pedido,
                    amount = valor,
                    deliveryAddress = direccionEntrega
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);
        }

        public async Task UpdatePaymentAsync(string reference, string status, string transactionId, string paymentMethod, string timestamp)
        {
            const string spName = "usp_updatePaymentStatus";

            await _dbExecutor.ExecuteAsync(
                Context,
                spName,
                new { 
                    reference, 
                    status, 
                    transactionId, 
                    paymentMethod, 
                    timestamp 
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task SavePaymentLogAsync(string reference, string payload, string message)
        {
            const string spName = "usp_savePaymentLog";

            await _dbExecutor.ExecuteAsync(
                Context,
                spName,
                new { 
                    reference, 
                    payload,
                    message 
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}
