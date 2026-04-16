using Dislana.Application.Order.DTOs;
using Dislana.Application.Order.Interfaces;
using Dislana.Domain.Order.Entities;
using Dislana.Domain.Order.Interfaces;

namespace Dislana.Application.Order
{
    /// <summary>
    /// Application Service: SOLO orquesta, NO contiene lógica de negocio
    /// La lógica está en las entidades del Domain
    /// Responsabilidades:
    /// - Coordinar operaciones
    /// - Mapear DTOs ↔ Domain Entities
    /// - Gestionar transacciones
    /// </summary>
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<OrderSaveResponseDto> SaveOrderAsync(
            string userName,
            OrderRequestDto request,
            CancellationToken cancellationToken)
        {
            // 1. Crear la entidad Order (el Domain valida)
            var order = OrderEntity.Create(userName, request.Observacion);

            // 2. Agregar items (el Domain valida y gestiona la lógica)
            foreach (var itemDto in request.Items)
            {
                // Crear item del dominio
                var item = OrderItemEntity.Create(
                    itemDto.CodigoItem,
                    itemDto.Cantidad1,
                    itemDto.CantidadB,
                    itemDto.Pvp,
                    itemDto.PvpB
                );

                // Agregar acabados si existen
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

                // Agregar item a la orden (el Domain valida)
                order.AddItem(item);
            }

            // 3. Persistir (Infrastructure)
            var result = await _orderRepository.SaveAsync(order, cancellationToken);

            // 4. Mapear resultado a DTO
            return new OrderSaveResponseDto(result?.Message ?? string.Empty);
        }

        public async Task<IEnumerable<FabricFinishDto>> GetFabricFinishesAsync(
            string userName,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException("El usuario es requerido", nameof(userName));

            // Obtener entidades del dominio
            var entities = await _orderRepository.GetFabricFinishesAsync(userName, cancellationToken);

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
