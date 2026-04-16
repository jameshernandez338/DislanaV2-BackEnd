using Dislana.Application.Transaction.DTO;
using Dislana.Application.Transaction.Interfaces;
using Dislana.Domain.Transaction.Interfaces;

namespace Dislana.Application.Transaction
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(ITransactionRepository transactionRepository) => _transactionRepository = transactionRepository;

        public async Task<IReadOnlyList<TransactionDto>> GetTransactionListAsync(string login, CancellationToken cancellationToken)
        {
            var items = await _transactionRepository.GetTransactionListAsync(login, cancellationToken);
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
