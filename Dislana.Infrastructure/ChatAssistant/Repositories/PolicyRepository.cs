using Dislana.Application.Common.Interfaces;
using Dislana.Domain.ChatAssistant.Entities;
using Dislana.Domain.ChatAssistant.Interfaces;
using Dislana.Domain.Common.Enums;
using Dislana.Infrastructure.ChatAssistant.DTOs;
using Dislana.Infrastructure.Persistence.Dapper;

namespace Dislana.Infrastructure.ChatAssistant.Repositories
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly IContextualDbExecutor _dbExecutor;
        private readonly ICacheService _cacheService;
        private const DatabaseContext Context = DatabaseContext.ChatBot;
        private const string CacheKeyPrefix = "chat_policies";
        private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(24);

        public PolicyRepository(IContextualDbExecutor dbExecutor, ICacheService cacheService)
        {
            _dbExecutor = dbExecutor;
            _cacheService = cacheService;
        }

        public async Task<PolicyEntity?> GetPolicyContentAsync(CancellationToken cancellationToken)
        {
            var cacheKey = $"{CacheKeyPrefix}:content";

            var cachedData = await _cacheService.GetAsync<PolicyEntity>(cacheKey, cancellationToken);
            if (cachedData != null)
                return cachedData;

            var query = @"
                SELECT 
                    contenido AS Contenido
                FROM POLITICAS";

            var dtos = await _dbExecutor.QueryAsync<PolicyDto>(Context, query, null, null, cancellationToken);
            var dto = dtos.FirstOrDefault();

            if (dto == null)
                return null;

            var entity = new PolicyEntity
            {
                Contenido = dto.Contenido?.Trim() ?? string.Empty
            };

            await _cacheService.SetAsync(cacheKey, entity, CacheExpiration, cancellationToken);

            return entity;
        }
    }
}
