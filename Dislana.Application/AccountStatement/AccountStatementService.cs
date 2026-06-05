using Dislana.Application.AccountStatement.DTOs;
using Dislana.Application.AccountStatement.Interfaces;
using Dislana.Application.Common.Interfaces;
using Dislana.Domain.AccountStatement.Interfaces;

namespace Dislana.Application.AccountStatement
{
    public class AccountStatementService : IAccountStatementService
    {
        private readonly IAccountStatementRepository _accountStatementRepository;
        private readonly IUserContextService _userContextService;

        public AccountStatementService(
            IAccountStatementRepository accountStatementRepository,
            IUserContextService userContextService)
        {
            _accountStatementRepository = accountStatementRepository;
            _userContextService = userContextService;
        }

        public async Task<IEnumerable<AccountStatementDto>> GetAccountStatementAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken)
        {
            var userName = _userContextService.GetUserName();

            if (string.IsNullOrWhiteSpace(userName))
                throw new UnauthorizedAccessException("No se pudo obtener el login del usuario.");

            var statements = await _accountStatementRepository.GetAccountStatementAsync(userName, startDate, endDate, cancellationToken);

            return statements.Select(s => new AccountStatementDto
            {
                DocumentDate = s.DocumentDate,
                DueDate = s.DueDate,
                DocumentNumber = s.DocumentNumber,
                Value = s.Value,
                Balance = s.Balance,
                DocumentType = s.DocumentType,
                Type = s.Type
            });
        }
    }
}
