using ManageEmployee.DataTransferObject.LedgerWarehouseModels;
using ManageEmployee.Services.Interfaces.Ledgers.V2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.V2;

[ApiController]
[Authorize]
[Route("api/v2/ledgers")]
public class LedgersV2Controller : ControllerBase
{
    private readonly ILedgerV2Service _ledgerV2Service;
    public LedgersV2Controller(ILedgerV2Service ledgerV2Service) 
    {
        _ledgerV2Service = ledgerV2Service;
    }
    [HttpGet]
    public async Task<IActionResult> GetListData([FromQuery] LedgerRequestModel request, [FromHeader] int yearFilter)
    {
        var data = await _ledgerV2Service.GetPage(request, yearFilter);
        return Ok(data);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int id, [FromQuery] int isInternal, [FromHeader] int yearFilter)
    {
        var result = await _ledgerV2Service.GetByIdAsync(id, isInternal, yearFilter);
        if (result is null)
        {
            return BadRequest(new
            {
                message = "Can't not find account"
            });
        }
        return Ok(result);
    }

    [HttpGet("arising-for-origin-voucher-number")]
    public async Task<IActionResult> TotalArisingForVoucherNumber(string? orginalVoucherNumber, int isInternal = 1)
    {
        var result = await _ledgerV2Service.TotalArisingForVoucherNumber(orginalVoucherNumber, isInternal);
        return Ok(result);
    }
}