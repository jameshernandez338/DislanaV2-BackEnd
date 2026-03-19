using Dislana.Application.Quote.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dislana.Api.Controllers
{
    [ApiController]
    [Route("api/quote")]
    [Authorize]
    public class QuoteController : ControllerBase
    {
        private readonly IQuoteService _quoteService;

        public QuoteController(IQuoteService quoteService) => _quoteService = quoteService;

        // GET api/quote/list
        [HttpGet("list")]
        public async Task<IActionResult> GetList(CancellationToken cancellationToken)
        {
            var login = User?.Identity?.Name
                        ?? User?.FindFirst("name")?.Value
                        ?? User?.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(login))
                return BadRequest(new { message = "No se pudo obtener el login del usuario." });

            var items = await _quoteService.GetQuotesAsync(login, cancellationToken);
            return Ok(items);
        }

        // GET api/quote/customer-taxes
        [HttpGet("customer-taxes")]
        public async Task<IActionResult> GetCustomerTaxes(CancellationToken cancellationToken)
        {
            var login = User?.Identity?.Name
                        ?? User?.FindFirst("name")?.Value
                        ?? User?.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(login))
                return BadRequest(new { message = "No se pudo obtener el login del usuario." });

            var balance = await _quoteService.GetCustomerTaxesAsync(login, cancellationToken);
            if (balance == null)
                return NotFound();

            return Ok(balance);
        }

        // GET api/quote/customer-balance?type=...
        [HttpGet("customer-balance")]
        public async Task<IActionResult> GetCustomerBalance([FromQuery] string type, CancellationToken cancellationToken)
        {
            var login = User?.Identity?.Name
                        ?? User?.FindFirst("name")?.Value
                        ?? User?.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(login))
                return BadRequest(new { message = "No se pudo obtener el login del usuario." });

            var items = await _quoteService.GetCustomerBalanceAsync(login, type, cancellationToken);
            return Ok(items);
        }
    }
}
