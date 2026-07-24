namespace Dislana.Application.AccountStatement.DTOs
{
    public record AccountStatementRequestDto(
        DateTime StartDate,
        DateTime EndDate,
        string? DocumentType = null
    );
}
