namespace Dislana.Domain.ChatAssistant.Entities
{
    public class ChatSessionEntity
    {
        public string SessionId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public List<ChatMessageEntity> History { get; set; } = new();
        public bool WaitingForPdf { get; set; }
        public string PendingPdfType { get; set; } = string.Empty;
    }
}
