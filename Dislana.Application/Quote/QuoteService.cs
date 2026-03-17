using Dislana.Application.Quote.DTOs;
using Dislana.Application.Quote.Interfaces;
using Dislana.Domain.Quote.Interfaces;

namespace Dislana.Application.Quote
{
    public class QuoteService : IQuoteService
    {
        private readonly IQuoteRepository _quoteRepository;

        public QuoteService(IQuoteRepository quoteRepository) => _quoteRepository = quoteRepository;

        public async Task<IReadOnlyList<QuoteDto>> GetQuotesAsync(string login, CancellationToken cancellationToken)
        {
            var items = await _quoteRepository.GetQuotesAsync(login, cancellationToken);
            return items.Select(i => new QuoteDto(
                i.Documento, 
                i.Imagen, 
                i.Codigo, 
                i.Acabado, 
                i.Descripcion, 
                i.Calidad, 
                i.Linea,
                i.Saldo, 
                i.Separados,
                i.Cantidad,
                i.PrecioTotal)
            ).ToList()
             .AsReadOnly();
        }

        public async Task<CustomerBalanceDto?> GetCustomerBalanceAsync(string login, CancellationToken cancellationToken)
        {
            var entity = await _quoteRepository.GetCustomerBalanceAsync(login, cancellationToken);
            if (entity == null) return null;

            return new CustomerBalanceDto(
                entity.Descuento,
                entity.Iva,
                entity.ReteFuente,
                entity.ReteIva,
                entity.ReteIca,
                entity.Cartera,
                entity.Apin,
                entity.SaldoAFavor,
                entity.Cupo,
                entity.UsaCupo
            );
        }
    }
}
