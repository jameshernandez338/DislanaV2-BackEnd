namespace Dislana.Domain.Auth.Entities
{
    public class UserCredentialEntity
    {
        public long UserId { get; private set; }
        public string PasswordHash { get; private set; }
        public DateTime PasswordChangedAt { get; private set; }

        public UserCredentialEntity(long userId, string passwordHash, DateTime passwordChangedAt)
        {
            UserId = userId;
            PasswordHash = passwordHash;
            PasswordChangedAt = passwordChangedAt;
        }
    }
}
