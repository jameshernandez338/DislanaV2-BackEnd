using Dislana.Domain.ChatAssistant.Entities;
using Dislana.Domain.ChatAssistant.Interfaces;
using System.Collections.Concurrent;

namespace Dislana.Infrastructure.ChatAssistant.Repositories
{
    public class InMemoryChatSessionRepository : IChatSessionRepository
    {
        private static readonly ConcurrentDictionary<string, ChatSessionEntity> _sessions = new();

        public ChatSessionEntity? GetSession(string sessionId)
        {
            return _sessions.TryGetValue(sessionId, out var session) ? session : null;
        }

        public void SaveSession(ChatSessionEntity session)
        {
            _sessions[session.SessionId] = session;
        }

        public void ClearOldSessions(TimeSpan maxAge)
        {
            // En una implementación real, necesitarías timestamps
            // Por ahora, limitar por cantidad
            if (_sessions.Count > 1000)
            {
                var toRemove = _sessions.Keys.Take(_sessions.Count - 1000).ToList();
                foreach (var key in toRemove)
                {
                    _sessions.TryRemove(key, out _);
                }
            }
        }
    }
}

