namespace Dislana.Application.Payment.DTOs
{
    public record PaymentItemDto(
        string Tipo,
        string Documento,
        string Item,
        decimal Cantidad,
        decimal Valor
    );
}
