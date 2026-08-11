namespace Dislana.Application.Stock.DTOs
{
    public record InventoryStatementDetailDto(
        string Codigo,
        string Documento,
        string Calidad,
        decimal Separados,
        decimal Cantidad,
        decimal Valor
    );
}
