namespace Dislana.Application.Quote.DTOs
{
    public record QuoteDto(
        string Grupo,
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
        decimal PrecioTotal,
        decimal PrecioAnticipo,
        string  Estado
    );
}
