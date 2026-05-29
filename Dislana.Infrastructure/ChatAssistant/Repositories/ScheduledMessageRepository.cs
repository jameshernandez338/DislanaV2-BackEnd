using Dislana.Application.Common.Interfaces;
using Dislana.Domain.ChatAssistant.Entities;
using Dislana.Domain.ChatAssistant.Interfaces;
using Dislana.Domain.Common.Enums;
using Dislana.Infrastructure.ChatAssistant.DTOs;
using Dislana.Infrastructure.Persistence.Dapper;

namespace Dislana.Infrastructure.ChatAssistant.Repositories
{
    public class ScheduledMessageRepository : IScheduledMessageRepository
    {
        private readonly IContextualDbExecutor _dbExecutor;
        private readonly ICacheService _cacheService;
        private const DatabaseContext Context = DatabaseContext.ChatBot;
        private const string CacheKeyPrefix = "scheduled_messages";
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(30);

        public ScheduledMessageRepository(IContextualDbExecutor dbExecutor, ICacheService cacheService)
        {
            _dbExecutor = dbExecutor;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<ScheduledMessageEntity>> GetActiveMessagesAsync(CancellationToken cancellationToken)
        {
            var cacheKey = $"{CacheKeyPrefix}:active";

            var cachedData = await _cacheService.GetAsync<List<ScheduledMessageEntity>>(cacheKey, cancellationToken);
            if (cachedData != null)
                return cachedData;

            var query = @"
                SELECT 
                    FechaInicial,
                    FechaFinal,
                    Mensaje
                FROM dbo.mensajes
                WHERE CAST(GETDATE() AS DATE) >= CAST(FechaInicial AS DATE) 
                  AND CAST(GETDATE() AS DATE) <= CAST(FechaFinal AS DATE)";

            var dtos = await _dbExecutor.QueryAsync<ScheduledMessageDto>(Context, query, null, null, cancellationToken);

            var entities = dtos.Select(dto => new ScheduledMessageEntity
            {
                StartDate = dto.FechaInicial,
                EndDate = dto.FechaFinal,
                Message = dto.Mensaje
            }).ToList();

            await _cacheService.SetAsync(cacheKey, entities, CacheExpiration, cancellationToken);

            return entities;
        }
    }
}
