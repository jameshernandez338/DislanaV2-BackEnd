using Dislana.Domain.Auth.Entities;

namespace Dislana.Domain.Auth.Interfaces
{
    public interface IUserRepository
    {
        Task<UserEntity?> CreateUserWithCredentialAsync(string name, string lastName, string email, string passwordHash, CancellationToken cancellationToken);
        Task<UserEntity?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken);
        Task<UserEntity?> GetUserByIdAsync(long userId, CancellationToken cancellationToken);
    }
}
