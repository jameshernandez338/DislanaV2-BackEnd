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
    }
}
