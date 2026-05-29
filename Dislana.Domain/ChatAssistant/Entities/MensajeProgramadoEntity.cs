namespace Dislana.Domain.ChatAssistant.Entities
{
    public class MensajeProgramadoEntity
    {
        public DateTime FechaInicial { get; set; }
        public DateTime FechaFinal { get; set; }
        public string Mensaje { get; set; } = string.Empty;

        public bool EsActivo()
        {
            var now = DateTime.Now.Date;
            return now >= FechaInicial.Date && now <= FechaFinal.Date;
        }
    }
}
