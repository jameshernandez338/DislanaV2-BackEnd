using Dislana.Application.Product.DTOs;

namespace Dislana.Application.Product.Interfaces
{
    public interface IProductService
    {
        Task<IReadOnlyList<FilterItemDto>> GetFiltersAsync(string type, CancellationToken cancellationToken);
        Task<IReadOnlyList<ProductListDto>> GetProductsAsync(string type, CancellationToken cancellationToken);
        Task<ProductDetailDto?> GetProductDetailAsync(string itemCode, CancellationToken cancellationToken);
    }
}
