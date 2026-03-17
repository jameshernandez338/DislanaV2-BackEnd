using Dislana.Application.Stock.DTOs;

namespace Dislana.Application.Stock.Interfaces
{
    public interface IStockService
    {
        Task<IReadOnlyList<CommittedInventoryDto>> GetCommittedInventoryAsync(string login, string itemCode, CancellationToken cancellationToken);
        Task<IReadOnlyList<InventoryStatementDto>> GetInventoryStatementAsync(string login, CancellationToken cancellationToken);
    }
}
