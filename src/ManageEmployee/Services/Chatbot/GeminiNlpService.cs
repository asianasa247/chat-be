using System.Text;
using System.Text.Json;
using ManageEmployee.Services.Interfaces.Chatbot;

namespace ManageEmployee.Services.Chatbot
{
    // Gọi Gemini (free tier) qua Generative Language API
    public sealed class GeminiNlpService : IGeminiNlpService
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _cfg;
        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public GeminiNlpService(IHttpClientFactory http, IConfiguration cfg) { _http = http; _cfg = cfg; }

        public async Task<string?> GenerateAsync(string prompt, CancellationToken ct = default)
        {
            var key = _cfg["Gemini:ApiKey"];
            if (string.IsNullOrWhiteSpace(key)) return null;

            var model = _cfg["Gemini:Model"] ?? "gemini-1.5-flash";
            var endpoint = (_cfg["Gemini:Endpoint"] ?? "https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent")
                           .Replace("{model}", model);

            using var req = new HttpRequestMessage(HttpMethod.Post, $"{endpoint}?key={key}");
            var payload = new { contents = new[] { new { parts = new[] { new { text = prompt } } } } };
            req.Content = new StringContent(JsonSerializer.Serialize(payload, _json), Encoding.UTF8, "application/json");

            using var client = _http.CreateClient();
            using var res = await client.SendAsync(req, ct);
            if (!res.IsSuccessStatusCode) return null;

            using var stream = await res.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);
            try
            {
                return doc.RootElement.GetProperty("candidates")[0]
                    .GetProperty("content").GetProperty("parts")[0]
                    .GetProperty("text").GetString();
            }
            catch { return null; }
        }
    }
}
