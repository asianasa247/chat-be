using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ManageEmployee.Services.Interfaces.Goods;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.SearchModels;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class GoodWarehousesController : ControllerBase
{
    private readonly IGoodWarehousesService _goodWarehousesService;

    public GoodWarehousesController(
        IGoodWarehousesService goodWarehousesService) 
    {
        _goodWarehousesService = goodWarehousesService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll([FromQuery] SearchViewModel param)
    {
        var result = await _goodWarehousesService.GetAll(param);
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }
    [HttpGet("sync-chartofaccount")]
    public async Task<IActionResult> SyncChartOfAccount([FromHeader] int yearFilter)
    {
        var result = await _goodWarehousesService.SyncChartOfAccount(yearFilter);
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }
    [HttpGet("sync-inventory")]
    public async Task<IActionResult> SyncTonKho([FromHeader] int yearFilter)
    {
        var result = await _goodWarehousesService.SyncTonKho(yearFilter);
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }
    [HttpPost("complete-bill/{isForce}")]
    public async Task<IActionResult> CompleteBill(List<BillDetailViewPaging> goodQRs, bool isForce)
    {
        var userId = 0;
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }
        var result = await _goodWarehousesService.CompleteBill(goodQRs, isForce, userId);
        return Ok(result);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] GoodWarehousesUpdateModel param)
    {
        await _goodWarehousesService.Update(param);
        return Ok();
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _goodWarehousesService.GetById(id);
        return Ok(result);
    }

    [HttpPost("update-good-printed-status")]
    public async Task<IActionResult> UpdatePrintedStatus(int[] ids)
    {
        var result = await _goodWarehousesService.UpdatePrintedStatus(ids);
        return Ok(result);
    }

    [HttpGet("report-good-for-warehouse")]
    public async Task<IActionResult> ReportForWareHouse(int warehouseId, int shelveId, int floorId, string type)
    {
        var result = await _goodWarehousesService.ReportWareHouse(warehouseId, shelveId, floorId, type);
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }
}
