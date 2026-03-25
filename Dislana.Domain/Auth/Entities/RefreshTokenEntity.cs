namespace Dislana.Domain.Auth.Entities
{
    public class RefreshTokenEntity
    {
        public string Token { get; }
        public long UserId { get; }
        public DateTime ExpiresAt { get; }
        public DateTime CreatedAt { get; }
        public bool IsRevoked { get; }

        public RefreshTokenEntity(
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

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        public bool IsValid => !IsRevoked && !IsExpired;
    }
}
