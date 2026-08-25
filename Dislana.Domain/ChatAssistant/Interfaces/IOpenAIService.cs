using Dislana.Domain.ChatAssistant.Entities;

namespace Dislana.Domain.ChatAssistant.Interfaces
{
    public interface IOpenAIService
    {
        Task<string> SendMessageAsync(ChatSessionEntity session, string systemPrompt, CancellationToken cancellationToken);
        Task<string> SendAsync(string prompt, CancellationToken cancellationToken);
    }
}
