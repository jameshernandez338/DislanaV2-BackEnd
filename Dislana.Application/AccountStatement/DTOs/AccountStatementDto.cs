namespace Dislana.Application.AccountStatement.DTOs
{
    public record AccountStatementDto
    {
        public DateTime DocumentDate { get; init; }
        public DateTime DueDate { get; init; }
        public string DocumentNumber { get; init; } = string.Empty;
        public decimal Value { get; init; }
        public decimal Balance { get; init; }
        public string DocumentType { get; init; } = string.Empty;
        public string Type { get; init; } = string.Empty;
    }
}
