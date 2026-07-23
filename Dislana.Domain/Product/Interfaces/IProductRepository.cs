using Dislana.Domain.Product.Entities;

namespace Dislana.Domain.Product.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<FilterEntity>> GetFiltersByTipoAsync(string type, CancellationToken cancellationToken);
        Task<IEnumerable<ProductListEntity>> GetProductsByTypeAsync(string type, int userId, CancellationToken cancellationToken);
        Task<ProductDetailEntity?> GetProductDetailByItemCodeAsync(string itemCode, int userId, CancellationToken cancellationToken);
        Task<IEnumerable<FeatureEntity>> GetFeaturesByItemCodeAsync(string itemCode, CancellationToken cancellationToken);
        Task<IEnumerable<SimilarProductEntity>> GetSimilarProductsByItemCodeAsync(string itemCode, int userId, CancellationToken cancellationToken);
    }
}
