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
                return (!string.IsNullOrEmpty(expected) && token == expected) ? Ok("OK") : Unauthorized();
            }

            // 3) POST: có thể rỗng khi "Kiểm tra" => vẫn 200
            if (method == HttpMethods.Post)
            {
                try
                {
                    if (Request.ContentLength == 0)
                        return Ok(new { ok = true });

                    string body;
                    using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                        body = await reader.ReadToEndAsync(); // KHÔNG truyền CancellationToken để tránh CS1501

                    if (string.IsNullOrWhiteSpace(body))
                        return Ok(new { ok = true });

                    ZaloWebhookPayload? payload = null;
                    try
                    {
                        payload = JsonSerializer.Deserialize<ZaloWebhookPayload>(body,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    }
                    catch { /* body có thể không phải JSON, bỏ qua */ }

                    if (payload == null || !string.Equals(payload.EventName, "user_send_text", StringComparison.OrdinalIgnoreCase))
                        return Ok(new { ok = true });

                    var userId = payload.Sender?.Id;
                    var text = payload.Message?.Text ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(userId))
                        return Ok(new { ok = true });

                    await _subs.AddAsync(userId, ct);
                    var reply = await _chatbot.BuildReplyAsync(text, ct);
                    await _zalo.SendTextAsync(userId, reply, ct);

                    return Ok(new { ok = true });
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, "Webhook POST error");
                    // vẫn trả 200 để Zalo không đánh fail webhook
                    return Ok(new { ok = false, error = ex.Message });
                }
            }

            // fallback
            return Ok();
        }
    }
}
