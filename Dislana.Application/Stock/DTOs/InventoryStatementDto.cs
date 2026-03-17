using System;

namespace Dislana.Application.Stock.DTOs
{
    public record InventoryStatementDto(
        string Grupo,
        string Documento,
        string Fecha,
        string Item,
        string Descripcion,
        decimal Cantidad,
        decimal SaldoPendiente,
        string CalidadLote,
        decimal PrecioTotal,
        string Imagen
    );
}
