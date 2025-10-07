using ManageEmployee.Entities;
using System.Text;
using System.Text.Json;
using System.Net.Http;

namespace ManageEmployee.Services
{
    public class GeminiService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        private const string ModelGeminiFlash = "gemini-1.5-flash-latest";
        private const string ModelGeminiPro = "gemini-pro";
        private const string ModelGeminiProVision = "gemini-pro-vision";

        public GeminiService(string apiKey, HttpClient httpClient)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> GenerateTextFromPrompt(string prompt)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{ModelGeminiFlash}:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[] { new { text = prompt } }
                    }
                }
            };

            return await SendRequestAndParseResponse(url, requestBody);
        }

        public async Task<string> GetChatResponseWithHistoryAsync(string newPrompt, List<ChatHistory> previousHistory)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{ModelGeminiFlash}:generateContent?key={_apiKey}";

            var contents = new List<object>();
            foreach (var history in previousHistory)
            {
                contents.Add(new { role = "user", parts = new[] { new { text = history.Prompt } } });
                contents.Add(new { role = "model", parts = new[] { new { text = history.Response } } });
            }

            contents.Add(new { role = "user", parts = new[] { new { text = newPrompt } } });

            var requestBody = new { contents };

            return await SendRequestAndParseResponse(url, requestBody);
        }

        public async Task<string> GenerateContentWithImageAsync(string prompt, byte[] imageBytes, string mimeType)
        {
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{ModelGeminiFlash}:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new object[]
                        {
                            new { text = prompt },
                            new
                            {
                                inline_data = new
                                {
                                    mime_type = mimeType,
                                    data = Convert.ToBase64String(imageBytes)
                                }
                            }
                        }
                    }
                }
            };

            return await SendRequestAndParseResponse(url, requestBody, "Gemini Vision API");
        }

        private async Task<string> SendRequestAndParseResponse(string url, object requestBody, string apiName = "Gemini API")
        {
            var jsonBody = JsonSerializer.Serialize(requestBody, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = $"{apiName} error: {response.StatusCode} - {jsonResponse}";
                Console.Error.WriteLine(errorMessage);
                throw new Exception(errorMessage);
            }

            using var doc = JsonDocument.Parse(jsonResponse);

            if (doc.RootElement.TryGetProperty("candidates", out var candidates) &&
                candidates.GetArrayLength() > 0 &&
                candidates[0].TryGetProperty("content", out var contentElement) &&
                contentElement.TryGetProperty("parts", out var parts) &&
                parts.GetArrayLength() > 0 &&
                parts[0].TryGetProperty("text", out var textElement))
            {
                return textElement.GetString() ?? "Không có phản hồi văn bản.";
            }

            if (doc.RootElement.TryGetProperty("promptFeedback", out var feedback) &&
                feedback.TryGetProperty("blockReason", out var reason))
            {
                var reasonMessage = $"Yêu cầu bị chặn vì lý do an toàn: {reason.GetString()}";
                Console.Error.WriteLine(reasonMessage);
                return reasonMessage;
            }

            var unexpectedStructureMessage = $"Unexpected {apiName} response structure: {jsonResponse}";
            Console.Error.WriteLine(unexpectedStructureMessage);
            throw new Exception(unexpectedStructureMessage);
        }
    }
}
