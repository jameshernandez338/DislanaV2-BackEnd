namespace Dislana.Application.Payment.DTOs
{
    public record WompiPaymentDto(
        string PublicKey,
        string Currency,
        long AmountInCents,
        string Reference,
        string Signature,
        string RedirectUrl,
        string UrlBase
    );
}
