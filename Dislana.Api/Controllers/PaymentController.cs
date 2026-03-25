using Dislana.Application.Payment.DTOs;
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

            var userName = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userName))
                return BadRequest(new { message = "No se pudo obtener el login del usuario." });

            var payment = await _paymentService.CreatePaymentAsync(userName, request, cancellationToken);
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

            var userName = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userName))
                return BadRequest(new { message = "No se pudo obtener el login del usuario." });

            var result = await _paymentService.SaveOrderOnlyAsync(userName, request, cancellationToken);
            return Ok(result);
        }

        // POST wompi/webhook
        [HttpPost("/wompi/webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> WompiWebhook([FromBody] WompiWebhookRequest body)
        {
            if (body == null)
                return Ok("ok");

            await _paymentService.ProcessWebhookAsync(body);

            return Ok("ok");
        }
    }
}