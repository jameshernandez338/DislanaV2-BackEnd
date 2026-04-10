namespace Dislana.Domain.Order.Entities
{
    public class FabricFinishEntity
    {
        public string Acabado { get; set; } = string.Empty;
        public bool TieneTexto { get; set; }
        public decimal Valor { get; set; }
    }
}
