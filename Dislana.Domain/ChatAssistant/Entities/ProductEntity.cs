namespace Dislana.Domain.ChatAssistant.Entities
{
    public class ProductEntity
    {
        public string Grupo { get; set; } = string.Empty;
        public string CodItem { get; set; } = string.Empty;
        public string DesItem { get; set; } = string.Empty;
        public string Imagen { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string Acabado { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Atributo { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int PVP { get; set; }
        public int PVP_DCTO { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public double Disponible { get; set; }
        public int PorDes { get; set; }
        public string CTS { get; set; } = string.Empty;
        public string NomCiu { get; set; } = string.Empty;
        public string NomDep { get; set; } = string.Empty;
        public string Detalle { get; set; } = string.Empty;
    }
}
