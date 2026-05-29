using Dislana.Domain.ChatAssistant.Entities;

namespace Dislana.Domain.ChatAssistant.Interfaces
{
    public interface IMensajeProgramadoRepository
    {
        Task<IEnumerable<MensajeProgramadoEntity>> GetMensajesActivosAsync(CancellationToken cancellationToken);
    }
}
