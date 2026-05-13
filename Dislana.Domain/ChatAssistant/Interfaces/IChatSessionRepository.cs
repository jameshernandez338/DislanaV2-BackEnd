using Dislana.Domain.ChatAssistant.Entities;

namespace Dislana.Domain.ChatAssistant.Interfaces
{
    public interface IChatSessionRepository
    {
        ChatSessionEntity? GetSession(string sessionId);
        void SaveSession(ChatSessionEntity session);
        void ClearOldSessions(TimeSpan maxAge);
    }
}
