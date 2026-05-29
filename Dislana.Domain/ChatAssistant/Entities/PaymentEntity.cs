namespace Dislana.Domain.ChatAssistant.Entities
{
    public class PaymentEntity
    {
        public int Login { get; set; }
        public string CodCli { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public decimal Pago { get; set; }
        public string Referencia { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
    }
}
