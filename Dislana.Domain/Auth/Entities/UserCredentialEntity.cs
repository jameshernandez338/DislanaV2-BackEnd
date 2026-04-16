using Dislana.Domain.Auth.ValueObjects;

namespace Dislana.Domain.Auth.Entities
{
    /// <summary>
    /// Entidad de Dominio: Representa las credenciales de un usuario
    /// </summary>
    public sealed class UserCredentialEntity
    {
        public long UserId { get; private set; }
        public HashedPassword PasswordHash { get; private set; }
        public DateTime PasswordChangedAt { get; private set; }

        private UserCredentialEntity(long userId, HashedPassword passwordHash, DateTime passwordChangedAt)
        {
            UserId = userId;
            PasswordHash = passwordHash;
            PasswordChangedAt = passwordChangedAt;
        }

        /// <summary>
        /// Factory Method: Crea credenciales para un nuevo registro (sin userId aún)
        /// </summary>
        public static UserCredentialEntity CreateForNewUser(string passwordHash)
        {
            var hashedPasswordVO = HashedPassword.Create(passwordHash);

            return new UserCredentialEntity(
                0, // UserId se asignará en BD
                hashedPasswordVO,
                DateTime.UtcNow
            );
        }

        /// <summary>
        /// Factory Method: Crea nuevas credenciales para un usuario existente
        /// </summary>
        public static UserCredentialEntity Create(long userId, string passwordHash)
        {
            if (userId <= 0)
                throw new Exceptions.DomainException("UserId inválido");

            var hashedPasswordVO = HashedPassword.Create(passwordHash);

            return new UserCredentialEntity(
                userId,
                hashedPasswordVO,
                DateTime.UtcNow
            );
        }

        /// <summary>
        /// Factory Method: Reconstruye desde el repositorio
        /// </summary>
        public static UserCredentialEntity Reconstitute(
            long userId,
            string passwordHash,
            DateTime passwordChangedAt)
        {
            var hashedPasswordVO = HashedPassword.Create(passwordHash);
            return new UserCredentialEntity(userId, hashedPasswordVO, passwordChangedAt);
        }

        /// <summary>
        /// Actualiza el password
        /// </summary>
        public void UpdatePassword(string newPasswordHash)
        {
            PasswordHash = HashedPassword.Create(newPasswordHash);
            PasswordChangedAt = DateTime.UtcNow;
        }
    }
}
