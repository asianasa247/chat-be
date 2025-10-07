using ManageEmployee.DataTransferObject.Payments;
using ManageEmployee.Services.Interfaces.PaymentServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.PaymentControllers
{
    [ApiController]
    [Route("api/payments/alepay")]
    [Authorize]
    public class AlepayController : ControllerBase
    {
        private readonly IAlepayService _service;
        public AlepayController(IAlepayService service) => _service = service;

        [HttpPost("checkout")]
        public async Task<ActionResult<AlepayCheckoutResponse>> CreateCheckout([FromBody] AlepayCheckoutRequest req, CancellationToken ct)
            => Ok(await _service.CreateCheckoutAsync(req, ct));

        [AllowAnonymous]
        [HttpGet("return")]
        public async Task<IActionResult> Return([FromQuery] AlepayReturnQuery q, CancellationToken ct)
        {
            var tx = await _service.HandleReturnAsync(q, ct);
            return Ok(new { ok = tx != null, status = tx?.Status, transactionCode = tx?.TransactionCode, orderCode = tx?.OrderCode });
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] AlepayWebhookDto dto, CancellationToken ct)
            => Ok(new { success = await _service.HandleWebhookAsync(dto, ct) });

        [HttpPost("sync/{transactionCode}")]
        public async Task<IActionResult> Sync(string transactionCode, CancellationToken ct)
        {
            var tx = await _service.SyncTransactionInfoAsync(transactionCode, ct);
            return Ok(new { ok = tx != null, status = tx?.Status });
        }
    }
}
