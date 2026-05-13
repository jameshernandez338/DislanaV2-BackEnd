using Dislana.Domain.ChatAssistant.Entities;
using Dislana.Domain.ChatAssistant.Interfaces;
using Dislana.Domain.ChatAssistant.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;

namespace Dislana.Infrastructure.ChatAssistant.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly OpenAISettings _settings;

        public OpenAIService(HttpClient httpClient, IOptions<OpenAISettings> settings)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
        }

        public async Task<string> SendMessageAsync(ChatSessionEntity session, string systemPrompt, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            {
                throw new InvalidOperationException("OpenAI API Key no configurada. Configure en User Secrets.");
            }

            var messages = new List<object>
            {
                new { role = "system", content = systemPrompt }
            };
            messages.AddRange(session.History.Select(h => new { role = h.Role, content = h.Content }));

            var requestBody = new
            {
                model = _settings.Model,
                max_tokens = _settings.MaxTokens,
                messages
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
            {
                Content = JsonContent.Create(requestBody)
            };
            request.Headers.Add("Authorization", $"Bearer {_settings.ApiKey}");

            var response = await _httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                throw new HttpRequestException($"OpenAI API error: {response.StatusCode} - {errorContent}");
            }

            var jsonResponse = await response.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: cancellationToken);
            var responseText = jsonResponse.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;

            return responseText;
        }
    }
}

