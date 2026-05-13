using System.ComponentModel.DataAnnotations;

namespace Dislana.Application.ChatAssistant.Request
{
    public class ChatMessageRequest
    {
        [Required(ErrorMessage = "SessionId is required")]
        public string SessionId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mensaje is required")]
        [StringLength(1000, ErrorMessage = "Mensaje cannot exceed 1000 characters")]
        public string Message { get; set; } = string.Empty;
    }
}
