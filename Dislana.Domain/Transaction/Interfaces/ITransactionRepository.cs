using Dislana.Domain.Transaction.Entities;

namespace Dislana.Domain.Transaction.Interfaces
{
    public interface ITransactionRepository
    {
        Task<IEnumerable<TransactionEntity>> GetTransactionListAsync(int userId, CancellationToken cancellationToken);
    }
}
