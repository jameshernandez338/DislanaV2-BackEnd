using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dislana.Api.Controllers
{
    [ApiController]
    [Route("api/stock")]
    [Authorize]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService) => _stockService = stockService;

        // GET api/stock/committed?itemCode=...
        [HttpGet("committed")]
        public async Task<IActionResult> GetCommitted([FromQuery] string itemCode, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(itemCode))
                return BadRequest(new { message = "El parámetro 'itemCode' es requerido." });

            var items = await _stockService.GetCommittedInventoryAsync(itemCode, cancellationToken);
            return Ok(items);
        }

        // GET api/stock/statement
        [HttpGet("statement")]
        public async Task<IActionResult> GetInventoryStatement(CancellationToken cancellationToken)
        {
            var items = await _stockService.GetInventoryStatementAsync(cancellationToken);
            return Ok(items);
        }

        // GET api/stock/statement/detail/{item}
        [HttpGet("statement/detail/{item}")]
        public async Task<IActionResult> GetInventoryStatementDetail([FromRoute] string item, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(item))
                return BadRequest(new { message = "Item is required." });

            var details = await _stockService.GetInventoryStatementDetailAsync(item, cancellationToken);
            return Ok(details);
        }

        // DELETE api/stock/cancel-order?document=...&item=...
        [HttpDelete("cancel-order")]
        public async Task<IActionResult> CancelOrder(
            [FromQuery] string document,
            [FromQuery] string item,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(document))
                return BadRequest(new { message = "El parámetro 'document' es requerido." });

            if (string.IsNullOrWhiteSpace(item))
                return BadRequest(new { message = "El parámetro 'item' es requerido." });

            await _stockService.CancelOrderAsync(document, item, cancellationToken);

            return Ok(new { message = "Pedido cancelado exitosamente." });
        }
    }
}
