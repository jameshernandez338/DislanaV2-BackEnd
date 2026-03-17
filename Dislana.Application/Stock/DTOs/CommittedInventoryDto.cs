using System;

namespace Dislana.Application.Stock.DTOs
{
    public record CommittedInventoryDto(string Grupo, string Documento, DateTime Fecha, decimal Cantidad);
}
