namespace Dislana.Application.Product.DTOs
{
    public record ProductDto(
        string CodeItem,
        string Imagen,
        string Detalle,
        double PvpDescuento,
        double Pvp,
        double PvpB,
        double Calidad1,
        double CalidadB,
        double PesoML,
        double PesoGSM,
        double Ancho,
        string Descuento,
        decimal Cantidad1,
        decimal CantidadB,
        string FichaTecnica);    
}