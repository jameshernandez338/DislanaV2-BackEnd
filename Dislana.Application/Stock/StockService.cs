using Dislana.Application.Common.Interfaces;
using Dislana.Application.Stock.DTOs;
using Dislana.Application.Stock.Interfaces;
using Dislana.Domain.Stock.Interfaces;

namespace Dislana.Application.Stock
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepository;
        private readonly IUserContextService _userContextService;

        public StockService(IStockRepository stockRepository, IUserContextService userContextService)
        {
            _stockRepository = stockRepository;
            _userContextService = userContextService;
        }

        public async Task<IReadOnlyList<CommittedInventoryDto>> GetCommittedInventoryAsync(string itemCode, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetUserId()
                ?? throw new UnauthorizedAccessException("No se pudo obtener el login del usuario.");

            var items = await _stockRepository.GetCommittedInventoryAsync(userId, itemCode, cancellationToken);
            return items.Select(i => new CommittedInventoryDto(i.Grupo, i.Documento, i.Fecha, i.Cantidad))
                        .ToList()
                        .AsReadOnly();
        }

        public async Task<IReadOnlyList<InventoryStatementDto>> GetInventoryStatementAsync(CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetUserId()
                ?? throw new UnauthorizedAccessException("No se pudo obtener el login del usuario.");

            var items = await _stockRepository.GetInventoryStatementAsync(userId, cancellationToken);
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
                i.Imagen,
                i.Estado
            )).ToList().AsReadOnly();
        }

        public async Task<IReadOnlyList<InventoryStatementDetailDto>> GetInventoryStatementDetailAsync(string item, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetUserId()
                ?? throw new UnauthorizedAccessException("No se pudo obtener el login del usuario.");

            var details = await _stockRepository.GetInventoryStatementDetailAsync(userId, item, cancellationToken);

            return details.Select(d => new InventoryStatementDetailDto(
                d.Codigo,
                d.Documento,
                d.Calidad,
                d.Separados,
                d.Cantidad,
                d.PrecioTotal
            )).ToList()
              .AsReadOnly();
        }

        public async Task CancelOrderAsync(string document, string item, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(document))
                throw new ArgumentException("Document cannot be empty.", nameof(document));

            if (string.IsNullOrWhiteSpace(item))
                throw new ArgumentException("Item cannot be empty.", nameof(item));

            await _stockRepository.CancelOrderAsync(document, item, cancellationToken);
        }
    }
}
