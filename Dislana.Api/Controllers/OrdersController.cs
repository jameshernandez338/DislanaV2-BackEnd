using Dislana.Application.Order.DTOs;
using Dislana.Application.Order.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dislana.Api.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService) => _orderService = orderService;

        // POST api/orders/save
        [HttpPost("save")]
        public async Task<IActionResult> Save([FromBody] OrderRequestDto request, CancellationToken cancellationToken)
        {
            if (request == null)
                return BadRequest(new { message = "El cuerpo de la petición es requerido." });

            if (request.Items == null || !request.Items.Any())
                return BadRequest(new { message = "La lista de items es requerida." });

            var login = User?.Identity?.Name
                        ?? User?.FindFirst("name")?.Value
                        ?? User?.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(login))
                return BadRequest(new { message = "No se pudo obtener el login del usuario." });

            var result = await _orderService.SaveOrderAsync(login, request, request.Orillo ?? string.Empty, cancellationToken);

            return Ok(result);
        }
    }
}
