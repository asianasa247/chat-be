using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.V3;
using ManageEmployee.Services.Interfaces.Ledgers.V3;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.V3;

[ApiController]
[Authorize]
[Route("api/v3/ledgers")]
public class LedgersV3Controller : ControllerBase
{
    private readonly ILedgerV3Service _ledgerV3Service;

    public LedgersV3Controller(ILedgerV3Service ledgerV3Service) 
    {
        _ledgerV3Service = ledgerV3Service;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] List<LedgerV3UpdateModel> entities, [FromHeader] int yearFilter, bool isReverse)
    {
        await _ledgerV3Service.UpdateAsync(entities, yearFilter, isReverse);
        return Ok();
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetListData(long id ,[FromHeader] int yearFilter)
    {
        var response = await _ledgerV3Service.GetLedgerById(id, yearFilter);
        return Ok(new ObjectReturn
        {
            data = response,
            status = 200,
        });
    }
}