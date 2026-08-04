namespace Dislana.Domain.Quote.Entities
{
    public class QuoteEntity
    {
        public string Grupo { get; set; } = default!;
        public string Documento { get; set; } = default!;
        public string Imagen { get; private set; } = default!;
        public string Codigo { get; private set; } = default!;
        public string Acabado { get; private set; } = default!;
        public string Descripcion { get; private set; } = default!;
        public string Calidad { get; private set; } = default!;
        public string Linea { get; private set; } = default!;
        public decimal Saldo { get; private set; }
        public decimal Separados { get; private set; }
        public decimal Cantidad { get; private set; }
        public decimal PrecioTotal { get; private set; }
        public decimal PrecioAnticipo { get; private set; }
        public string Estado { get; private set; } = default!;
    }
}
