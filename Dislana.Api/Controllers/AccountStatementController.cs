using Dislana.Application.AccountStatement.DTOs;
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

        // GET api/account-statement?startDate=2024-01-01&endDate=2024-12-31&documentType=...
        [HttpGet]
        public async Task<IActionResult> GetAccountStatement(
            [FromQuery] AccountStatementRequestDto request,
            CancellationToken cancellationToken)
        {
            if (request.StartDate == default || request.EndDate == default)
                return BadRequest(new { message = "Las fechas de inicio y fin son requeridas." });

            if (request.StartDate > request.EndDate)
                return BadRequest(new { message = "La fecha de inicio no puede ser mayor a la fecha de fin." });

            var result = await _accountStatementService.GetAccountStatementAsync(request, cancellationToken);

            return Ok(result);
        }

        // GET api/account-statement/detail/{documentNumber}
        [HttpGet("detail/{documentNumber}")]
        public async Task<IActionResult> GetAccountStatementDetail(
            [FromRoute] string documentNumber,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                return BadRequest(new { message = "Document number is required." });

            var result = await _accountStatementService.GetAccountStatementDetailAsync(documentNumber, cancellationToken);

            return Ok(result);
        }
    }
}
