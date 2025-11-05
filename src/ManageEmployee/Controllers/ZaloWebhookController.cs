using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ManageEmployee.Services.Interfaces.Chatbot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/zalo/webhook")]
    public class ZaloWebhookController : ControllerBase
    {
        private readonly IZaloApiService _zalo;
        private readonly IZaloChatbotService _chatbot;
        private readonly ISubscribersStore _subs;
        private readonly IConfiguration _cfg;
        private readonly ILogger<ZaloWebhookController> _log;

        public ZaloWebhookController(
            IZaloApiService zalo,
            IZaloChatbotService chatbot,
            ISubscribersStore subs,
            IConfiguration cfg,
            ILogger<ZaloWebhookController> log)
        {
            _zalo = zalo; _chatbot = chatbot; _subs = subs; _cfg = cfg; _log = log;
        }

        // NHẬN TẤT CẢ VERB để tránh 405 khi Zalo "Kiểm tra"
        [HttpGet, HttpPost, HttpHead, HttpOptions]
        public async Task<IActionResult> Entry(string appCode, CancellationToken ct)
        {
            var method = HttpContext.Request.Method;

            if (method == HttpMethods.Head || method == HttpMethods.Options)
                return Ok();

            if (method == HttpMethods.Get)
            {
                var token = Request.Query["verify_token"].ToString();
                var expected = _cfg["Zalo:VerifyToken"]; // Có thể dùng chung giữa các app
                var ok = (!string.IsNullOrEmpty(expected) && token == expected);
                _log.LogInformation("Zalo webhook GET verify_token={Token}, app={App} => {Ok}", token, appCode, ok);
                return ok ? Ok("OK") : Unauthorized();
            }

            if (Request.ContentLength == 0)
            {
                _log.LogWarning("Zalo webhook POST empty body (app={App}).", appCode);
                return Ok(new { ok = true });
            }

            string raw;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                raw = await reader.ReadToEndAsync();

            // (Optional) verify signature theo AppSecret của appCode
            try
            {
                var sigHeader = Request.Headers["X-ZEvent-Signature"].ToString();
                if (!string.IsNullOrWhiteSpace(sigHeader))
                {
                    var appSecret = _cfg.GetSection("Zalo:Apps").GetChildren()
                        .FirstOrDefault(s => string.Equals(s["Code"], appCode, StringComparison.OrdinalIgnoreCase))?["AppSecret"]
                        ?? _cfg["Zalo:AppSecret"];

                    if (!string.IsNullOrWhiteSpace(appSecret))
                    {
                        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret));
                        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(raw));
                        var hex = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

                        var headerMac = sigHeader.StartsWith("mac=", StringComparison.OrdinalIgnoreCase)
                            ? sigHeader.Substring(4)
                            : sigHeader;

                        var ok = string.Equals(hex, headerMac, StringComparison.OrdinalIgnoreCase);
                        if (!ok) _log.LogWarning("Signature mismatch (app={App}). header={Header} computed={Hex}", appCode, sigHeader, hex);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Signature verify error (non-blocking, app={App}).", appCode);
            }

            try
            {
                using var doc = JsonDocument.Parse(raw);
                var root = doc.RootElement;

                var eventName = root.TryGetProperty("event_name", out var ev) ? ev.GetString() : null;
                var senderId = root.TryGetProperty("sender", out var s) && s.TryGetProperty("id", out var sid) ? sid.GetString() : null;

                string? text = null;
                if (root.TryGetProperty("message", out var msg))
                {
                    if (msg.TryGetProperty("text", out var t)) text = t.GetString();
                }

                _log.LogInformation("Webhook POST app={App} event={Event} sender={Sender} text={Text}", appCode, eventName, senderId, text);

                if (string.IsNullOrWhiteSpace(eventName) || string.IsNullOrWhiteSpace(senderId))
                    return Ok(new { ok = true });

                if (!eventName.StartsWith("user_send_", StringComparison.OrdinalIgnoreCase))
                    return Ok(new { ok = true });

                await _subs.AddAsync(appCode, senderId, ct);

                if (string.IsNullOrWhiteSpace(text))
                {
                    await SafeSendAsync(appCode, senderId, "Mình chưa đọc được loại tin nhắn này. Bạn thử gửi câu hỏi bằng chữ nhé.", ct);
                    return Ok(new { ok = true });
                }

                var reply = await _chatbot.BuildReplyAsync(senderId, text, ct); // logic QA giữ nguyên
                await SafeSendAsync(appCode, senderId, reply, ct);

                return Ok(new { ok = true });
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Webhook POST error (app={App}). Raw={Raw}", appCode, raw);
                return Ok(new { ok = false, error = ex.Message });
            }
        }

        // ====== REPLACE SafeSendAsync để nhận appCode ======
        private async Task SafeSendAsync(string appCode, string userId, string message, CancellationToken ct)
        {
            try
            {
                await _zalo.SendTextAsync(appCode, userId, message, ct);
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "SendTextAsync failed for {User} (app={App})", userId, appCode);
            }
        }
    }

}