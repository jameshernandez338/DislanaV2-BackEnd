using Dislana.Application.ChatAssistant.Interfaces;
using Dislana.Application.ChatAssistant.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dislana.Api.Controllers
{
    [ApiController]
    [Route("api/chat-assistant")]
    [Authorize]
    public class ChatAssistantController : Controller
    {
        private readonly IChatAssistantService _chatAssistantService;

        public ChatAssistantController(
            IChatAssistantService chatAssistantService)
        {
            _chatAssistantService = chatAssistantService;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageRequest request, CancellationToken cancellationToken)
        {
            var response = await _chatAssistantService.ProcessMessageAsync(request, cancellationToken);
            return Ok(response);
        }

        [HttpPost("generate-pdf")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GeneratePdfReport(
            [FromBody] GeneratePdfReportRequest request, 
            CancellationToken cancellationToken)
        {
            var response = await _chatAssistantService.GeneratePdfReportAsync(request, cancellationToken);

            if (!response.IsSuccess)
                return BadRequest(new { message = response.Message });

            return File(
                response.PdfBytes!, 
                "application/pdf", 
                response.FileName!
            );
        }
    }
}
