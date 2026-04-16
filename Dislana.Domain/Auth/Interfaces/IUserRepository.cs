using Dislana.Domain.Auth.Entities;

namespace Dislana.Domain.Auth.Interfaces
{
    /// <summary>
    /// Repositorio del Dominio: Define contrato para persistencia de usuarios
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Crea un nuevo usuario con sus credenciales
        /// </summary>
        Task<UserEntity?> CreateUserWithCredentialAsync(
            UserEntity user,
            UserCredentialEntity credential,
            CancellationToken cancellationToken);

        /// <summary>
        /// Obtiene un usuario por su nombre de usuario (email)
        /// </summary>
        Task<UserEntity?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken);

        /// <summary>
        /// Obtiene un usuario por su ID
        /// </summary>
        Task<UserEntity?> GetUserByIdAsync(long userId, CancellationToken cancellationToken);
    }
}
