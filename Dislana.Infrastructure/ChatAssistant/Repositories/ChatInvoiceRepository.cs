using Dislana.Application.Common.Interfaces;
using Dislana.Domain.ChatAssistant.Entities;
using Dislana.Domain.ChatAssistant.Interfaces;
using Dislana.Domain.Common.Enums;
using Dislana.Infrastructure.ChatAssistant.DTOs;
using Dislana.Infrastructure.Persistence.Dapper;

namespace Dislana.Infrastructure.ChatAssistant.Repositories
{
    public class ChatInvoiceRepository : IChatInvoiceRepository
    {
        private readonly IContextualDbExecutor _dbExecutor;
        private readonly ICacheService _cacheService;
        private const DatabaseContext Context = DatabaseContext.ChatBot;
        private const string CacheKeyPrefix = "chat_invoices";
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(1);

        public ChatInvoiceRepository(IContextualDbExecutor dbExecutor, ICacheService cacheService)
        {
            _dbExecutor = dbExecutor;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<ChatInvoiceEntity>> GetChatInvoiceByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            var cacheKey = $"{CacheKeyPrefix}:user:{userId}";

            var cachedData = await _cacheService.GetAsync<List<ChatInvoiceEntity>>(cacheKey, cancellationToken);
            if (cachedData != null)
                return cachedData;

            var query = @"
                SELECT 
                    LTRIM(RTRIM(TypeDocument)) AS TypeDocument,
                    LTRIM(RTRIM(Number)) AS Number,
                    CONVERT(varchar, fecha, 103) AS Fecha,
                    LTRIM(RTRIM(CustomerDni)) AS CustomerDni,
                    LTRIM(RTRIM(Customer)) AS Customer,
                    valor AS Valor,
                    saldo AS Saldo,
                    linkInvoice AS LinkInvoice,
                    LTRIM(RTRIM(guia)) AS Guia,
                    linkguia AS LinkGuia,
                    enviado AS Enviado,
                    enviadoguia AS EnviadoGuia
                FROM dbo.contactos
                WHERE login = @userId
                ORDER BY fecha DESC";

            var parameters = new { userId };
            var dtos = await _dbExecutor.QueryAsync<ChatInvoiceDto>(Context, query, parameters, null, cancellationToken);

            var entities = dtos.Select(dto => new ChatInvoiceEntity
            {
                TypeDocument = dto.TypeDocument,
                Number = dto.Number,
                Fecha = dto.Fecha,
                CustomerDni = dto.CustomerDni,
                Customer = dto.Customer,
                Valor = dto.Valor,
                Saldo = dto.Saldo,
                LinkInvoice = dto.LinkInvoice,
                Guia = dto.Guia,
                LinkGuia = dto.LinkGuia,
                Enviado = dto.Enviado,
                EnviadoGuia = dto.EnviadoGuia
            }).ToList();

            await _cacheService.SetAsync(cacheKey, entities, CacheExpiration, cancellationToken);

            return entities;
        }
    }
}

