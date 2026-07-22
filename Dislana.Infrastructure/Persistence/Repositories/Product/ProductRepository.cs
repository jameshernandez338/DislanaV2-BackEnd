using Dislana.Domain.Common.Enums;
using Dislana.Domain.Product.Entities;
using Dislana.Domain.Product.Interfaces;
using Dislana.Infrastructure.Persistence.Dapper;
using System.Data;

namespace Dislana.Infrastructure.Persistence.Repositories.Product
{
    public class ProductRepository : IProductRepository
    {
        private readonly IContextualDbExecutor _dbExecutor;
        private const DatabaseContext Context = DatabaseContext.Ecommerce;

        public ProductRepository(IContextualDbExecutor dbExecutor) => _dbExecutor = dbExecutor;

        public async Task<IEnumerable<FilterEntity>> GetFiltersByTipoAsync(string type, CancellationToken cancellationToken)
        {
            const string spName = "usp_getFilterByType";

            var rows = await _dbExecutor.QueryAsync<dynamic>(
                Context,
                spName,
                new { tipo = type },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var list = new List<FilterEntity>();
            foreach (var r in rows)
            {
                list.Add(new FilterEntity((string)r.filtro!, (string)r.valor!, (string)r.campoVista!));
            }

            return list;
        }

        public async Task<IEnumerable<ProductListEntity>> GetProductsByTypeAsync(string type, int userId, CancellationToken cancellationToken)
        {
            const string spName = "usp_getProductsByType";

            var rows = await _dbExecutor.QueryAsync<ProductListEntity>(
                Context,
                spName,
                new { 
                    type,
                    userId
                },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return rows;
        }

        public async Task<ProductDetailEntity?> GetProductDetailByItemCodeAsync(string itemCode, CancellationToken cancellationToken)
        {
            const string spName = "usp_getProductDetail";

            var result = await _dbExecutor.QuerySingleOrDefaultAsync<ProductDetailEntity>(
                Context,
                spName,
                new { codigoItem = itemCode },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            return result;
        }

        public async Task<IEnumerable<FeatureEntity>> GetFeaturesByItemCodeAsync(string itemCode, CancellationToken cancellationToken)
        {
            const string spName = "usp_getFeaturesByItemCode";

            var rows = await _dbExecutor.QueryAsync<dynamic>(
                Context,
                spName,
                new { codigoItem = itemCode },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);

            var list = new List<FeatureEntity>();
            foreach (var r in rows)
            {
                list.Add(new FeatureEntity((string)r.Titulo!, (string)r.Detalle!));
            }

            return list;
        }

        public async Task<IEnumerable<SimilarProductEntity>> GetSimilarProductsByItemCodeAsync(string itemCode, CancellationToken cancellationToken)
        {
            const string spName = "usp_getSimilarProductsByItemCode";

            var result = await _dbExecutor.QueryAsync<SimilarProductEntity>(
                Context,
                spName,
                new { codigoItem = itemCode },
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken);            

            return result;
        }
    }
}
