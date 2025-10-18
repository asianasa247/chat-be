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

        public ZaloDebugController(IZaloApiService zalo) { _zalo = zalo; }

        // GET /api/zalo/debug/send?uid=xxx&text=Hello
        [HttpGet("send")]
        public async Task<IActionResult> Send([FromQuery] string uid, [FromQuery] string? text = null, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(uid)) return BadRequest("Missing uid");
            await _zalo.SendTextAsync(uid, text ?? "PING từ hệ thống.", ct);
            return Ok(new { ok = true });
        }
    }
}
