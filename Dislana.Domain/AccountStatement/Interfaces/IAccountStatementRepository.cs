using Dislana.Domain.AccountStatement.Entities;

namespace Dislana.Domain.AccountStatement.Interfaces
{
    public interface IAccountStatementRepository
    {
        Task<IEnumerable<AccountStatementEntity>> GetAccountStatementAsync(string login, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    }
}
