using Dislana.Domain.ChatAssistant.Entities;
using Dislana.Domain.ChatAssistant.Interfaces;
using Dislana.Domain.Common.Enums;
using Dislana.Infrastructure.ChatAssistant.DTOs;
using Dislana.Infrastructure.Persistence.Dapper;

namespace Dislana.Infrastructure.ChatAssistant.Repositories
{
    public class ProductChatRepository : IProductChatRepository
    {
        private readonly IContextualDbExecutor _dbExecutor;
        private const DatabaseContext Context = DatabaseContext.Ecommerce;

        public ProductChatRepository(IContextualDbExecutor dbExecutor)
        {
            _dbExecutor = dbExecutor;
        }

        public async Task<IEnumerable<ProductEntity>> GetActiveProductsAsync(CancellationToken cancellationToken)
        {
            var query = @"
                SELECT 
                    grupo AS Grupo,
                    cod_item AS CodItem,
                    des_item AS DesItem,
                    imagen AS Imagen,
                    tipo AS Tipo,
                    acabado AS Acabado,
                    categoria AS Categoria,
                    atributo AS Atributo,
                    color AS Color,
                    PVP,
                    PVP_DCTO,
                    NOMBRE,
                    disponible AS Disponible,
                    POR_DES,
                    CTS,
                    NOM_CIU,
                    NOM_DEP,
                    DETALLE
                FROM dbo.productos
                WHERE disponible > 0";

            var dtos = await _dbExecutor.QueryAsync<ProductDto>(Context, query, null, null, cancellationToken);

            return dtos.Select(dto => new ProductEntity
            {
                Grupo = dto.Grupo?.Trim() ?? string.Empty,
                CodItem = dto.CodItem?.Trim() ?? string.Empty,
                DesItem = dto.DesItem?.Trim() ?? string.Empty,
                Imagen = dto.Imagen?.Trim() ?? string.Empty,
                Tipo = dto.Tipo?.Trim() ?? string.Empty,
                Acabado = dto.Acabado?.Trim() ?? string.Empty,
                Categoria = dto.Categoria?.Trim() ?? string.Empty,
                Atributo = dto.Atributo?.Trim() ?? string.Empty,
                Color = dto.Color?.Trim() ?? string.Empty,
                PVP = dto.PVP,
                PVP_DCTO = dto.PVP_DCTO,
                Nombre = dto.NOMBRE?.Trim() ?? string.Empty,
                Disponible = dto.Disponible,
                PorDes = dto.POR_DES,
                CTS = dto.CTS?.Trim() ?? string.Empty,
                NomCiu = dto.NOM_CIU?.Trim() ?? string.Empty,
                NomDep = dto.NOM_DEP?.Trim() ?? string.Empty,
                Detalle = dto.DETALLE?.Trim() ?? string.Empty
            });
        }
    }
}
