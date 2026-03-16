using Dislana.Domain.Auth.Entities;

namespace Dislana.Domain.Auth.Interfaces
{
    public interface IUserCredentialRepository
    {
        Task<UserCredentialEntity?> GetCredentialByUserIdAsync(long userId, CancellationToken cancellationToken);
    }
}
