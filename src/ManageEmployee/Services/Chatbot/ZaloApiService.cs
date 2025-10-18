using System.Net;
using System.Text;
using System.Text.Json;
using ManageEmployee.Services.Interfaces.Chatbot;

namespace ManageEmployee.Services.Chatbot
{
    /// <summary>
    /// Client gửi tin nhắn OA Zalo.
    /// - Đọc access token từ ITokenStore (file JSON).
    /// - Tự động refresh khi hết hạn (nếu cấu hình đầy đủ).
    /// - Parse JSON response của Zalo (HTTP luôn 200, lỗi nằm trong trường "error").
    /// </summary>
    public sealed class ZaloApiService : IZaloApiService
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _cfg;
        private readonly ITokenStore _tokens;
        private readonly ILogger<ZaloApiService> _log;

        private readonly string _apiBase;
        private string _sendTextPath;

        private static readonly JsonSerializerOptions _json = new(JsonSerializerDefaults.Web);

        public ZaloApiService(
            IHttpClientFactory http,
            IConfiguration cfg,
            ITokenStore tokens,
            ILogger<ZaloApiService> log)
        {
            _http = http;
            _cfg = cfg;
            _tokens = tokens;
            _log = log;

            _apiBase = _cfg["Zalo:ApiBase"] ?? throw new("Missing Zalo:ApiBase");

            // V3: dùng /v3.0/oa/message/cs cho tin nhắn CSKH
            _sendTextPath = _cfg["Zalo:SendTextPath"] ?? "/v3.0/oa/message/cs";

            // Vá cấu hình cũ /v3.0/oa/message -> tự append /cs để tránh lỗi 404 trong body
            if (_sendTextPath.EndsWith("/oa/message", StringComparison.OrdinalIgnoreCase))
            {
                _log.LogWarning("Zalo:SendTextPath đang là '{Path}'. V3 yêu cầu '/oa/message/cs'. Tự động dùng '/oa/message/cs'.", _sendTextPath);
                _sendTextPath = _sendTextPath + "/cs";
            }
        }

        public async Task SendTextAsync(string userId, string text, CancellationToken ct = default)
        {
            var token = await EnsureAccessTokenAsync(ct);
            var ok = await TrySendAsync(token, userId, text, ct);
            if (ok) return;

            // Nếu fail do 401/403 → refresh và retry
            _log.LogInformation("Send failed or returned error. Trying refresh then retry...");
            var refreshed = await RefreshAccessTokenAsync(ct);
            if (!refreshed)
                throw new("Zalo sendText failed and refresh unsuccessful.");

            var token2 = await EnsureAccessTokenAsync(ct);
            var ok2 = await TrySendAsync(token2, userId, text, ct);
            if (!ok2)
                throw new("Zalo sendText failed after refresh.");
        }

        // ==== Private helpers ====

        private async Task<string> EnsureAccessTokenAsync(CancellationToken ct)
        {
            var t = await _tokens.LoadAsync(ct) ?? throw new("OA token missing (Data/zalo_tokens.json).");

            if (_tokens.IsExpired(t))
            {
                _log.LogInformation("Zalo token expired. Trying to refresh...");
                var ok = await RefreshAccessTokenAsync(ct);
                if (!ok)
                    throw new("OA token expired & refresh failed. Please update Data/zalo_tokens.json or configure refresh.");
                t = await _tokens.LoadAsync(ct) ?? throw new("Reload token failed after refresh.");
            }
            return t.AccessToken;
        }

        private static int ReadIntFlexible(JsonElement root, string prop, int @default)
        {
            if (!root.TryGetProperty(prop, out var el)) return @default;
            return el.ValueKind switch
            {
                JsonValueKind.Number => el.TryGetInt32(out var n) ? n : @default,
                JsonValueKind.String => int.TryParse(el.GetString(), out var s) ? s : @default,
                _ => @default
            };
        }

