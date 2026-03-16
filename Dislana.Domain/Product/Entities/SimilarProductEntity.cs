namespace Dislana.Domain.Product.Entities
{
    public class SimilarProductEntity
    {
        public string CodigoItem { get; private set; } = default!;
        public string Imagen { get; private set; } = default!;
        public string DescripcionItem { get; private set; } = default!;
        public decimal Disponible { get; private set; }
        public decimal Pvp { get; private set; }
        public string Detalle { get; private set; } = default!;

        private SimilarProductEntity() { }

        public SimilarProductEntity(string codigoItem, string imagen, string descripcionItem, decimal disponible, decimal pvp, string detalle)
        {
            CodigoItem = codigoItem;
            Imagen = imagen;
            DescripcionItem = descripcionItem;
            Disponible = disponible;
            Pvp = pvp;
            Detalle = detalle;
        }
    }
}
