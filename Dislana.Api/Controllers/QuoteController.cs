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
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(new { message = "No se pudo obtener el ID del usuario." });

            var items = await _quoteService.GetQuotesAsync(userId, cancellationToken);
            return Ok(items);
        }

        // GET api/quote/detail/{item}
        [HttpGet("detail/{item}")]
        public async Task<IActionResult> GetDetail([FromRoute] string item, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(item))
                return BadRequest(new { message = "Item is required." });

            var details = await _quoteService.GetQuoteDetailAsync(item, cancellationToken);
            return Ok(details);
        }

        // GET api/quote/customer-address
        [HttpGet("customer-address")]
        public async Task<IActionResult> GetCustomerAddress(CancellationToken cancellationToken)
        {
            var addresses = await _quoteService.GetCustomerAddressAsync(cancellationToken);
            return Ok(addresses);
        }

        // GET api/quote/customer-taxes
        [HttpGet("customer-taxes")]
        public async Task<IActionResult> GetCustomerTaxes(CancellationToken cancellationToken)
        {
            var balance = await _quoteService.GetCustomerTaxesAsync(cancellationToken);
            if (balance == null)
                return NotFound();

            return Ok(balance);
        }

        // GET api/quote/customer-balance?type=...
        [HttpGet("customer-balance")]
        public async Task<IActionResult> GetCustomerBalance([FromQuery] string type, CancellationToken cancellationToken)
        {
            var items = await _quoteService.GetCustomerBalanceAsync(type, cancellationToken);
            return Ok(items);
        }
    }
}
