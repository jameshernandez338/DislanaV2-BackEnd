namespace Dislana.Domain.Product.Entities
{
    public class FeatureEntity
    {
        public string Titulo { get; private set; } = default!;
        public string Detalle { get; private set; } = default!;

        private FeatureEntity() { }

        public FeatureEntity(string titulo, string detalle)
        {
            Titulo = titulo;
            Detalle = detalle;
        }
    }
}
