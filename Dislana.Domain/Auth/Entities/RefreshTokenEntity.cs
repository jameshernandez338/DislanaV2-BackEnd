namespace Dislana.Domain.Auth.Entities
{
    /// <summary>
    /// Entidad de Dominio: Representa un refresh token
    /// Rich Entity con comportamiento y validaciones de negocio
    /// </summary>
    public sealed class RefreshTokenEntity
    {
        private const int DefaultExpirationDays = 7;

        public string Token { get; private set; }
        public long UserId { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsRevoked { get; private set; }

        // Constructor privado
        private RefreshTokenEntity(
            string token,
            long userId,
            DateTime expiresAt,
            DateTime createdAt,
            bool isRevoked = false)
        {
            Token = token;
            UserId = userId;
            ExpiresAt = expiresAt;
            CreatedAt = createdAt;
            IsRevoked = isRevoked;
        }

        /// <summary>
        /// Factory Method: Crea un nuevo refresh token
        /// </summary>
        public static RefreshTokenEntity Create(string token, long userId)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new Exceptions.DomainException("El refresh token es requerido");

            if (userId <= 0)
                throw new Exceptions.DomainException("UserId inválido");

            var now = DateTime.UtcNow;

            return new RefreshTokenEntity(
                token: token.Trim(),
                userId: userId,
                expiresAt: now.AddDays(DefaultExpirationDays),
                createdAt: now,
                isRevoked: false
            );
        }

        /// <summary>
        /// Factory Method: Reconstruye desde el repositorio
        /// </summary>
        public static RefreshTokenEntity Reconstitute(
            string token,
            long userId,
            DateTime expiresAt,
            DateTime createdAt,
            bool isRevoked)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new Exceptions.DomainException("El refresh token es requerido");

            return new RefreshTokenEntity(token, userId, expiresAt, createdAt, isRevoked);
        }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsValid => !IsRevoked && !IsExpired;

        /// <summary>
        /// Revoca el token
        /// </summary>
        public void Revoke()
        {
            if (IsRevoked)
                throw new Exceptions.DomainException("El token ya está revocado");

            IsRevoked = true;
        }

        /// <summary>
        /// Valida si el token puede ser usado
        /// </summary>
        public void ValidateForUse()
        {
            if (IsRevoked)
                throw new Exceptions.DomainException("Refresh token revocado");

            if (IsExpired)
                throw new Exceptions.DomainException("Refresh token expirado");
        }
    }
}
