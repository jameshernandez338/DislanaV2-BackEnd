using Dislana.Domain.Stock.Entities;

namespace Dislana.Domain.Stock.Interfaces
{
    public interface IStockRepository
    {
        Task<IEnumerable<CommittedInventoryEntity>> GetCommittedInventoryAsync(string login, string itemCode, CancellationToken cancellationToken);
        Task<IEnumerable<InventoryStatementEntity>> GetInventoryStatementAsync(string login, CancellationToken cancellationToken);
    }
}
