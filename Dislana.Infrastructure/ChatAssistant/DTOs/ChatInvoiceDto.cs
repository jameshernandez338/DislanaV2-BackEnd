namespace Dislana.Infrastructure.ChatAssistant.DTOs
{
    public class ChatInvoiceDto
    {
        public string TypeDocument { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public string CustomerDni { get; set; } = string.Empty;
        public string Customer { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public decimal Saldo { get; set; }
        public string LinkInvoice { get; set; } = string.Empty;
        public string Guia { get; set; } = string.Empty;
        public string LinkGuia { get; set; } = string.Empty;
        public int Enviado { get; set; }
        public int EnviadoGuia { get; set; }
    }
}
