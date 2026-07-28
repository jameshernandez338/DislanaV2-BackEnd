using Dislana.Application.Common.Interfaces;
using Dislana.Application.Quote.DTOs;
using Dislana.Application.Quote.Interfaces;
using Dislana.Domain.Quote.Entities;
using Dislana.Domain.Quote.Interfaces;

namespace Dislana.Application.Quote
{
    public class QuoteService : IQuoteService
    {
        private readonly IQuoteRepository _quoteRepository;
        private readonly IUserContextService _userContextService;

        public QuoteService(
          IQuoteRepository quoteRepository,
          IUserContextService userContextService)
        {
            _quoteRepository = quoteRepository;
            _userContextService = userContextService;
        }

        public async Task<IReadOnlyList<QuoteDto>> GetQuotesAsync(string userId, CancellationToken cancellationToken)
        {
            var items = await _quoteRepository.GetQuotesAsync(userId, cancellationToken);
            return items.Select(i => new QuoteDto(
                i.Grupo,
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
                i.PrecioTotal,
                i.PrecioAnticipo)
            ).ToList()
             .AsReadOnly();
        }

        public async Task<CustomerTaxDto?> GetCustomerTaxesAsync(CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetUserId()
                ?? throw new UnauthorizedAccessException("No se pudo obtener el login del usuario.");

            var entity = await _quoteRepository.GetCustomerTaxesAsync(userId, cancellationToken);
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
                entity.UsaCupo,
                entity.BaseReteIca,
                entity.BaseReteIva
            );
        }

        public async Task<IReadOnlyList<CustomerBalanceEntryDto>> GetCustomerBalanceAsync(string type, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetUserId()
                ?? throw new UnauthorizedAccessException("No se pudo obtener el login del usuario.");

            IEnumerable<CustomerBalanceEntryEntity> items;

            // choose repository method based on type
            items = type switch
            {
                "saldoAFavor" => await _quoteRepository.GetCustomerOverdueBalance(userId, cancellationToken),
                "cartera" => await _quoteRepository.GetCustomerCreditBalance(userId, cancellationToken),
                "apin" => await _quoteRepository.GetCustomerApin(userId, cancellationToken),
                _ => await _quoteRepository.GetCustomerOverdueBalance(userId, cancellationToken)
            };

            return items.Select(i => new CustomerBalanceEntryDto(i.Observacion, i.Tipo, i.Numero, i.Fecha, i.Valor))
                        .ToList()
                        .AsReadOnly();
        }
    }
}
