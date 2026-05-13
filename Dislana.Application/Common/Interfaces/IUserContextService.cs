namespace Dislana.Application.Common.Interfaces
{
    public interface IUserContextService
    {
        /// <summary>
        /// Obtiene el ID del usuario autenticado
        /// </summary>
        string? GetId();

        /// <summary>
        /// Obtiene el nombre de usuario
        /// </summary>
        string? GetUserName();

        /// <summary>
        /// Obtiene el correo electrónico del usuario
        /// </summary>
        string? GetEmail();
    }
}
