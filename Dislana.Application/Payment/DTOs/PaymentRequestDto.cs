namespace Dislana.Application.Payment.DTOs
{
    public record PaymentRequestDto(
        decimal ValorTotal,
        IReadOnlyList<PaymentItemDto> Items
    );
}
