using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ManageEmployee.Services.Interfaces.Chatbot;

namespace ManageEmployee.Services.Chatbot
{
    // Gửi tin nhắn OA. LƯU Ý: phải có AccessToken hợp lệ trong file tokens.
    public sealed class ZaloApiService : IZaloApiService
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _cfg;
        private readonly ITokenStore _tokens;
        private readonly ILogger<ZaloApiService> _log;

        private readonly string _apiBase;
        private readonly string _sendTextPath;

        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public ZaloApiService(IHttpClientFactory http, IConfiguration cfg, ITokenStore tokens, ILogger<ZaloApiService> log)
        {
            _http = http; _cfg = cfg; _tokens = tokens; _log = log;
            _apiBase = _cfg["Zalo:ApiBase"] ?? throw new("Missing Zalo:ApiBase");
            _sendTextPath = _cfg["Zalo:SendTextPath"] ?? "/v3.0/oa/message";
        }

        private async Task<string> EnsureAccessTokenAsync(CancellationToken ct)
        {
            var t = await _tokens.LoadAsync(ct) ?? throw new("OA token missing");
            if (_tokens.IsExpired(t)) throw new("OA token expired – hãy refresh/cập nhật file tokens");
            return t.AccessToken;
        }

        public async Task SendTextAsync(string userId, string text, CancellationToken ct = default)
        {
            var token = await EnsureAccessTokenAsync(ct);
            var url = $"{_apiBase.TrimEnd('/')}{_sendTextPath}";

            using var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Body theo spec OA (tuỳ version có thể khác)
            var payload = new
            {
                recipient = new { user_id = userId },
                message = new { text }
            };
            req.Content = new StringContent(JsonSerializer.Serialize(payload, _json), Encoding.UTF8, "application/json");

            using var client = _http.CreateClient();
            using var res = await client.SendAsync(req, ct);
            var body = await res.Content.ReadAsStringAsync(ct);

            if (!res.IsSuccessStatusCode)
            {
                _log.LogWarning("Zalo sendText fail {Status} {Body}", res.StatusCode, body);
                throw new($"Zalo sendText failed: {(int)res.StatusCode}");
            }
        }
    }
}
