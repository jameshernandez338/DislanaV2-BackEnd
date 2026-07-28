namespace Dislana.Domain.AccountStatement.Entities
{
    public class AccountStatementDetailEntity
    {
        public DateTime Date { get; private set; }
        public string DocumentNumber { get; private set; }
        public decimal Value { get; private set; }
        public string DocumentType { get; private set; }

        private AccountStatementDetailEntity(
            DateTime date,
            string documentNumber,
            decimal value,
            string documentType)
        {
            Date = date;
            DocumentNumber = documentNumber;
            Value = value;
            DocumentType = documentType;
        }

        public static AccountStatementDetailEntity Create(
            DateTime date,
            string documentNumber,
            decimal value,
            string documentType)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                throw new ArgumentException("Document number cannot be empty.", nameof(documentNumber));

            if (string.IsNullOrWhiteSpace(documentType))
                throw new ArgumentException("Document type cannot be empty.", nameof(documentType));

            return new AccountStatementDetailEntity(
                date,
                documentNumber,
                value,
                documentType);
        }
    }
}
