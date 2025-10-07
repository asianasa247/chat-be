using ManageEmployee.DataTransferObject.LedgerModels;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Ledgers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.LedgerControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LedgerProducesController : ControllerBase
{
    private readonly ILedgerProduceService _ledgerProduceService;

    public LedgerProducesController(ILedgerProduceService ledgerProduceService)
    {
        _ledgerProduceService = ledgerProduceService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromHeader] int yearFilter, [FromBody] LedgerProduceModel request)
    {
        var identityUser = HttpContext.GetIdentityUser();

        await _ledgerProduceService.AddProduce(request, identityUser.Id, yearFilter);
        return Ok();
    }
}