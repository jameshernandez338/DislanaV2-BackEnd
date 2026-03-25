using Dislana.Application.Payment.DTOs;

namespace Dislana.Application.Payment.Interfaces
{
    public interface IPaymentService
    {
        Task<WompiPaymentDto> CreatePaymentAsync(string userName, PaymentRequestDto request, CancellationToken cancellationToken);
        Task<PaymentResponseDto> SaveOrderOnlyAsync(string userName, PaymentRequestDto request, CancellationToken cancellationToken);
        Task ProcessWebhookAsync(WompiWebhookRequest request);
    }
}
