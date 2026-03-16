using Dislana.Domain.Product.Entities;

namespace Dislana.Domain.Product.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<FilterEntity>> GetFiltersByTipoAsync(string type, CancellationToken cancellationToken);
        Task<IEnumerable<ProductListEntity>> GetProductsByTypeAsync(string type, CancellationToken cancellationToken);
        Task<ProductDetailEntity?> GetProductDetailByItemCodeAsync(string itemCode, CancellationToken cancellationToken);
        Task<IEnumerable<FeatureEntity>> GetFeaturesByItemCodeAsync(string itemCode, CancellationToken cancellationToken);
        Task<IEnumerable<SimilarProductEntity>> GetSimilarProductsByItemCodeAsync(string itemCode, CancellationToken cancellationToken);
    }
}
