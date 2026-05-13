using Dislana.Application.Payment.DTOs;

namespace Dislana.Application.Payment.Interfaces
{
    public interface IPaymentService
    {
        Task<WompiPaymentDto> CreatePaymentAsync(PaymentRequestDto request, CancellationToken cancellationToken);
        Task<PaymentResponseDto> SaveOrderOnlyAsync(PaymentRequestDto request, CancellationToken cancellationToken);
        Task ProcessWebhookAsync(WompiWebhookRequest request);
    }
}
