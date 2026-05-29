namespace Dislana.Infrastructure.ChatAssistant.DTOs
{
    public class ProductDto
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
        public string NOMBRE { get; set; } = string.Empty;
        public double Disponible { get; set; }
        public int POR_DES { get; set; }
        public string CTS { get; set; } = string.Empty;
        public string NOM_CIU { get; set; } = string.Empty;
        public string NOM_DEP { get; set; } = string.Empty;
        public string DETALLE { get; set; } = string.Empty;
    }
}
