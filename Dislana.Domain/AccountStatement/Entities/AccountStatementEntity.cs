namespace Dislana.Domain.AccountStatement.Entities
{
    public class AccountStatementEntity
    {
        public DateTime DocumentDate { get; private set; }
        public DateTime DueDate { get; private set; }
        public string DocumentNumber { get; private set; }
        public decimal Value { get; private set; }
        public decimal Balance { get; private set; }
        public string DocumentType { get; private set; }
        public string Type { get; private set; }

        private AccountStatementEntity(
            DateTime documentDate,
            DateTime dueDate,
            string documentNumber,
            decimal value,
            decimal balance,
            string documentType,
            string type)
        {
            DocumentDate = documentDate;
            DueDate = dueDate;
            DocumentNumber = documentNumber;
            Value = value;
            Balance = balance;
            DocumentType = documentType;
            Type = type;
        }

        public static AccountStatementEntity Create(
            DateTime documentDate,
            DateTime dueDate,
            string documentNumber,
            decimal value,
            decimal balance,
            string documentType,
            string type)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                throw new ArgumentException("Document number cannot be empty.", nameof(documentNumber));

            return new AccountStatementEntity(
                documentDate,
                dueDate,
                documentNumber,
                value,
                balance,
                documentType,
                type);
        }
    }
}
