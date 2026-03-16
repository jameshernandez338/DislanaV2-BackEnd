namespace Dislana.Application.Product.DTOs
{
    public record ProductListDto(
        string CodigoItem,
        string DescripcionItem,
        string Imagen,
        decimal Pvp,
        decimal Disponible,
        string Departamento,
        string Ciudad,
        string Nombre,
        string Cts,
        string Tipo,
        string Acabado,
        string Categoria,
        string Atributo,
        string Color,
        string Detalle,
        decimal Separado,
        decimal Pendiente);
}