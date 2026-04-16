using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Dislana.Domain.Transaction.Entities
{
    public class TransactionEntity
    {
        public string TypeDocument { get; private set; } = default!;
        public string Number { get; private set; } = default!;
        public string Date { get; private set; } = default!;
        public string CustomerDni { get; private set; } = default!;
        public string Customer { get; private set; } = default!;
        public decimal Valor { get; private set; }
        public string LinkInvoice { get; private set; } = default!;
        public string Cufe { get; private set; } = default!;
        public string LinkDian { get; private set; } = default!;

        private TransactionEntity() { }

        public TransactionEntity(string typeDocument, string number, string date, string customerDni, string customer, decimal valor, string linkInvoice, string cufe, string linkDian)
        {
            TypeDocument = typeDocument;
            Number = number;
            Date = date;
            CustomerDni = customerDni;
            Customer = customer;
            Valor = valor;
            LinkInvoice = linkInvoice;
            Cufe = cufe;
            LinkDian = linkDian;
        }
    }
}
