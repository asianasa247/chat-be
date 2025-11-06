using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ManageEmployee.Services.Interfaces.Chatbot;

namespace ManageEmployee.Controllers
{
    [AllowAnonymous]
    [Route("api/zalo/oauth")]
    [ApiController]
    public class ZaloOAuthController : ControllerBase
    {
        private readonly IConfiguration _cfg;
        private readonly IMemoryCache _cache;
        private readonly IHttpClientFactory _http;
        private readonly ITokenStore _tokenStore;
        private readonly ILogger<ZaloOAuthController> _log;

        public ZaloOAuthController(
            IConfiguration cfg,
            IMemoryCache cache,
            IHttpClientFactory http,
            ITokenStore tokenStore,
            ILogger<ZaloOAuthController> log)
        {
            _cfg = cfg;
            _cache = cache;
            _http = http;
            _tokenStore = tokenStore;
            _log = log;
        }

        // =========================
        // MULTI-APP OAUTH FLOW
        // - Start:   GET /api/zalo/oauth/{appCode}/start
        // - Callback:GET /api/zalo/oauth/{appCode}/callback?code=...&state=...
        // Giữ nguyên logic PKCE + fallback legacy, nhưng tất cả đều theo từng appCode.
        // =========================

        // GET .../api/zalo/oauth/{appCode}/start
        [HttpGet("{appCode}/start")]
        public IActionResult Start([FromRoute] string appCode)
        {
            if (string.IsNullOrWhiteSpace(appCode)) return BadRequest("Missing appCode");

            var app = GetAppCfg(appCode);
            if (string.IsNullOrWhiteSpace(app.AppId))
                return BadRequest($"Missing AppId for appCode '{appCode}' in appsettings.");

            var callbackUrl = Url.ActionLink(nameof(Callback), values: new { appCode })!;
            var permissionBase = _cfg["Zalo:PermissionEndpoint"] ?? "https://oauth.zaloapp.com/v4/oa/permission";

            // PKCE
            var state = Guid.NewGuid().ToString("N");
            var codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);

            // Cache PKCE theo appCode để tránh va chạm khi nhiều app cùng authorize
            _cache.Set($"zalo.pkce.{appCode}.{state}", codeVerifier, TimeSpan.FromMinutes(10));

            var url = $"{permissionBase}?app_id={Uri.EscapeDataString(app.AppId!)}" +
                      $"&redirect_uri={Uri.EscapeDataString(callbackUrl)}" +
                      $"&state={Uri.EscapeDataString(state)}" +
                      $"&code_challenge={Uri.EscapeDataString(codeChallenge)}" +
                      $"&code_challenge_method=S256";
            return Redirect(url);
        }

        // GET .../api/zalo/oauth/{appCode}/callback?code=...&state=...
        // Một số luồng có thể không trả state -> fallback legacy (không PKCE).
        [HttpGet("{appCode}/callback")]
        public async Task<IActionResult> Callback(
            [FromRoute] string appCode,
            [FromQuery] string? code,
            [FromQuery] string? state,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(appCode)) return Content("Missing appCode.");
            if (string.IsNullOrWhiteSpace(code)) return Content("Missing code/state.");

            var app = GetAppCfg(appCode);
            var tokenUrl = _cfg["Zalo:OAuthRefreshUrl"] ?? "https://oauth.zaloapp.com/v4/oa/access_token";
            if (string.IsNullOrWhiteSpace(app.AppId) || string.IsNullOrWhiteSpace(app.AppSecret))
                return Content($"Missing AppId/AppSecret for appCode '{appCode}'.");

            using var client = _http.CreateClient();
            client.DefaultRequestHeaders.Remove("secret_key");
            client.DefaultRequestHeaders.Add("secret_key", app.AppSecret!);

            var form = new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["app_id"] = app.AppId!,
                ["code"] = code!
            };

            if (!string.IsNullOrWhiteSpace(state) &&
                _cache.TryGetValue($"zalo.pkce.{appCode}.{state}", out string? codeVerifier) &&
                !string.IsNullOrWhiteSpace(codeVerifier))
            {
                form["code_verifier"] = codeVerifier!;
            }
            else
            {
                _log.LogInformation("Zalo OAuth callback without usable state/PKCE (app={App}). Proceeding legacy exchange.", appCode);
            }

            var res = await client.PostAsync(tokenUrl, new FormUrlEncodedContent(form), ct);
            var text = await res.Content.ReadAsStringAsync(ct);
            if (!res.IsSuccessStatusCode)
            {
                _log.LogWarning("Zalo token exchange failed (app={App}): {Status} {Body}", appCode, res.StatusCode, text);
                return Content($"Exchange failed: {(int)res.StatusCode}\n{text}");
            }

            try
            {
                using var doc = JsonDocument.Parse(text);
                var root = doc.RootElement;

                var access = root.GetProperty("access_token").GetString();
                var refresh = root.TryGetProperty("refresh_token", out var r) ? r.GetString() : null;
                var expiresIn = ReadIntFlexible(root, "expires_in", 3600);

                if (string.IsNullOrWhiteSpace(access))
                    return Content("Token exchange OK but access_token missing.");

                await _tokenStore.SaveAsync(appCode, new ManageEmployee.Entities.Chatbot.ZaloTokens
                {
                    AccessToken = access!,
                    RefreshToken = refresh,
                    ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresIn)
                }, ct);

                return Content($"✅ Zalo OA token stored for app '{appCode}'. You can close this tab.");
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Parse token response error (app={App}). Body={Body}", appCode, text);
                return Content("Token save failed: " + ex.Message + "\n" + text);
            }
        }

        // ===== Helpers =====

        private sealed record AppCfg(string Code, string? AppId, string? AppSecret);

        private AppCfg GetAppCfg(string appCode)
        {
            // Tìm theo Zalo:Apps[*]
            var apps = _cfg.GetSection("Zalo:Apps").GetChildren();
            foreach (var a in apps)
            {
                var code = a["Code"];
                if (!string.IsNullOrWhiteSpace(code) &&
                    code.Equals(appCode, StringComparison.OrdinalIgnoreCase))
                {
                    // fallback vào cặp single-app nếu thiếu
                    return new AppCfg(
                        code,
                        a["AppId"] ?? _cfg["Zalo:AppId"],
                        a["AppSecret"] ?? _cfg["Zalo:AppSecret"]);
                }
            }
            // Backward-compatible: cho phép “default” chạy bằng key cũ
            return new AppCfg(appCode, _cfg["Zalo:AppId"], _cfg["Zalo:AppSecret"]);
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

        private static string GenerateCodeVerifier()
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            return Base64Url(bytes);
        }

        private static string GenerateCodeChallenge(string verifier)
        {
            using var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(verifier));
            return Base64Url(hash);
        }

        private static string Base64Url(ReadOnlySpan<byte> input)
            => Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }
}
