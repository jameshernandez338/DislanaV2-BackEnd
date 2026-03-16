namespace Dislana.Application.Product.DTOs
{
    public record SimilarProductDto(
        string CodigoItem,
        string Imagen,
        string DescripcionItem,
        decimal Disponible,
        decimal Pvp,
        string Detalle
    );
}
