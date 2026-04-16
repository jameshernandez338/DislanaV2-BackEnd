using Dislana.Application.Transaction.DTO;

namespace Dislana.Application.Transaction.Interfaces
{
    public interface ITransactionService
    {
        Task<IReadOnlyList<TransactionDto>> GetTransactionListAsync(string login, CancellationToken cancellationToken);
    }
}
