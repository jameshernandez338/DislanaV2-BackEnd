using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dislana.Api.Controllers
{
    [ApiController]
    [Route("api/transaction")]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService) => _transactionService = transactionService;

        // GET api/transaction/list
        [HttpGet("list")]
        public async Task<IActionResult> GetTransactionList(CancellationToken cancellationToken)
        {
            var items = await _transactionService.GetTransactionListAsync(cancellationToken);
            return Ok(items);
        }
    }
}
