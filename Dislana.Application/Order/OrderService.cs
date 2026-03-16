using Dislana.Application.Order.DTOs;
using Dislana.Application.Order.Interfaces;
using Dislana.Domain.Order.Interfaces;
using System.Globalization;
using System.Xml.Linq;

namespace Dislana.Application.Order
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository) => _orderRepository = orderRepository;

        public async Task<OrderSaveResponseDto> SaveOrderAsync(string login, OrderRequestDto request, string orillo, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(login)) throw new ArgumentException("login is required", nameof(login));

            if (request == null) throw new ArgumentNullException(nameof(request));

            var root = new XElement("Items",
                request.Items.Select(i =>
                    new XElement("Item",
                        new XElement("CodigoItem", i.CodigoItem ?? string.Empty),
                        new XElement("Cantidad1", i.Cantidad1.ToString(CultureInfo.InvariantCulture)),
                        new XElement("CantidadB", i.CantidadB.ToString(CultureInfo.InvariantCulture)),
                        new XElement("Pvp", i.Pvp.ToString(CultureInfo.InvariantCulture)),
                        new XElement("PvpB", i.PvpB.ToString(CultureInfo.InvariantCulture))
                    )
                )
            );

            var pedido = new XDocument(root).ToString(SaveOptions.DisableFormatting);

            var repoResult = await _orderRepository.SaveOrderAsync(login, pedido, orillo ?? string.Empty, cancellationToken);

            var message = repoResult?.Message ?? string.Empty;
            return new OrderSaveResponseDto(message);
        }
    }
}
