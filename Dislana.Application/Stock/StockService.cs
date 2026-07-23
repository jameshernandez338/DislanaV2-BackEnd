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
            var userIdString = _userContextService.GetId();
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in context");
            }

            var items = await _stockRepository.GetCommittedInventoryAsync(userId, itemCode, cancellationToken);
            return items.Select(i => new CommittedInventoryDto(i.Grupo, i.Documento, i.Fecha, i.Cantidad))
                        .ToList()
                        .AsReadOnly();
        }

        public async Task<IReadOnlyList<InventoryStatementDto>> GetInventoryStatementAsync(CancellationToken cancellationToken)
        {
            var userIdString = _userContextService.GetId();
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in context");
            }

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
                i.Imagen
            )).ToList().AsReadOnly();
        }
    }
}
