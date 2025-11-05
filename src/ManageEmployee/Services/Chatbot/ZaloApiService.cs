using System.Net;
using System.Text;
using System.Text.Json;
using ManageEmployee.Services.Interfaces.Chatbot;

namespace ManageEmployee.Services.Chatbot
{
    /// <summary>
    /// Zalo OA client đa-app.
    /// - Đọc AppId/AppSecret theo appCode từ cấu hình Zalo:Apps (fallback keys cũ nếu thiếu).
    /// - Token & Subscribers tách file theo appCode (thông qua ITokenStore).
    /// - Backward compatible: nếu không tìm thấy appCode -> dùng cấu hình đơn lẻ cũ.
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
            _sendTextPath = _cfg["Zalo:SendTextPath"] ?? "/v3.0/oa/message/cs";
            if (_sendTextPath.EndsWith("/oa/message", StringComparison.OrdinalIgnoreCase))
            {
                _log.LogWarning("Zalo:SendTextPath='{Path}' -> tự động dùng '/oa/message/cs'.", _sendTextPath);
                _sendTextPath = _sendTextPath + "/cs";
            }
        }

        // ===== Public API =====================================================

        public async Task SendTextAsync(string appCode, string userId, string text, CancellationToken ct = default)
        {
            var token = await EnsureAccessTokenCoreAsync(appCode, ct);
            var ok = await TrySendAsync(token, userId, text, ct);
            if (ok) return;

            _log.LogInformation("Send failed. Trying refresh then retry for appCode={App}...", appCode);
            var refreshed = await RefreshAccessTokenAsync(appCode, ct);
            if (!refreshed) throw new("Zalo sendText failed and refresh unsuccessful.");

            var token2 = await EnsureAccessTokenCoreAsync(appCode, ct);
            var ok2 = await TrySendAsync(token2, userId, text, ct);
            if (!ok2) throw new("Zalo sendText failed after refresh.");
        }

        public async Task EnsureAccessTokenAsync(string appCode, CancellationToken ct = default)
        {
            _ = await EnsureAccessTokenCoreAsync(appCode, ct);
        }

        // ===== Private helpers ===============================================

        private sealed record AppCfg(string Code, string? AppId, string? AppSecret);

        private AppCfg GetAppCfg(string appCode)
        {
            // Tìm trong Zalo:Apps
            var apps = _cfg.GetSection("Zalo:Apps").GetChildren();
            foreach (var a in apps)
            {
                var code = a["Code"];
                if (!string.IsNullOrWhiteSpace(code) &&
                    code.Equals(appCode, StringComparison.OrdinalIgnoreCase))
                {
                    return new AppCfg(
                        code,
                        a["AppId"] ?? _cfg["Zalo:AppId"],
                        a["AppSecret"] ?? _cfg["Zalo:AppSecret"]);
                }
            }
            // Fallback: single-app cũ
            return new AppCfg(appCode, _cfg["Zalo:AppId"], _cfg["Zalo:AppSecret"]);
        }

        private async Task<string> EnsureAccessTokenCoreAsync(string appCode, CancellationToken ct)
        {
            var t = await _tokens.LoadAsync(appCode, ct)
                    ?? throw new($"OA token missing for app '{appCode}'. Fill Zalo:Apps[*].TokensFile.");

            if (_tokens.IsExpired(t))
            {
                _log.LogInformation("Zalo token expired (app={App}). Trying to refresh...", appCode);
                var ok = await RefreshAccessTokenAsync(appCode, ct);
                if (!ok) throw new($"OA token expired & refresh failed (app={appCode}).");
                t = await _tokens.LoadAsync(appCode, ct) ?? throw new("Reload token failed after refresh.");
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

        private async Task<bool> RefreshAccessTokenAsync(string appCode, CancellationToken ct)
        {
            var app = GetAppCfg(appCode);

            var url = _cfg["Zalo:OAuthRefreshUrl"];
            var appId = app.AppId;
            var appSecret = app.AppSecret;

            var current = await _tokens.LoadAsync(appCode, ct);
            var refresh = current?.RefreshToken;

            if (string.IsNullOrWhiteSpace(url) ||
                string.IsNullOrWhiteSpace(appId) ||
                string.IsNullOrWhiteSpace(appSecret) ||
                string.IsNullOrWhiteSpace(refresh))
            {
                _log.LogWarning("Refresh not configured or missing refresh_token (app={App}).", appCode);
                return false;
            }

            using var client = _http.CreateClient();
            client.DefaultRequestHeaders.Remove("secret_key");
            client.DefaultRequestHeaders.Add("secret_key", appSecret);

            var form = new Dictionary<string, string>
            {
                ["app_id"] = appId!,
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
                _log.LogWarning(ex, "Refresh request failed (app={App}).", appCode);
                return false;
            }

            var body = await res.Content.ReadAsStringAsync(ct);
            if (!res.IsSuccessStatusCode)
            {
                _log.LogWarning("Refresh failed (app={App}): {Status} {Body}", appCode, res.StatusCode, body);
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
                    _log.LogWarning("Refresh OK but access_token not found (app={App}).", appCode);
                    return false;
                }

                var updated = new ManageEmployee.Entities.Chatbot.ZaloTokens
                {
                    AccessToken = newAccess!,
                    RefreshToken = newRefresh,
                    ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresInSec)
                };
                await _tokens.SaveAsync(appCode, updated, ct);
                _log.LogInformation("Zalo token refreshed (app={App}). Expires at {ExpiresAt:u}", appCode, updated.ExpiresAt);
                return true;
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Refresh parse failed (app={App}). Body: {Body}", appCode, body);
                return false;
            }
        }

        private async Task<bool> TrySendAsync(string token, string userId, string text, CancellationToken ct)
        {
            var baseTrimmed = _apiBase.TrimEnd('/');
            var path = _sendTextPath.StartsWith('/') ? _sendTextPath : "/" + _sendTextPath;
            var url = $"{baseTrimmed}{path}";

            using var req = new HttpRequestMessage(HttpMethod.Post, url);

            // OA v3: access_token ở HEADER
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

            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                var isSuccess =
                    (root.TryGetProperty("error", out var errEl) && ReadErrorCode(errEl) == 0) ||
                    root.TryGetProperty("message_id", out _) ||
                    (root.TryGetProperty("data", out var dataEl) && dataEl.TryGetProperty("message_id", out _));

                if (isSuccess) return true;

                var errCode = root.TryGetProperty("error", out var e) ? ReadErrorCode(e) : -1;
                var errMsg = root.TryGetProperty("message", out var m) ? m.GetString() : "(no message)";
                _log.LogWarning("Zalo send returned error={Code}, message={Msg}. Body: {Body}", errCode, errMsg, body);

                if (errCode is 401 or 403 or -201) return false;
                throw new("Zalo sendText failed (API returned error).");
            }
            catch (JsonException)
            {
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
