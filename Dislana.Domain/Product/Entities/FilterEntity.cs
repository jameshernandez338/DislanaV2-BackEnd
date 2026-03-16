namespace Dislana.Domain.Product.Entities
{
    public class FilterEntity
    {
        public string Filtro { get; }
        public string Valor { get; }
        public string CampoVista { get; }

        public FilterEntity(string filtro, string valor, string campoVista)
        {
            Filtro = filtro;
            Valor = valor;
            CampoVista = campoVista;
        }
    }
}