        private async Task<bool> RefreshAccessTokenAsync(CancellationToken ct)
        {
            var url = _cfg["Zalo:OAuthRefreshUrl"];
            var appId = _cfg["Zalo:AppId"];
            var appSecret = _cfg["Zalo:AppSecret"];

            var current = await _tokens.LoadAsync(ct);
            var refresh = current?.RefreshToken;

            if (string.IsNullOrWhiteSpace(url) ||
                string.IsNullOrWhiteSpace(appId) ||
                string.IsNullOrWhiteSpace(appSecret) ||
                string.IsNullOrWhiteSpace(refresh))
            {
                _log.LogWarning("Refresh token not configured or missing refresh_token. Skip refresh.");
                return false;
            }

            using var client = _http.CreateClient();
            client.DefaultRequestHeaders.Remove("secret_key");
            client.DefaultRequestHeaders.Add("secret_key", appSecret);

            var form = new Dictionary<string, string>
            {
                ["app_id"] = appId,
                ["refresh_token"] = refresh!,
                ["grant_type"] = "refresh_token"
            };

            HttpResponseMessage res;
            try
            {
                res = await client.PostAsync(url!, new FormUrlEncodedContent(form), ct);
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Refresh request failed.");
                return false;
            }

            var body = await res.Content.ReadAsStringAsync(ct);
            if (!res.IsSuccessStatusCode)
            {
                _log.LogWarning("Refresh failed: {Status} {Body}", res.StatusCode, body);
                return false;
            }

            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                var newAccess = root.TryGetProperty("access_token", out var at) ? at.GetString() : null;
                var newRefresh = root.TryGetProperty("refresh_token", out var rt) ? rt.GetString() : refresh;
                var expiresInSec = ReadIntFlexible(root, "expires_in", 3600);
                if (string.IsNullOrWhiteSpace(newAccess))
                {
                    _log.LogWarning("Refresh OK but access_token not found in response.");
                    return false;
                }

                var updated = new ManageEmployee.Entities.Chatbot.ZaloTokens
                {
                    AccessToken = newAccess!,
                    RefreshToken = newRefresh,
                    ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresInSec)
                };
                await _tokens.SaveAsync(updated, ct);
                _log.LogInformation("Zalo token refreshed. Expires at {ExpiresAt:u}", updated.ExpiresAt);
                return true;
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Refresh parse failed. Body: {Body}", body);
                return false;
            }
        }

        private async Task<bool> TrySendAsync(string token, string userId, string text, CancellationToken ct)
        {
            var baseTrimmed = _apiBase.TrimEnd('/');
            var path = _sendTextPath.StartsWith('/') ? _sendTextPath : "/" + _sendTextPath;
            var url = $"{baseTrimmed}{path}";

            using var req = new HttpRequestMessage(HttpMethod.Post, url);

            // OA v3: access_token ở HEADER (không phải query)
            req.Headers.Remove("access_token");
            req.Headers.Add("access_token", token);

            var payload = new
            {
                recipient = new { user_id = userId },
                message = new { text }
            };
            req.Content = new StringContent(JsonSerializer.Serialize(payload, _json), Encoding.UTF8, "application/json");

            using var client = _http.CreateClient();
            using var res = await client.SendAsync(req, ct);
            var body = await res.Content.ReadAsStringAsync(ct);

            // HTTP code của Zalo thường là 200, lỗi nằm trong JSON
            if (!res.IsSuccessStatusCode)
            {
                if (res.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
                {
                    _log.LogWarning("Zalo sendText HTTP {Status}. Body: {Body}", res.StatusCode, body);
                    return false; // để caller refresh + retry
                }

                _log.LogWarning("Zalo sendText HTTP {Status}. Body: {Body}", res.StatusCode, body);
                throw new($"Zalo sendText failed: {(int)res.StatusCode}");
            }

            // Parse JSON để kiểm tra trường "error"
            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                // success: error == 0 hoặc có message_id
                var isSuccess =
                    (root.TryGetProperty("error", out var errEl) && ReadErrorCode(errEl) == 0) ||
                    root.TryGetProperty("message_id", out _) ||
                    (root.TryGetProperty("data", out var dataEl) && dataEl.TryGetProperty("message_id", out _));

                if (isSuccess) return true;

                var errCode = root.TryGetProperty("error", out var e) ? ReadErrorCode(e) : -1;
                var errMsg = root.TryGetProperty("message", out var m) ? m.GetString() : "(no message)";
                _log.LogWarning("Zalo send returned error={Code}, message={Msg}. Body: {Body}", errCode, errMsg, body);

                // Trả về false để tầng trên thử refresh token (nếu là lỗi auth), hoặc ném exception
                if (errCode is 401 or 403 or -201) return false;
                throw new("Zalo sendText failed (API returned error).");
            }
            catch (JsonException)
            {
                // Không parse được JSON → nếu HTTP 200 mà body không chuẩn thì cứ coi là lỗi
                _log.LogWarning("Zalo send returned non-JSON body: {Body}", body);
                throw new("Zalo sendText failed: invalid JSON response.");
            }
        }

        private static int ReadErrorCode(JsonElement el)
        {
            return el.ValueKind switch
            {
                JsonValueKind.Number => el.TryGetInt32(out var n) ? n : -1,
                JsonValueKind.String => int.TryParse(el.GetString(), out var s) ? s : -1,
                _ => -1
            };
        }
    }
}
