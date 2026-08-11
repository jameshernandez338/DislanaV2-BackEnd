using Dislana.Application.Stock.DTOs;

namespace Dislana.Application.Stock.Interfaces
{
    public interface IStockService
    {
        Task<IReadOnlyList<CommittedInventoryDto>> GetCommittedInventoryAsync(string itemCode, CancellationToken cancellationToken);
        Task<IReadOnlyList<InventoryStatementDto>> GetInventoryStatementAsync(CancellationToken cancellationToken);
        Task<IReadOnlyList<InventoryStatementDetailDto>> GetInventoryStatementDetailAsync(string item, CancellationToken cancellationToken);
        Task CancelOrderAsync(string document, string item, CancellationToken cancellationToken);
    }
}
