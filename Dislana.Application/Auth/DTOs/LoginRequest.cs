using System.ComponentModel.DataAnnotations;

namespace Dislana.Application.Auth.DTOs
{
    public record LoginRequest(
        [Required] string UserName,
        [Required] string Password
    );
}
