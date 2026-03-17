namespace Dislana.Domain.Quote.Entities
{
    public class CustomerBalanceEntity
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

        private CustomerBalanceEntity() { }

        public CustomerBalanceEntity(decimal descuento, decimal iva, decimal reteFuente, decimal reteIva, decimal reteIca, decimal cartera, decimal apin, decimal saldoAFavor, decimal cupo, bool usaCupo)
        {
            Descuento = descuento;
            Iva = iva;
            ReteFuente = reteFuente;
            ReteIva = reteIva;
            ReteIca = reteIca;
            Cartera = cartera;
            Apin = apin;
            SaldoAFavor = saldoAFavor;
            Cupo = cupo;
            UsaCupo = usaCupo;
        }
    }
}
