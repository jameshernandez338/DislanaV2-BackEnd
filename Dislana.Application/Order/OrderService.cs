using Dislana.Application.Common.Interfaces;
using Dislana.Application.Order.DTOs;
using Dislana.Application.Order.Interfaces;
using Dislana.Domain.Order.Entities;
using Dislana.Domain.Order.Interfaces;

namespace Dislana.Application.Order
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserContextService _userContextService;

        public OrderService(
            IOrderRepository orderRepository,
            IUserContextService userContextService)
        {
            _orderRepository = orderRepository;
            _userContextService = userContextService;
        }

        public async Task<OrderSaveResponseDto> SaveOrderAsync(
            OrderRequestDto request,
            CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetUserId()
                ?? throw new UnauthorizedAccessException("No se pudo obtener el login del usuario.");

            var order = OrderEntity.Create(userId, request.Observacion);

            foreach (var itemDto in request.Items)
            {
                var item = OrderItemEntity.Create(
                    itemDto.CodigoItem,
                    itemDto.Cantidad1,
                    itemDto.CantidadB,
                    itemDto.Pvp,
                    itemDto.PvpB
                );

                if (itemDto.Acabados != null)
                {
                    foreach (var acabadoDto in itemDto.Acabados)
                    {
                        var finish = FabricFinishEntity.Create(
                            acabadoDto.Acabado,
                            acabadoDto.TieneTexto,
                            acabadoDto.Texto,
                            acabadoDto.Valor
                        );
                        item.AddFinish(finish);
                    }
                }

                order.AddItem(item);
            }

            var result = await _orderRepository.SaveAsync(order, cancellationToken);

            return new OrderSaveResponseDto(result?.Message ?? string.Empty);
        }

        public async Task<IEnumerable<FabricFinishDto>> GetFabricFinishesAsync(
            CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetUserId()
                ?? throw new UnauthorizedAccessException("No se pudo obtener el login del usuario.");

            // Obtener entidades del dominio
            var entities = await _orderRepository.GetFabricFinishesAsync(userId, cancellationToken);

            // Mapear Domain Entities → DTOs
            return entities.Select(e => new FabricFinishDto(
                e.Name,
                e.RequiresText,
                e.Text,
                e.Price.Amount
            ));
        }
    }
}
