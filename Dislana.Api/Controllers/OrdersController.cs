using Dislana.Application.Order.DTOs;
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

            var userName = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userName))
                return BadRequest(new { message = "No se pudo obtener el login del usuario." });

            var result = await _orderService.SaveOrderAsync(userName, request, request.Orillo ?? string.Empty, cancellationToken);

            return Ok(result);
        }
    }
}
