using Dislana.Application.Common.Interfaces;
using Dislana.Application.Product.DTOs;
using Dislana.Application.Product.Interfaces;
using Dislana.Domain.Product.Interfaces;

namespace Dislana.Application.Product
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IUserContextService _userContextService;

        public ProductService(
           IProductRepository productRepository,
           IUserContextService userContextService)
        {
            _productRepository = productRepository;
            _userContextService = userContextService;
        }

        public async Task<IReadOnlyList<FilterItemDto>> GetFiltersAsync(string type, CancellationToken cancellationToken)
        {
            var items = await _productRepository.GetFiltersByTipoAsync(type, cancellationToken);
            return items.Select(i => new FilterItemDto(i.Filtro, i.Valor, i.CampoVista))
                        .ToList()
                        .AsReadOnly();
        }

        public async Task<IReadOnlyList<ProductListDto>> GetProductsAsync(string type, CancellationToken cancellationToken)
        {
            var userIdString = _userContextService.GetId();
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("No se pudo obtener el login del usuario.");
            }

            var items = await _productRepository.GetProductsByTypeAsync(type, userId, cancellationToken);
            return items.Select(i => new ProductListDto(
                    i.CodigoItem,
                    i.DescripcionItem,
                    i.Imagen,
                    i.Pvp,
                    i.Disponible,
                    i.Departamento,
                    i.Ciudad,
                    i.Nombre,
                    i.Cts,
                    i.Tipo,
                    i.Acabado,
                    i.Categoria,
                    i.Atributo,
                    i.Color,
                    i.Detalle,
                    i.Separado,
                    i.Pendiente))
                .ToList()
                .AsReadOnly();
        }

        public async Task<ProductDetailDto?> GetProductDetailAsync(string itemCode, CancellationToken cancellationToken)
        {
            var userIdString = _userContextService.GetId();
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("No se pudo obtener el login del usuario.");
            }

            var detailEntity = await _productRepository.GetProductDetailByItemCodeAsync(itemCode, userId, cancellationToken);
            if (detailEntity == null)
                return null;

            var features = (await _productRepository.GetFeaturesByItemCodeAsync(itemCode, cancellationToken))
                .Select(c => new FeatureDto(c.Titulo, c.Detalle))
                .ToList()
                .AsReadOnly();

            var similars = (await _productRepository.GetSimilarProductsByItemCodeAsync(itemCode, userId, cancellationToken))
                .Select(s => new SimilarProductDto(
                    s.CodigoItem,
                    s.Imagen,
                    s.DescripcionItem,
                    s.Disponible,
                    s.Pvp,
                    s.Detalle))
                .ToList()
                .AsReadOnly();

            var product = new ProductDto(
                detailEntity.CodigoItem,
                detailEntity.Imagen,
                detailEntity.Detalle,
                detailEntity.PvpDescuento,
                detailEntity.Pvp,
                detailEntity.PvpB,
                detailEntity.Calidad1,
                detailEntity.CalidadB,
                detailEntity.PesoML,
                detailEntity.PesoGSM,
                detailEntity.Ancho,
                detailEntity.Descuento,
                detailEntity.Cantidad1,
                detailEntity.CantidadB,
                detailEntity.FichaTecnica);

            return new ProductDetailDto(product, features, similars);
        }
    }
}
