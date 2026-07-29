namespace Dislana.Application.Quote.DTOs
{
    public record QuoteDetailDto(
        string Codigo,
        decimal Separados,
        string Calidad,
        string Imagen,
        decimal Cantidad,
        decimal PrecioTotal
    );
}
