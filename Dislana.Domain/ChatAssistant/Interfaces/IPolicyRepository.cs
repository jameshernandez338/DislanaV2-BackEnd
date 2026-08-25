using Dislana.Domain.ChatAssistant.Entities;

namespace Dislana.Domain.ChatAssistant.Interfaces
{
    public interface IPolicyRepository
    {
        Task<PolicyEntity?> GetPolicyContentAsync(CancellationToken cancellationToken);
    }
}
