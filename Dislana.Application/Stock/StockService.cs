using Dislana.Application.Stock.DTOs;
using Dislana.Application.Stock.Interfaces;
using Dislana.Domain.Stock.Interfaces;

namespace Dislana.Application.Stock
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;

        public StockService(IStockRepository stockRepository) => _stockRepository = stockRepository;

        public async Task<IReadOnlyList<CommittedInventoryDto>> GetCommittedInventoryAsync(string login, string itemCode, CancellationToken cancellationToken)
        {
            var items = await _stockRepository.GetCommittedInventoryAsync(login, itemCode, cancellationToken);
            return items.Select(i => new CommittedInventoryDto(i.Grupo, i.Documento, i.Fecha, i.Cantidad))
                        .ToList()
                        .AsReadOnly();
        }

        public async Task<IReadOnlyList<InventoryStatementDto>> GetInventoryStatementAsync(string login, CancellationToken cancellationToken)
        {
            var items = await _stockRepository.GetInventoryStatementAsync(login, cancellationToken);
            return items.Select(i => new InventoryStatementDto(
                i.Grupo,
                i.Documento,
                i.Fecha,
                i.Item,
                i.Descripcion,
                i.Cantidad,
                i.SaldoPendiente,
                i.CalidadLote,
                i.PrecioTotal,
                i.Imagen
            )).ToList().AsReadOnly();
        }
    }
}
