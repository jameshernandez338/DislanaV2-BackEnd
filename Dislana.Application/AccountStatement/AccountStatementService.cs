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
            AccountStatementRequestDto request,
            CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetUserId()
                ?? throw new UnauthorizedAccessException("No se pudo obtener el login del usuario.");

            var statements = await _accountStatementRepository.GetAccountStatementAsync(
                userId, 
                request.StartDate, 
                request.EndDate, 
                request.DocumentType,
                cancellationToken);

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

        public async Task<IEnumerable<AccountStatementDetailDto>> GetAccountStatementDetailAsync(
            string documentNumber,
            CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetUserId()
                ?? throw new UnauthorizedAccessException("No se pudo obtener el login del usuario.");

            var details = await _accountStatementRepository.GetAccountStatementDetailAsync(
                userId, 
                documentNumber,
                cancellationToken);

            return details.Select(d => new AccountStatementDetailDto
            {
                Date = d.Date,
                DocumentNumber = d.DocumentNumber,
                Value = d.Value,
                DocumentType = d.DocumentType
            });
        }
    }
}
