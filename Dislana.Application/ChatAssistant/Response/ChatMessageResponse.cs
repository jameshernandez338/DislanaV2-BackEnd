namespace Dislana.Application.ChatAssistant.Response
{
    public record ChatMessageResponse(
        string Message,
        bool OfferPdf,
        string? PdfType,
        string Type
    );
}
