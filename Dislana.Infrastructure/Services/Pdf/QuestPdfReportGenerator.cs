using Dislana.Domain.ChatAssistant.Entities;
using Dislana.Domain.ChatAssistant.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;

namespace Dislana.Infrastructure.Services.Pdf
{
    /// <summary>
    /// Infrastructure Service: Implementa generación de PDF usando QuestPDF
    /// Implementa la interface del Domain (Inversión de dependencias)
    /// </summary>
    public class QuestPdfReportGenerator : IPdfReportGenerator
    {
        private static readonly CultureInfo CultureCO = new("es-CO");

        public Task<byte[]> GeneratePdfAsync(InvoiceReportEntity report, CancellationToken cancellationToken)
        {
            // Configurar licencia de QuestPDF (Community License para uso no comercial)
            QuestPDF.Settings.License = LicenseType.Community;

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

                    page.Header().Element(c => ComposeHeader(c, report));
                    page.Content().Element(c => ComposeContent(c, report));
                    page.Footer().Element(ComposeFooter);
                });
            })
            .GeneratePdf();

            return Task.FromResult(pdfBytes);
        }

        private void ComposeHeader(IContainer container, InvoiceReportEntity report)
        {
            container.Column(column =>
            {
                // Logo y título
                column.Item().Text("TEXTILES DISLANA")
                    .FontSize(20)
                    .Bold()
                    .FontColor("#1a1a2e");

                column.Item().Text("Calle 9 #41a-16 · Bogotá · textilesdislana.com")
                    .FontSize(10)
                    .FontColor("#555");

                column.Item().PaddingVertical(5).LineHorizontal(1).LineColor("#cccccc");

                // Título del reporte
                column.Item().PaddingTop(10).Text(report.Type.GetTitle())
                    .FontSize(14)
                    .Bold()
                    .FontColor("#1a1a2e");

                // Información del reporte
                var fecha = report.GeneratedAt.ToLocalTime().ToString("dd 'de' MMMM 'de' yyyy", CultureCO);
                column.Item().Text($"Cliente: {report.CustomerName}   |   Fecha: {fecha}")
                    .FontSize(10)
                    .FontColor("#555");

                // Resumen en caja
                column.Item().PaddingTop(15).Background("#f0f4ff").Padding(10).Column(summary =>
                {
                    var totalSales = report.CalculateTotalSales();
                    var totalBalance = report.CalculateTotalBalance();
                    var invoiceCount = report.GetInvoiceCount();

                    summary.Item().Text($"Total ventas:           ${totalSales.ToString("N0", CultureCO)}")
                        .FontSize(11)
                        .Bold()
                        .FontColor("#1a1a2e");

                    summary.Item().Text($"Total saldo pendiente:  ${totalBalance.ToString("N0", CultureCO)}")
                        .FontSize(11)
                        .Bold()
                        .FontColor("#1a1a2e");

                    summary.Item().Text($"Número de facturas: {invoiceCount}")
                        .FontSize(10)
                        .FontColor("#555");
                });

                column.Item().PaddingTop(20).Text("DETALLE DE FACTURAS")
                    .FontSize(11)
                    .Bold()
                    .FontColor("#1a1a2e");
            });
        }

        private void ComposeContent(IContainer container, InvoiceReportEntity report)
        {
            container.PaddingTop(10).Column(column =>
            {
                // Header de la tabla
                column.Item().Background("#1a1a2e").Padding(5).Row(row =>
                {
                    row.RelativeItem(2).Text("Nro. Factura").FontSize(9).Bold().FontColor(Colors.White);
                    row.RelativeItem(2).Text("Fecha").FontSize(9).Bold().FontColor(Colors.White);
                    row.RelativeItem(2).Text("Valor").FontSize(9).Bold().FontColor(Colors.White);
                    row.RelativeItem(2).Text("Saldo").FontSize(9).Bold().FontColor(Colors.White);
                    row.RelativeItem(1).Text("Guía").FontSize(9).Bold().FontColor(Colors.White);
                });

                // Filas de la tabla
                var invoices = report.Invoices.ToList();
                for (int i = 0; i < invoices.Count; i++)
                {
                    var invoice = invoices[i];
                    var bgColor = i % 2 == 0 ? "#f9f9f9" : "#ffffff";

                    column.Item().Background(bgColor).Padding(4).Row(row =>
                    {
                        row.RelativeItem(2).Text(invoice.Number ?? "-").FontSize(8).FontColor("#222");
                        row.RelativeItem(2).Text(invoice.Fecha ?? "-").FontSize(8).FontColor("#222");
                        row.RelativeItem(2).Text($"${invoice.Valor.ToString("N0", CultureCO)}").FontSize(8).FontColor("#222");
                        row.RelativeItem(2).Text($"${invoice.Saldo.ToString("N0", CultureCO)}").FontSize(8).FontColor("#222");
                        row.RelativeItem(1).Text(invoice.Guia ?? "-").FontSize(8).FontColor("#222");
                    });
                }

                // Totales
                column.Item().PaddingTop(10).LineHorizontal(1).LineColor("#1a1a2e");

                column.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem(4).Text(""); // Espaciado
                    row.RelativeItem(2).Column(col =>
                    {
                        col.Item().Text($"TOTAL VENTAS: ${report.CalculateTotalSales().ToString("N0", CultureCO)}")
                            .FontSize(10)
                            .Bold()
                            .FontColor("#1a1a2e");

                        col.Item().Text($"TOTAL SALDO:  ${report.CalculateTotalBalance().ToString("N0", CultureCO)}")
                            .FontSize(10)
                            .Bold()
                            .FontColor("#1a1a2e");
                    });
                    row.RelativeItem(1).Text(""); // Espaciado
                });
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Column(column =>
            {
                column.Item().PaddingTop(20).Text("Generado automáticamente por el asistente virtual de Textiles Dislana")
                    .FontSize(8)
                    .FontColor("#999");

                column.Item().Text("Lunes a viernes 5am–4pm · Calle 9 #41a-16 Bogotá · textilesdislana.com")
                    .FontSize(8)
                    .FontColor("#999");
            });
        }
    }
}
