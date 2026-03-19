using System;

namespace Dislana.Application.Quote.DTOs
{
    public record CustomerBalanceEntryDto(string Observacion, string Tipo, string Numero, DateTime Fecha, decimal Valor);
}
