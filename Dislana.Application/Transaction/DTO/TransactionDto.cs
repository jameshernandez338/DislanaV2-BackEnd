namespace Dislana.Application.Transaction.DTO
{
    public record TransactionDto(
        string TypeDocument,
        string Number,
        string Date,
        string CustomerDni,
        string Customer,
        decimal Valor,
        string LinkInvoice,
        string Cufe,
        string LinkDian
    );
}
