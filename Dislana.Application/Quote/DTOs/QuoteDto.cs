namespace Dislana.Application.Quote.DTOs
{
    public record QuoteDto(
        string Documento,
        string Imagen,
        string Codigo,
        string Acabado,
        string Descripcion,
        string Calidad,
        string Linea,
        decimal Saldo,
        decimal Separados,
        decimal Cantidad,
        decimal PrecioTotal
    );
}
