namespace Dislana.Application.Quote.DTOs
{
    public record CustomerTaxDto(
        decimal Descuento,
        decimal Iva,
        decimal ReteFuente,
        decimal ReteIva,
        decimal ReteIca,
        decimal Cartera,
        decimal Apin,
        decimal SaldoAFavor,
        decimal Cupo,
        bool UsaCupo
    );
}
