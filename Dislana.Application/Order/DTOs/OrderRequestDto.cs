namespace Dislana.Application.Order.DTOs
{
    public record OrderRequestDto(
        IReadOnlyList<OrderItemDto> Items,
        string Orillo
    );
}
