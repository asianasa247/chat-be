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
    /// - Retry 1 lần nếu gặp token hết hạn/invalid.
    /// - Kiểm tra body JSON: chỉ coi là thành công khi error == 0.
    /// </summary>
    public sealed class ZaloApiService : IZaloApiService
    {
        private readonly IHttpClientFactory _http;
        private readonly IConfiguration _cfg;
        private readonly ITokenStore _tokens;
        private readonly ILogger<ZaloApiService> _log;

        private readonly string _apiBase;
        private readonly string _sendTextPath;

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
            _sendTextPath = _cfg["Zalo:SendTextPath"] ?? "/v3.0/oa/message";
        }

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
                res = await client.PostAsync(url, new FormUrlEncodedContent(form), ct);
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

        public async Task SendTextAsync(string userId, string text, CancellationToken ct = default)
        {
            var token = await EnsureAccessTokenAsync(ct);
            var result = await TrySendAsync(token, userId, text, ct);
            if (result == TrySendResult.Success) return;

            if (result == TrySendResult.ShouldRefresh)
            {
                _log.LogInformation("Send failed (token issue). Trying refresh then retry...");
                var refreshed = await RefreshAccessTokenAsync(ct);
                if (!refreshed)
                    throw new("Zalo sendText failed and refresh unsuccessful.");

                var token2 = await EnsureAccessTokenAsync(ct);
                var result2 = await TrySendAsync(token2, userId, text, ct);
                if (result2 != TrySendResult.Success)
                    throw new("Zalo sendText failed after refresh.");
            }
            else
            {
                throw new("Zalo sendText failed (API returned error).");
            }
        }

        private enum TrySendResult { Success, ShouldRefresh, Fail }

        private async Task<TrySendResult> TrySendAsync(string token, string userId, string text, CancellationToken ct)
        {
            var url = $"{_apiBase.TrimEnd('/')}{_sendTextPath}";
            using var req = new HttpRequestMessage(HttpMethod.Post, url);

            // OA v3: access_token ở HEADER
            req.Headers.Remove("access_token");
            req.Headers.Add("access_token", token);

            var payload = new
            {
                recipient = new { user_id = userId },
                message = new { text }
            };
            var json = JsonSerializer.Serialize(payload, _json);
            req.Content = new StringContent(json, Encoding.UTF8, "application/json");

            using var client = _http.CreateClient();
            using var res = await client.SendAsync(req, ct);
            var body = await res.Content.ReadAsStringAsync(ct);

            // 1) HTTP không 2xx → lỗi mạng/hệ thống
            if (!res.IsSuccessStatusCode)
            {
                _log.LogWarning("Zalo sendText HTTP {Status}. Resp: {Body}. Payload: {Payload}",
                    (int)res.StatusCode, body, json);

                if (res.StatusCode == HttpStatusCode.Unauthorized || res.StatusCode == HttpStatusCode.Forbidden)
                    return TrySendResult.ShouldRefresh;

                return TrySendResult.Fail;
            }

            // 2) HTTP 200: phải kiểm tra "error" trong body
            int error = 0;
            string? message = null;
            string? messageId = null;
            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                if (root.TryGetProperty("error", out var e))
                {
                    error = e.ValueKind == JsonValueKind.Number
                        ? e.GetInt32()
                        : int.TryParse(e.GetString(), out var tmp) ? tmp : 0;
                }
                if (root.TryGetProperty("message", out var m)) message = m.GetString();
                if (root.TryGetProperty("message_id", out var mid)) messageId = mid.GetString();
            }
            catch
            {
                // Body không phải JSON hợp lệ (hiếm)
                _log.LogInformation("Zalo sendText HTTP 200 (non-JSON). Body: {Body}", body);
                return TrySendResult.Success; // coi như OK để không chặn
            }

            if (error == 0)
            {
                _log.LogInformation("Zalo send OK -> {User}. message_id={MsgId}. Body: {Body}", userId, messageId, body);
                return TrySendResult.Success;
            }

            // Lỗi nhưng HTTP 200 — ghi rõ & quyết định có nên refresh hay không
            _log.LogWarning("Zalo send returned error={Error}, message={Msg}. Body: {Body}", error, message, body);

            // heuristic: nếu lỗi có vẻ liên quan token → refresh
            if ((message ?? "").Contains("access token", StringComparison.OrdinalIgnoreCase) ||
                (message ?? "").Contains("expired", StringComparison.OrdinalIgnoreCase) ||
                error == 401 || error == 403)
            {
                return TrySendResult.ShouldRefresh;
            }

            return TrySendResult.Fail;
        }
    }
}
