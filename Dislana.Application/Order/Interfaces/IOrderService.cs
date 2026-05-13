using Dislana.Application.Order.DTOs;

namespace Dislana.Application.Order.Interfaces
{
    public interface IOrderService
    {
        Task<OrderSaveResponseDto> SaveOrderAsync(OrderRequestDto request, CancellationToken cancellationToken);
        Task<IEnumerable<FabricFinishDto>> GetFabricFinishesAsync(CancellationToken cancellationToken);
    }
}
