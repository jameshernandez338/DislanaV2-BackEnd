namespace Dislana.Domain.ChatAssistant.Entities
{
    public class ScheduledMessageEntity
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Message { get; set; } = string.Empty;

        public bool IsActive()
        {
            var now = DateTime.Now.Date;
            return now >= StartDate.Date && now <= EndDate.Date;
        }
    }
}
