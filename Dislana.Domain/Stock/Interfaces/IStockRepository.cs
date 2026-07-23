using Dislana.Domain.Stock.Entities;

namespace Dislana.Domain.Stock.Interfaces
{
    public interface IStockRepository
    {
        Task<IEnumerable<CommittedInventoryEntity>> GetCommittedInventoryAsync(int userId, string itemCode, CancellationToken cancellationToken);
        Task<IEnumerable<InventoryStatementEntity>> GetInventoryStatementAsync(int userId, CancellationToken cancellationToken);
    }
}
