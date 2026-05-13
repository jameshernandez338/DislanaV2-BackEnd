namespace Dislana.Application.ChatAssistant.Response
{
    public record GeneratePdfReportResponse
    {
        public bool IsSuccess { get; init; }
        public string? Message { get; init; }
        public byte[]? PdfBytes { get; init; }
        public string? FileName { get; init; }

        public static GeneratePdfReportResponse Success(byte[] pdfBytes, string fileName)
            => new()
            {
                IsSuccess = true,
                PdfBytes = pdfBytes,
                FileName = fileName,
                Message = "PDF generado exitosamente"
            };

        public static GeneratePdfReportResponse Fail(string message)
            => new()
            {
                IsSuccess = false,
                Message = message
            };
    }
}
