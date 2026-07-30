namespace Dislana.Domain.Stock.Entities
{
    public class InventoryStatementEntity
    {
        public string Grupo { get; private set; } = default!;
        public string Documento { get; private set; } = default!;
        public string Fecha { get; private set; }
        public string Item { get; private set; } = default!;
        public string Descripcion { get; private set; } = default!;
        public decimal Cantidad { get; private set; }
        public decimal SaldoPendiente { get; private set; }
        public string CalidadLote { get; private set; } = default!;
        public decimal PrecioTotal { get; private set; }
        public string Imagen { get; private set; } = default!;
        public string Estado { get; private set; } = default!;
    }
}
