using Dislana.Application.Payment.DTOs;
using Dislana.Application.Payment.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dislana.Api.Controllers
{
    [ApiController]
    [Route("api/payment")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService) => _paymentService = paymentService;

        // POST api/payment/create
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] PaymentRequestDto request, CancellationToken cancellationToken)
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

            var payment = await _paymentService.CreatePaymentAsync(login, request, cancellationToken);
            return Ok(payment);
        }

        // POST api/payment/save-only
        [HttpPost("save-only")]
        public async Task<IActionResult> SaveOnly([FromBody] PaymentRequestDto request, CancellationToken cancellationToken)
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

            var result = await _paymentService.SaveOrderOnlyAsync(login, request, cancellationToken);
            return Ok(result);
        }
    }
}