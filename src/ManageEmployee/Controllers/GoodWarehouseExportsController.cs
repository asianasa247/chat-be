using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Goods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class GoodWarehouseExportsController : ControllerBase
{
    private IGoodWarehouseExportService _goodWarehouseExportsService;

    public GoodWarehouseExportsController(
        IGoodWarehouseExportService goodWarehouseExportsService)
    {
        _goodWarehouseExportsService = goodWarehouseExportsService;
    }

    [HttpGet()]
    public IActionResult GetAll([FromQuery] GoodWarehouseExportRequestModel param)
    {
        var result = _goodWarehouseExportsService.GetAll(param);
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }
    [HttpDelete("{billId}")]
    public async Task<IActionResult> Delete(int billId)
    {
        var result = await _goodWarehouseExportsService.Delete(billId);
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }
}
