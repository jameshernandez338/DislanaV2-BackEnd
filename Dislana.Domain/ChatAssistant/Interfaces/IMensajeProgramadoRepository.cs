using Dislana.Domain.ChatAssistant.Entities;

namespace Dislana.Domain.ChatAssistant.Interfaces
{
    public interface IScheduledMessageRepository
    {
        Task<IEnumerable<ScheduledMessageEntity>> GetActiveMessagesAsync(CancellationToken cancellationToken);
    }
}
