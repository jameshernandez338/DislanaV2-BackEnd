namespace Dislana.Infrastructure.ChatAssistant.DTOs
{
    public class ScheduledMessageDto
    {
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFinal { get; set; }
        public string Mensaje { get; set; } = string.Empty;
    }
}
