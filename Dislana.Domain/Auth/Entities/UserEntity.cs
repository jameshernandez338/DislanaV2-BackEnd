namespace Dislana.Domain.Auth.Entities
{
    public class UserEntity
    {
        public long Id { get; }
        public string UserName { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public bool IsActive { get; }

        public UserEntity(
            long id,
            string userName,
            string email,
            string firstName,
            string lastName,
            bool isActive)
        {
            Id = id;
            UserName = userName;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            IsActive = isActive;
        }

        public string FullName => $"{FirstName} {LastName}";
    }
}
