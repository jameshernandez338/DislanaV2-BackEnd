using Dislana.Domain.ChatAssistant.ValueObjects;

namespace Dislana.Domain.ChatAssistant.Entities
{
    /// <summary>
    /// Entidad de Dominio: Representa un reporte PDF de facturas
    /// Rich Entity con comportamiento y validaciones
    /// </summary>
    public sealed class InvoiceReportEntity
    {
        public ReportType Type { get; private set; }
        public string CustomerName { get; private set; }
        public DateTime GeneratedAt { get; private set; }
        private readonly List<ChatInvoiceEntity> _invoices;

        public IReadOnlyList<ChatInvoiceEntity> Invoices => _invoices.AsReadOnly();

        private InvoiceReportEntity(
            ReportType type,
            string customerName,
            List<ChatInvoiceEntity> invoices,
            DateTime generatedAt)
        {
            Type = type;
            CustomerName = customerName;
            _invoices = invoices;
            GeneratedAt = generatedAt;
        }

        /// <summary>
        /// Factory Method: Crea un reporte de facturas
        /// </summary>
        public static InvoiceReportEntity Create(
            string reportType,
            string customerName,
            IEnumerable<ChatInvoiceEntity> invoices)
        {
            var typeVO = ReportType.Create(reportType);

            if (string.IsNullOrWhiteSpace(customerName))
                throw new Exceptions.DomainException("El nombre del cliente es requerido");

            var invoiceList = invoices?.ToList() ?? new List<ChatInvoiceEntity>();

            if (invoiceList.Count == 0)
                throw new Exceptions.DomainException("No hay facturas para generar el reporte");

            return new InvoiceReportEntity(
                typeVO,
                customerName.Trim(),
                invoiceList,
                DateTime.UtcNow
            );
        }

        /// <summary>
        /// Calcula el total de ventas del reporte
        /// </summary>
        public decimal CalculateTotalSales()
        {
            return _invoices.Sum(invoice => invoice.Valor);
        }

        /// <summary>
        /// Calcula el total de saldo pendiente
        /// </summary>
        public decimal CalculateTotalBalance()
        {
            return _invoices.Sum(invoice => invoice.Saldo);
        }

        /// <summary>
        /// Obtiene el número total de facturas
        /// </summary>
        public int GetInvoiceCount()
        {
            return _invoices.Count;
        }

        /// <summary>
        /// Genera el nombre del archivo PDF
        /// </summary>
        public string GenerateFileName()
        {
            var timestamp = GeneratedAt.ToString("yyyyMMddHHmmss");
            return $"reporte_{Type.Value}_{timestamp}.pdf";
        }

        /// <summary>
        /// Valida si el reporte puede ser generado
        /// </summary>
        public void ValidateForGeneration()
        {
            if (_invoices.Count == 0)
                throw new Exceptions.DomainException("No hay facturas disponibles para generar el reporte");

            if (string.IsNullOrWhiteSpace(CustomerName))
                throw new Exceptions.DomainException("El nombre del cliente no está disponible");
        }
    }
}
