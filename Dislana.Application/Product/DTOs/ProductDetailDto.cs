namespace Dislana.Application.Product.DTOs
{
    public record ProductDetailDto(
        ProductDto Product,
        IReadOnlyList<FeatureDto> Features,
        IReadOnlyList<SimilarProductDto> SimilarProducts
    );
}
