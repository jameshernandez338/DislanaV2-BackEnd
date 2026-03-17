using System;

namespace Dislana.Domain.Stock.Entities
{
    public class CommittedInventoryEntity
    {
        public string Grupo { get; private set; } = default!;
        public string Documento { get; private set; } = default!;
        public DateTime Fecha { get; private set; }
        public decimal Cantidad { get; private set; }
        private CommittedInventoryEntity() { }        public CommittedInventoryEntity(string grupo, string documento, DateTime fecha, decimal cantidad)        {
            Grupo = grupo;
            Documento = documento;
            Fecha = fecha;
            Cantidad = cantidad;
        }
    }
}
