using Dislana.Application.Order.DTOs;
using Dislana.Application.Payment.DTOs;

namespace Dislana.Application.Payment.Interfaces
{
    public interface IPaymentService
    {
        Task<WompiPaymentDto> CreatePaymentAsync(string login, PaymentRequestDto request, CancellationToken cancellationToken);
        Task<PaymentResponseDto> SaveOrderOnlyAsync(string login, PaymentRequestDto request, CancellationToken cancellationToken);
        Task ProcessWebhookAsync(WompiWebhookRequest request);
    }
}
