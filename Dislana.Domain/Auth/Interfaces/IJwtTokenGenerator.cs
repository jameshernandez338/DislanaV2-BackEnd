using Dislana.Domain.Auth.Entities;

namespace Dislana.Domain.Auth.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string Generate(UserEntity user);
    }
}
