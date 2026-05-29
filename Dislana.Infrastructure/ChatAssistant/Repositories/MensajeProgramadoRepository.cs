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
        private const DatabaseContext Context = DatabaseContext.ChatBot;

        public ScheduledMessageRepository(IContextualDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<IEnumerable<ScheduledMessageEntity>> GetActiveMessagesAsync(CancellationToken cancellationToken)
        {
            var query = @"
                SELECT 
                    FechaInicial,
                    FechaFinal,
                    Mensaje
                FROM dbo.mensajes
                WHERE CAST(GETDATE() AS DATE) >= CAST(FechaInicial AS DATE) 
                  AND CAST(GETDATE() AS DATE) <= CAST(FechaFinal AS DATE)";

            var dtos = await _dbExecutor.QueryAsync<ScheduledMessageDto>(Context, query, null, null, cancellationToken);

            return dtos.Select(dto => new ScheduledMessageEntity
            {
                StartDate = dto.FechaInicial,
                EndDate = dto.FechaFinal,
                Message = dto.Mensaje
            });
        }
    }
}
