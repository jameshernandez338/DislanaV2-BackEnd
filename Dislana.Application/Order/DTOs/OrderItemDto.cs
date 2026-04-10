namespace Dislana.Application.Order.DTOs
{
    public record OrderItemDto(
        string CodigoItem,
        decimal Cantidad1,
        decimal CantidadB,
        decimal Pvp,
        decimal PvpB,
        IReadOnlyList<FabricFinishDto> Acabados
    );
}
