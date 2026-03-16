namespace Dislana.Domain.Product.Entities
{
    public class ProductDetailEntity
    {
        public string CodigoItem { get; private set; } = default!;
        public string Imagen { get; private set; } = default!;
        public string Detalle { get; private set; } = default!;
        public double PvpDescuento { get; private set; }
        public double Pvp { get; private set; }
        public double PvpB { get; private set; }
        public double Calidad1 { get; private set; } = default!;
        public double CalidadB { get; private set; } = default!;
        public double PesoML { get; private set; } = default!;
        public double PesoGSM { get; private set; } = default!;
        public double Ancho { get; private set; } = default!;
        public string Descuento { get; private set; }
        public decimal Cantidad1 { get; private set; }
        public decimal CantidadB { get; private set; }
        public string FichaTecnica { get; private set; } = default!;

        private ProductDetailEntity() { }

        public ProductDetailEntity(
            string codigoItem,
            string imagen,
            string detalle,
            double pvpDescuento,
            double pvp,
            double pvpB,
            double calidad1,
            double calidadB,
            double pesoML,
            double pesoGSM,
            double ancho,
            string descuento,
            decimal cantidad1,
            decimal cantidadB,
            string fichaTecnica)
        {
            CodigoItem = codigoItem;
            Imagen = imagen;
            Detalle = detalle;
            PvpDescuento = pvpDescuento;
            Pvp = pvp;
            PvpB = pvpB;
            Calidad1 = calidad1;
            CalidadB = calidadB;
            PesoML = pesoML;
            PesoGSM = pesoGSM;
            Ancho = ancho;
            Descuento = descuento;
            Cantidad1 = cantidad1;
            CantidadB = cantidadB;
            FichaTecnica = fichaTecnica;
        }
    }
}
