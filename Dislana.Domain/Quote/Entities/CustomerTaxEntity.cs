namespace Dislana.Domain.Quote.Entities
{
    public class CustomerTaxEntity
    {
        public decimal Descuento { get; private set; }
        public decimal Iva { get; private set; }
        public decimal ReteFuente { get; private set; }
        public decimal ReteIva { get; private set; }
        public decimal ReteIca { get; private set; }
        public decimal Cartera { get; private set; }
        public decimal Apin { get; private set; }
        public decimal SaldoAFavor { get; private set; }
        public decimal Cupo { get; private set; }
        public bool UsaCupo { get; private set; }
        public decimal BaseReteIca { get; private set; }
        public decimal BaseReteIva { get; private set; }

        private CustomerTaxEntity() { }
    }
}
