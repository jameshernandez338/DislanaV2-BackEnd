using Dislana.Application.AccountStatement.DTOs;

namespace Dislana.Application.AccountStatement.Interfaces
{
    public interface IAccountStatementService
    {
        Task<IEnumerable<AccountStatementDto>> GetAccountStatementAsync(AccountStatementRequestDto request, CancellationToken cancellationToken);
        Task<IEnumerable<AccountStatementDetailDto>> GetAccountStatementDetailAsync(string documentNumber, CancellationToken cancellationToken);
    }
}
