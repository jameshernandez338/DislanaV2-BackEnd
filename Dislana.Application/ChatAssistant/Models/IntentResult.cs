using System.Text.Json.Serialization;

namespace Dislana.Application.ChatAssistant.Models
{
    public class IntentResult
    {
        [JsonPropertyName("needsProducts")]
        public bool NeedsProducts { get; set; }

        [JsonPropertyName("needsPayments")]
        public bool NeedsPayments { get; set; }

        [JsonPropertyName("needsPolicy")]
        public bool NeedsPolicy { get; set; }
    }
}
