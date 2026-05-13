namespace Dislana.Domain.ChatAssistant.ValueObjects
{
    /// <summary>
    /// Value Object: Representa el tipo de reporte PDF
    /// </summary>
    public sealed record ReportType
    {
        public string Value { get; }

        // Tipos válidos
        public const string Saldo = "saldo";
        public const string Ventas = "ventas";
        public const string Completo = "completo";

        private ReportType(string value)
        {
            Value = value;
        }

        public static ReportType Create(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exceptions.DomainException("El tipo de reporte es requerido");

            var normalized = value.Trim().ToLowerInvariant();

            if (normalized != Saldo && normalized != Ventas && normalized != Completo)
                throw new Exceptions.DomainException(
                    $"Tipo de reporte inválido. Debe ser: {Saldo}, {Ventas} o {Completo}");

            return new ReportType(normalized);
        }

        public string GetTitle()
        {
            return Value switch
            {
                Saldo => "ESTADO DE SALDO PENDIENTE",
                Ventas => "RESUMEN DE VENTAS",
                Completo => "INFORME COMPLETO DE CUENTA",
                _ => "INFORME DE CUENTA"
            };
        }

        public static implicit operator string(ReportType type) => type.Value;

        public override string ToString() => Value;
    }
}
