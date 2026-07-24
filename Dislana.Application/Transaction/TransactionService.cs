using Dislana.Application.Common.Interfaces;
using Dislana.Application.Transaction.DTO;
using Dislana.Application.Transaction.Interfaces;
using Dislana.Domain.Transaction.Interfaces;

namespace Dislana.Application.Transaction
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IUserContextService _userContextService;

        public TransactionService(ITransactionRepository transactionRepository, IUserContextService userContextService)
        {
            _transactionRepository = transactionRepository;
            _userContextService = userContextService;
        }

        public async Task<IReadOnlyList<TransactionDto>> GetTransactionListAsync(CancellationToken cancellationToken)
        {
            var userIdString = _userContextService.GetId();
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                throw new UnauthorizedAccessException("No se pudo obtener el login del usuario.");
            }

            var items = await _transactionRepository.GetTransactionListAsync(userId, cancellationToken);
            return items.Select(i => new TransactionDto(
                i.TypeDocument,
                i.Number,
                i.Date,
                i.CustomerDni,
                i.Customer,
                i.Valor,
                i.LinkInvoice,
                i.Cufe,
                i.LinkDian
            )).ToList().AsReadOnly();
        }
    }
}
