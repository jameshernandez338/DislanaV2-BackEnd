using Dislana.Application.Auth.DTOs;
using Dislana.Application.Auth.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dislana.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.RegisterAsync(request, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.Message.Contains("registrado", StringComparison.OrdinalIgnoreCase))
                    return Conflict(new { message = result.Message });

                return BadRequest(new { message = result.Message });
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(request, cancellationToken);

            if (!result.IsSuccess)
                return Unauthorized(new { message = result.Message });

            return Ok(result);
        }
    }
}