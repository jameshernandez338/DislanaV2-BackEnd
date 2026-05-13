using System.ComponentModel.DataAnnotations;

namespace Dislana.Application.ChatAssistant.Request
{
    public record GeneratePdfReportRequest
    {
        /// <summary>
        /// Tipo de reporte: "saldo", "ventas", "completo"
        /// </summary>
        [Required(ErrorMessage = "El tipo de reporte es obligatorio")]
        [RegularExpression("^(saldo|ventas|completo)$", 
            ErrorMessage = "El tipo debe ser: saldo, ventas o completo")]
        public string Tipo { get; init; } = default!;

        /// <summary>
        /// SessionId del chat (opcional, para contexto)
        /// </summary>
        public string? SessionId { get; init; }
    }
}
