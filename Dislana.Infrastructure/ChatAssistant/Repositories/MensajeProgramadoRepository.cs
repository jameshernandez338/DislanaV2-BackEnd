using Dislana.Domain.ChatAssistant.Entities;
using Dislana.Domain.ChatAssistant.Interfaces;
using Dislana.Domain.Common.Enums;
using Dislana.Infrastructure.ChatAssistant.DTOs;
using Dislana.Infrastructure.Persistence.Dapper;

namespace Dislana.Infrastructure.ChatAssistant.Repositories
{
    public class MensajeProgramadoRepository : IMensajeProgramadoRepository
    {
        private readonly IContextualDbExecutor _dbExecutor;
        private const DatabaseContext Context = DatabaseContext.ChatBot;

        public MensajeProgramadoRepository(IContextualDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<IEnumerable<MensajeProgramadoEntity>> GetMensajesActivosAsync(CancellationToken cancellationToken)
        {
            var query = @"
                SELECT 
                    FechaInicial,
                    FechaFinal,
                    Mensaje
                FROM dbo.mensajes
                WHERE CAST(GETDATE() AS DATE) >= CAST(FechaInicial AS DATE) 
                  AND CAST(GETDATE() AS DATE) <= CAST(FechaFinal AS DATE)";

            var dtos = await _dbExecutor.QueryAsync<MensajeProgramadoDto>(Context, query, null, null, cancellationToken);

            return dtos.Select(dto => new MensajeProgramadoEntity
            {
                FechaInicial = dto.FechaInicial,
                FechaFinal = dto.FechaFinal,
                Mensaje = dto.Mensaje
            });
        }
    }
}
