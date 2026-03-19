using System;

namespace Dislana.Domain.Quote.Entities
{
    public class CustomerBalanceEntryEntity
    {
        public string Observacion { get; private set; } = default!;
        public string Tipo { get; private set; } = default!;
        public string Numero { get; private set; } = default!;
        public DateTime Fecha { get; private set; }
        public decimal Valor { get; private set; }

        private CustomerBalanceEntryEntity() { }

        public CustomerBalanceEntryEntity(string observacion, string tipo, string numero, DateTime fecha, decimal valor)
        {
            Observacion = observacion;
            Tipo = tipo;
            Numero = numero;
            Fecha = fecha;
            Valor = valor;
        }
    }
}
