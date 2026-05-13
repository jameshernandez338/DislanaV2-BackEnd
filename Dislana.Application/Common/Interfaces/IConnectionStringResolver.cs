using Dislana.Domain.Common.Enums;

namespace Dislana.Application.Common.Interfaces
{
    public interface IConnectionStringResolver
    {
        /// <summary>
        /// Obtiene la cadena de conexión para un contexto específico
        /// </summary>
        string GetConnectionString(DatabaseContext context);
    }
}
