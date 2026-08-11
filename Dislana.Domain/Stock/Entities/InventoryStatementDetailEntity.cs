namespace Dislana.Domain.Stock.Entities
{
    public class InventoryStatementDetailEntity
    {
        public string Codigo { get; private set; } = default!;
        public string Documento { get; private set; } = default!;
        public string Calidad { get; private set; } = default!;
        public decimal Separados { get; private set; }
        public decimal Cantidad { get; private set; }
        public decimal PrecioTotal { get; private set; }
    }
}
