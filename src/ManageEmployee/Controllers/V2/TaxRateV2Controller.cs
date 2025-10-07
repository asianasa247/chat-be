using ManageEmployee.DataTransferObject;
using ManageEmployee.Services.Interfaces.Ledgers.V2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.V2;

[Route("api/v2/tax-rate")]
[ApiController]
[Authorize]

public class TaxRateV2Controller : ControllerBase
{
    private readonly ITaxRateV2Service _taxRateV2Service;
    public TaxRateV2Controller(ITaxRateV2Service taxRateV2Service) 
    {
        _taxRateV2Service = taxRateV2Service;
    }
    [HttpGet]
    public async Task<IActionResult> GetListData([FromHeader] int yearFilter)
    {
        var data = await _taxRateV2Service.GetAll(yearFilter);
        return Ok(new ObjectReturn
        {
            data = data,
        });
    }
}
