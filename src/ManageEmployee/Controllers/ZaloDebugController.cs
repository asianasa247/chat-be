using ManageEmployee.Services.Interfaces.Chatbot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/zalo/debug")]
    public class ZaloDebugController : ControllerBase
    {
        private readonly IZaloApiService _zalo;
        private readonly IConfiguration _cfg;
        private readonly ILogger<ZaloDebugController> _log;

        public ZaloDebugController(IZaloApiService zalo, IConfiguration cfg, ILogger<ZaloDebugController> log)
        {
            _zalo = zalo;
            _cfg = cfg;
            _log = log;
        }

        // GET /api/zalo/debug/send?uid=xxx&text=Hello&app=app1
        // [CHANGED] hỗ trợ đa-app: thêm tham số "app" (appCode). Giữ nguyên tham số cũ, không phá backward.
        [HttpGet("send")]
        public async Task<IActionResult> Send(
            [FromQuery] string uid,
            [FromQuery] string? text = null,
            [FromQuery(Name = "app")] string? appCode = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(uid)) return BadRequest("Missing uid");

            // Resolve appCode: ưu tiên query ?app=..., sau đó Zalo:SchedulerAppCode, rồi app đầu tiên trong Zalo:Apps, cuối cùng 'default'
            var code = ResolveAppCode(appCode);
            _log.LogInformation("Debug send -> app={App}, uid={Uid}", code, uid);

            await _zalo.SendTextAsync(code, uid, text ?? "PING từ hệ thống.", ct);
            return Ok(new { ok = true, app = code });
        }

        // [NEW] route bổ sung: /api/zalo/debug/{appCode}/send?uid=xxx&text=Hello
        // Thuận tiện test nhanh từng app qua URL rõ ràng.
        [HttpGet("{appCode}/send")]
        public async Task<IActionResult> SendByPath(
            [FromRoute] string appCode,
            [FromQuery] string uid,
            [FromQuery] string? text = null,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(uid)) return BadRequest("Missing uid");
            await _zalo.SendTextAsync(appCode, uid, text ?? "PING từ hệ thống.", ct);
            return Ok(new { ok = true, app = appCode });
        }

        // ===== helpers =====
        private string ResolveAppCode(string? fromQuery)
        {
            if (!string.IsNullOrWhiteSpace(fromQuery)) return fromQuery;

            var scheduler = _cfg["Zalo:SchedulerAppCode"];
            if (!string.IsNullOrWhiteSpace(scheduler)) return scheduler!;

            var apps = _cfg.GetSection("Zalo:Apps").GetChildren();
            var first = apps.FirstOrDefault();
            if (first != null && !string.IsNullOrWhiteSpace(first["Code"])) return first["Code"]!;

            // Backward-compatible single-app
            return "default";
        }
    }
}
