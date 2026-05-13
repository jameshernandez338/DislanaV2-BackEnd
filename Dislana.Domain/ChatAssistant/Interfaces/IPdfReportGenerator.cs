using Dislana.Domain.ChatAssistant.Entities;

namespace Dislana.Domain.ChatAssistant.Interfaces
{
    public interface IPdfReportGenerator
    {
        Task<byte[]> GeneratePdfAsync(InvoiceReportEntity report, CancellationToken cancellationToken);
    }
}
