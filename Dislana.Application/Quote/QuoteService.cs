using Dislana.Application.Quote.DTOs;
using Dislana.Application.Quote.Interfaces;
using Dislana.Domain.Quote.Entities;
using Dislana.Domain.Quote.Interfaces;

namespace Dislana.Application.Quote
{
    public class QuoteService : IQuoteService
    {
        private readonly IQuoteRepository _quoteRepository;

        public QuoteService(IQuoteRepository quoteRepository) => _quoteRepository = quoteRepository;

        public async Task<IReadOnlyList<QuoteDto>> GetQuotesAsync(string userId, CancellationToken cancellationToken)
        {
            var items = await _quoteRepository.GetQuotesAsync(userId, cancellationToken);
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

        public async Task<CustomerTaxDto?> GetCustomerTaxesAsync(string login, CancellationToken cancellationToken)
        {
            var entity = await _quoteRepository.GetCustomerTaxesAsync(login, cancellationToken);
            if (entity == null) return null;

            return new CustomerTaxDto(
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

        public async Task<IReadOnlyList<CustomerBalanceEntryDto>> GetCustomerBalanceAsync(string login, string type, CancellationToken cancellationToken)
        {
            IEnumerable<CustomerBalanceEntryEntity> items;

            // choose repository method based on type
            items = type switch
            {
                "saldoAFavor" => await _quoteRepository.GetCustomerOverdueBalance(login, cancellationToken),
                "cartera" => await _quoteRepository.GetCustomerCreditBalance(login, cancellationToken),
                "apin" => await _quoteRepository.GetCustomerApin(login, cancellationToken),
                _ => await _quoteRepository.GetCustomerOverdueBalance(login, cancellationToken)
            };

            return items.Select(i => new CustomerBalanceEntryDto(i.Observacion, i.Tipo, i.Numero, i.Fecha, i.Valor))
                        .ToList()
                        .AsReadOnly();
        }
    }
}
