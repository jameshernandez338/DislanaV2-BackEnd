using Dislana.Application.Order.DTOs;

namespace Dislana.Application.Order.Interfaces
{
    public interface IOrderService
    {
        Task<OrderSaveResponseDto> SaveOrderAsync(string userName, OrderRequestDto request, string orillo, CancellationToken cancellationToken);
    }
}
