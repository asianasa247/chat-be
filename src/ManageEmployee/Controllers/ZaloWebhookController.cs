using ManageEmployee.DataTransferObject.Chatbot;
using ManageEmployee.Services.Interfaces.Chatbot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers
{
    [AllowAnonymous]
    [Route("api/zalo/webhook")]
    [ApiController]
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

        [HttpGet]
        public IActionResult Verify([FromQuery(Name = "verify_token")] string? token)
        {
            var expected = _cfg["Zalo:VerifyToken"];
            if (!string.IsNullOrEmpty(expected) && token == expected) return Ok("OK");
            return Unauthorized();
        }

        [HttpPost]
        public async Task<IActionResult> Receive([FromBody] ZaloWebhookPayload payload, CancellationToken ct)
        {
            try
            {
                if (payload?.EventName != "user_send_text")
                    return Ok(new { ok = true });

                var userId = payload.Sender?.Id;
                var text = payload.Message?.Text ?? "";

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
                return Ok(new { ok = false, error = ex.Message });
            }
        }
    }
}
