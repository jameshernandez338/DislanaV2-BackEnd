using Dislana.Application.ChatAssistant.Request;
using Dislana.Application.ChatAssistant.Response;

namespace Dislana.Application.ChatAssistant.Interfaces
{
    public interface IChatAssistantService
    {
        Task<ChatMessageResponse> ProcessMessageAsync(ChatMessageRequest request, CancellationToken cancellationToken);

        Task<GeneratePdfReportResponse> GeneratePdfReportAsync(GeneratePdfReportRequest request, CancellationToken cancellationToken);
    }
}
