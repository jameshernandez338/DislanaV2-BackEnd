namespace Dislana.Application.AccountStatement.DTOs
{
    public record AccountStatementDetailDto
    {
        public DateTime Date { get; init; }
        public string DocumentNumber { get; init; } = string.Empty;
        public decimal Value { get; init; }
        public string DocumentType { get; init; } = string.Empty;
    }
}
