namespace Dislana.Domain.Quote.Entities
{
    public class QuoteDetailEntity
    {
        public string Codigo { get; private set; }
        public decimal Separados { get; private set; }
        public string Calidad { get; private set; }
        public string Imagen { get; private set; }
        public decimal Cantidad { get; private set; }
        public decimal PrecioTotal { get; private set; }

        private QuoteDetailEntity(
            string codigo,
            decimal separados,
            string calidad,
            string imagen,
            decimal cantidad,
            decimal precioTotal)
        {
            Codigo = codigo;
            Separados = separados;
            Calidad = calidad;
            Imagen = imagen;
            Cantidad = cantidad;
            PrecioTotal = precioTotal;
        }

        public static QuoteDetailEntity Create(
            string codigo,
            decimal separados,
            string calidad,
            string imagen,
            decimal cantidad,
            decimal precioTotal)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("Codigo cannot be empty.", nameof(codigo));

            return new QuoteDetailEntity(
                codigo,
                separados,
                calidad,
                imagen,
                cantidad,
                precioTotal);
        }
    }
}
