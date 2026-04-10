using Dislana.Application.Order.DTOs;

namespace Dislana.Application.Order.Interfaces
{
    public interface IOrderService
    {
        Task<OrderSaveResponseDto> SaveOrderAsync(string userName, OrderRequestDto request, CancellationToken cancellationToken);
        Task<IEnumerable<FabricFinishDto>> GetFabricFinishesAsync(string userName, CancellationToken cancellationToken);
    }
}
