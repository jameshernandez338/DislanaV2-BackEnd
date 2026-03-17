namespace Dislana.Domain.Quote.Entities
{
    public class QuoteEntity
    {
        public string Documento { get; set; }
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

        private QuoteEntity() { }

        public QuoteEntity(string documento, string imagen, string codigo, string acabado, string descripcion, string calidad, string linea, decimal saldo, decimal separados, decimal cantidad, decimal precioTotal )
        {
            Documento = documento;
            Imagen = imagen;
            Codigo = codigo;
            Acabado = acabado;
            Descripcion = descripcion;
            Calidad = calidad;
            Linea = linea;
            Saldo = saldo;
            Separados = separados;
            Cantidad = cantidad;
            PrecioTotal = precioTotal;
        }
    }
}
