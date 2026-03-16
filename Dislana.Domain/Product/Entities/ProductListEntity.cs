namespace Dislana.Domain.Product.Entities
{
    public class ProductListEntity
    {
        public string CodigoItem { get; private set; }
        public string DescripcionItem { get; private set; } = default!;
        public string Imagen { get; private set; } = default!;
        public decimal Pvp { get; private set; }
        public decimal Disponible { get; private set; }
        public string Departamento { get; private set; }
        public string Ciudad { get; private set; }
        public string Nombre { get; private set; }
        public string Cts { get; private set; }
        public string Tipo { get; private set; }
        public string Acabado { get; private set; }
        public string Categoria { get; private set; }
        public string Atributo { get; private set; }
        public string Color { get; private set; }
        public string Detalle { get; private set; } = default!;
        public decimal Separado { get; private set; }
        public decimal Pendiente { get; private set; }

        private ProductListEntity() { }

        public ProductListEntity(
            string codigoItem,
            string descripcionItem,
            string imagen,
            decimal pvp,
            decimal disponible,
            string departamento,
            string ciudad,
            string nombre,
            string cts,
            string tipo,
            string acabado,
            string categoria,
            string atributo,
            string color,
            string detalle,
            decimal separado,
            decimal pendiente)
        {
            CodigoItem = codigoItem;
            DescripcionItem = descripcionItem;
            Imagen = imagen;
            Pvp = pvp;
            Disponible = disponible;
            Departamento = departamento;
            Ciudad = ciudad;
            Nombre = nombre;
            Cts = cts;
            Tipo = tipo;
            Acabado = acabado;
            Categoria = categoria;
            Atributo = atributo;
            Color = color;
            Detalle = detalle;
            Separado = separado;
            Pendiente = pendiente;
        }
    }
}
