using Dislana.Application.Stock.Interfaces;
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

            var login = User?.Identity?.Name
                        ?? User?.FindFirst("name")?.Value
                        ?? User?.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(login))
                return BadRequest(new { message = "No se pudo obtener el login del usuario." });

            var items = await _stockService.GetCommittedInventoryAsync(login, itemCode, cancellationToken);
            return Ok(items);
        }

        // GET api/stock/statement
        [HttpGet("statement")]
        public async Task<IActionResult> GetInventoryStatement(CancellationToken cancellationToken)
        {
            var login = User?.Identity?.Name
                        ?? User?.FindFirst("name")?.Value
                        ?? User?.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(login))
                return BadRequest(new { message = "No se pudo obtener el login del usuario." });

            var items = await _stockService.GetInventoryStatementAsync(login, cancellationToken);
            return Ok(items);
        }
    }
}
