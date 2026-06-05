using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dislana.Api.Controllers
{
    [ApiController]
    [Route("api/account-statement")]
    [Authorize]
    public class AccountStatementController : ControllerBase
    {
        private readonly IAccountStatementService _accountStatementService;

        public AccountStatementController(IAccountStatementService accountStatementService) 
            => _accountStatementService = accountStatementService;

        // GET api/account-statement?startDate=2024-01-01&endDate=2024-12-31
        [HttpGet]
        public async Task<IActionResult> GetAccountStatement(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            CancellationToken cancellationToken)
        {
            if (startDate == default || endDate == default)
                return BadRequest(new { message = "Las fechas de inicio y fin son requeridas." });

            if (startDate > endDate)
                return BadRequest(new { message = "La fecha de inicio no puede ser mayor a la fecha de fin." });

            var result = await _accountStatementService.GetAccountStatementAsync(startDate, endDate, cancellationToken);

            return Ok(result);
        }
    }
}
