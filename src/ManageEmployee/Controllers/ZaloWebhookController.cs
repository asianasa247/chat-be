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

        // Zalo có thể gọi GET để verify hoặc HEAD khi bấm "Kiểm tra"
        [AcceptVerbs("GET", "HEAD")]
        public IActionResult Verify([FromQuery(Name = "verify_token")] string? token)
        {
            // Nếu là HEAD → luôn trả 200 để vượt qua "Kiểm tra"
            if (HttpContext.Request.Method == "HEAD") return Ok();

            var expected = _cfg["Zalo:VerifyToken"];
            if (!string.IsNullOrEmpty(expected) && token == expected) return Ok("OK");
            return Unauthorized();
        }

        // POST từ Zalo (có thể rỗng khi "Kiểm tra") → luôn trả 200
        [HttpPost]
        public async Task<IActionResult> Receive([FromBody] ZaloWebhookPayload? payload, CancellationToken ct)
        {
            try
            {
                // Trường hợp Zalo chỉ test webhook (body rỗng) → pass
                if (payload == null) return Ok(new { ok = true });

                if (!string.Equals(payload.EventName, "user_send_text", StringComparison.OrdinalIgnoreCase))
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
                _log.LogError(ex, "Webhook error");
                // vẫn trả 200 để Zalo không đánh fail webhook; xem log để xử lý
                return Ok(new { ok = false, error = ex.Message });
            }
        }
    }
}
