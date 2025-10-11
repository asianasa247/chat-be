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

        // 1) Start OAuth: tạo state + PKCE, redirect qua trang permission của Zalo
        //   GET https://ql.asianasa.com/api/zalo/oauth/start
        [HttpGet("start")]
        public IActionResult Start()
        {
            var appId = _cfg["Zalo:AppId"];
            var cb = Url.ActionLink(nameof(Callback), values: null)!; // -> https://ql.asianasa.com/api/zalo/oauth/callback

            if (string.IsNullOrWhiteSpace(appId))
                return BadRequest("Missing Zalo:AppId in appsettings.");

            // PKCE
            var state = Guid.NewGuid().ToString("N");
            var codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);

            // Lưu tạm code_verifier theo state (10 phút)
            _cache.Set("zalo.pkce." + state, codeVerifier, TimeSpan.FromMinutes(10));

            var permissionBase = _cfg["Zalo:PermissionEndpoint"] ?? "https://oauth.zaloapp.com/v4/oa/permission";
            var url = $"{permissionBase}?app_id={Uri.EscapeDataString(appId!)}" +
                      $"&redirect_uri={Uri.EscapeDataString(cb)}" +
                      $"&state={Uri.EscapeDataString(state)}" +
                      $"&code_challenge={Uri.EscapeDataString(codeChallenge)}" +
                      $"&code_challenge_method=S256";
            return Redirect(url);
        }

        // 2) OAuth callback: nhận code + state, đổi lấy access_token + refresh_token rồi lưu vào Data/zalo_tokens.json
        //   GET https://ql.asianasa.com/api/zalo/oauth/callback?code=...&state=...
        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string? code, [FromQuery] string? state, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
                return Content("Missing code/state.");

            if (!_cache.TryGetValue("zalo.pkce." + state, out string? codeVerifier) || string.IsNullOrWhiteSpace(codeVerifier))
                return Content("State expired or invalid. Please /api/zalo/oauth/start again.");

            var appId = _cfg["Zalo:AppId"];
            var appSecret = _cfg["Zalo:AppSecret"];
            var tokenUrl = _cfg["Zalo:OAuthRefreshUrl"] ?? "https://oauth.zaloapp.com/v4/oa/access_token";

            if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(appSecret))
                return Content("Missing AppId/AppSecret in appsettings.");

            using var client = _http.CreateClient();
            // Theo hướng dẫn thực tế: header 'secret_key' và body form (grant_type=authorization_code, app_id, code, code_verifier)
            // (Một số tài liệu cũ truyền secret trong body; bản này dùng header 'secret_key')  :contentReference[oaicite:1]{index=1}
            client.DefaultRequestHeaders.Remove("secret_key");
            client.DefaultRequestHeaders.Add("secret_key", appSecret);

            var body = new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["app_id"] = appId!,
                ["code"] = code!,
                ["code_verifier"] = codeVerifier!
            };

            var res = await client.PostAsync(tokenUrl, new FormUrlEncodedContent(body), ct);
            var text = await res.Content.ReadAsStringAsync(ct);

            if (!res.IsSuccessStatusCode)
            {
                _log.LogWarning("Zalo token exchange failed: {Status} {Body}", res.StatusCode, text);
                return Content($"Exchange failed: {(int)res.StatusCode}\n{text}");
            }

            try
            {
                using var doc = JsonDocument.Parse(text);
                var root = doc.RootElement;
                var access = root.GetProperty("access_token").GetString();
                var refresh = root.TryGetProperty("refresh_token", out var r) ? r.GetString() : null;
                var expiresIn = root.TryGetProperty("expires_in", out var e) ? e.GetInt32() : 3600;
                if (string.IsNullOrWhiteSpace(access)) return Content("Token exchange OK but access_token missing.");

                await _tokenStore.SaveAsync(new ManageEmployee.Entities.Chatbot.ZaloTokens
                {
                    AccessToken = access!,
                    RefreshToken = refresh,
                    ExpiresAt = DateTimeOffset.UtcNow.AddSeconds(expiresIn)
                }, ct);

                return Content("✅ Zalo OA token stored. You can close this tab.");
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Parse token response error: {Body}", text);
                return Content("Token saved failed: " + ex.Message + "\n" + text);
            }
        }

        // === Helpers ===
        private static string GenerateCodeVerifier()
        {
            // 43–128 chars, URL-safe
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
