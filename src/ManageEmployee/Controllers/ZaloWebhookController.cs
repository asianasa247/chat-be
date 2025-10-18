using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ManageEmployee.DataTransferObject.Chatbot;
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
        public async Task<IActionResult> Entry(CancellationToken ct)
        {
            var method = HttpContext.Request.Method;

            // 1) HEAD/OPTIONS (Zalo có thể gọi khi "Kiểm tra") => luôn 200
            if (method == HttpMethods.Head || method == HttpMethods.Options)
                return Ok();

            // 2) GET verify_token => phải trùng với appsettings.json
            if (method == HttpMethods.Get)
            {
                var token = Request.Query["verify_token"].ToString();
                var expected = _cfg["Zalo:VerifyToken"];
                var ok = (!string.IsNullOrEmpty(expected) && token == expected);
                _log.LogInformation("Zalo webhook GET verify_token={Token} => {Ok}", token, ok);
                return ok ? Ok("OK") : Unauthorized();
            }

            // 3) POST: có thể rỗng khi "Kiểm tra" => vẫn 200
            if (Request.ContentLength == 0)
            {
                _log.LogWarning("Zalo webhook POST empty body.");
                return Ok(new { ok = true });
            }

            string raw;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                raw = await reader.ReadToEndAsync(); // KHÔNG truyền CancellationToken để tránh CS1501

            // (Optional) verify signature nếu header có — không chặn khi sai
            try
            {
                var sigHeader = Request.Headers["X-ZEvent-Signature"].ToString();
                if (!string.IsNullOrWhiteSpace(sigHeader))
                {
                    var appSecret = _cfg["Zalo:AppSecret"];
                    if (!string.IsNullOrWhiteSpace(appSecret))
                    {
                        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret));
                        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(raw));
                        var hex = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

                        var headerMac = sigHeader.StartsWith("mac=", StringComparison.OrdinalIgnoreCase)
                            ? sigHeader.Substring(4)
                            : sigHeader;

                        var ok = string.Equals(hex, headerMac, StringComparison.OrdinalIgnoreCase);
                        if (!ok) _log.LogWarning("Zalo signature mismatch. header={Header} computed={Hex}", sigHeader, hex);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Zalo signature verify error (non-blocking).");
            }

            try
            {
                // Parse linh hoạt (phủ cả user_send_text/image/link/sticker…)
                using var doc = JsonDocument.Parse(raw);
                var root = doc.RootElement;

                var eventName = root.TryGetProperty("event_name", out var ev)
                    ? ev.GetString() : null;

                var senderId = root.TryGetProperty("sender", out var s) && s.TryGetProperty("id", out var sid)
                    ? sid.GetString() : null;

                string? text = null;
                if (root.TryGetProperty("message", out var msg))
                {
                    if (msg.TryGetProperty("text", out var t)) text = t.GetString();
                }

                _log.LogInformation(
                    "Zalo webhook POST event={Event} sender={Sender} text={Text} body[200]={Body}",
                    eventName, senderId, text,
                    raw.Length > 200 ? raw.Substring(0, 200) + "..." : raw
                );

                if (string.IsNullOrWhiteSpace(eventName) || string.IsNullOrWhiteSpace(senderId))
                    return Ok(new { ok = true });

                // Chỉ xử lý nhóm user_send_* ; các sự kiện khác bỏ qua (follow, menu_click… tuỳ sau này)
                if (!eventName.StartsWith("user_send_", StringComparison.OrdinalIgnoreCase))
                    return Ok(new { ok = true });

                await _subs.AddAsync(senderId, ct);

                // Nếu không có text (sticker/image/link...) → nhắn nhủ người dùng gửi text
                if (string.IsNullOrWhiteSpace(text))
                {
                    await SafeSendAsync(senderId, "Mình chưa đọc được loại tin nhắn này. Bạn thử gửi câu hỏi bằng chữ nhé.", ct);
                    return Ok(new { ok = true });
                }

                var reply = await _chatbot.BuildReplyAsync(text, ct);
                await SafeSendAsync(senderId, reply, ct);

                return Ok(new { ok = true });
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Webhook POST error. Raw={Raw}", raw);
                // vẫn trả 200 để Zalo không đánh fail webhook
                return Ok(new { ok = false, error = ex.Message });
            }
        }

        private async Task SafeSendAsync(string userId, string message, CancellationToken ct)
        {
            try
            {
                await _zalo.SendTextAsync(userId, message, ct);
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "SendTextAsync failed for {User}", userId);
            }
        }
    }
}
