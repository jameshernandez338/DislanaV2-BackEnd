using Dislana.Application.Common.Interfaces;
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
        private readonly ICacheService _cacheService;
        private const DatabaseContext Context = DatabaseContext.ChatBot;
        private const string CacheKeyPrefix = "chat_payments";
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(1);

        public PaymentChatRepository(IContextualDbExecutor dbExecutor, ICacheService cacheService)
        {
            _dbExecutor = dbExecutor;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<PaymentEntity>> GetPaymentsByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (!int.TryParse(userId, out var userIdInt))
            {
                return Enumerable.Empty<PaymentEntity>();
            }

            var cacheKey = $"{CacheKeyPrefix}:user:{userId}";

            var cachedData = await _cacheService.GetAsync<List<PaymentEntity>>(cacheKey, cancellationToken);
            if (cachedData != null)
                return cachedData;

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

            var entities = dtos.Select(dto => new PaymentEntity
            {
                Login = dto.Login,
                CodCli = dto.CodCli?.Trim() ?? string.Empty,
                Tipo = dto.Tipo?.Trim() ?? string.Empty,
                Numero = dto.Numero?.Trim() ?? string.Empty,
                Pago = dto.Pago,
                Referencia = dto.Referencia?.Trim() ?? string.Empty,
                Fecha = dto.Fecha
            }).ToList();

            await _cacheService.SetAsync(cacheKey, entities, CacheExpiration, cancellationToken);

            return entities;
        }
    }
}
