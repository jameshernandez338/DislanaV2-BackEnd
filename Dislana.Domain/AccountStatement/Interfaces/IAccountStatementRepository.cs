using Dislana.Domain.AccountStatement.Entities;

namespace Dislana.Domain.AccountStatement.Interfaces
{
    public interface IAccountStatementRepository
    {
        Task<IEnumerable<AccountStatementEntity>> GetAccountStatementAsync(int userId, DateTime startDate, DateTime endDate, string? documentType, CancellationToken cancellationToken);
        Task<IEnumerable<AccountStatementDetailEntity>> GetAccountStatementDetailAsync(int userId, string documentNumber, CancellationToken cancellationToken);
    }
}
