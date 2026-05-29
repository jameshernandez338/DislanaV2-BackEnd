using Dislana.Domain.ChatAssistant.Entities;
using Dislana.Domain.ChatAssistant.Interfaces;
using Dislana.Domain.Common.Enums;
using Dislana.Infrastructure.ChatAssistant.DTOs;
using Dislana.Infrastructure.Persistence.Dapper;

namespace Dislana.Infrastructure.ChatAssistant.Repositories
{
    public class PaymentChatRepository : IPaymentChatRepository
    {
        private readonly IContextualDbExecutor _dbExecutor;
        private const DatabaseContext Context = DatabaseContext.ChatBot;

        public PaymentChatRepository(IContextualDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<IEnumerable<PaymentEntity>> GetPaymentsByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (!int.TryParse(userId, out var userIdInt))
            {
                return Enumerable.Empty<PaymentEntity>();
            }

            var query = @"
                SELECT 
                    login AS Login,
                    cod_cli AS CodCli,
                    tipo AS Tipo,
                    numero AS Numero,
                    pago AS Pago,
                    referencia AS Referencia,
                    fecha AS Fecha
                FROM dbo.pagos
                WHERE login = @UserId
                ORDER BY fecha DESC";

            var dtos = await _dbExecutor.QueryAsync<PaymentDto>(
                Context, 
                query, 
                new { UserId = userIdInt }, 
                null, 
                cancellationToken);

            return dtos.Select(dto => new PaymentEntity
            {
                Login = dto.Login,
                CodCli = dto.CodCli?.Trim() ?? string.Empty,
                Tipo = dto.Tipo?.Trim() ?? string.Empty,
                Numero = dto.Numero?.Trim() ?? string.Empty,
                Pago = dto.Pago,
                Referencia = dto.Referencia?.Trim() ?? string.Empty,
                Fecha = dto.Fecha
            });
        }
    }
}
