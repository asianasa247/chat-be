using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ManageEmployee.Services.Interfaces.P_Procedures.Supplies;
using ManageEmployee.Extends;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers.SupplyControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RequestExportGoodsController : ControllerBase
{
    private readonly IRequestExportGoodService _requestExportGoodService;

    public RequestExportGoodsController(IRequestExportGoodService requestExportGoodService)
    {
        _requestExportGoodService = requestExportGoodService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProcedurePagingRequestModel param)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        return Ok(await _requestExportGoodService.GetPaging(param, userId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _requestExportGoodService.GetDetail(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RequestExportGoodModel model)
    {
        await _requestExportGoodService.Create(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] RequestExportGoodModel model)
    {
        await _requestExportGoodService.Update(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept(int id)
    {
        await _requestExportGoodService.Accept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _requestExportGoodService.Delete(id);
        return Ok();
    }

    [HttpGet("get-procedure-number")]
    public async Task<IActionResult> GetProcedureNumber()
    {
        var result = await _requestExportGoodService.GetProcedureNumber();
        return Ok(new BaseResponseCommonModel
        {
            Data = result,
        });
    }

    [HttpGet("check-permission-button/{id}")]
    public async Task<IActionResult> CheckButton(int id)
    {
        var result = await _requestExportGoodService.CheckButton(id, HttpContext.GetIdentityUser().Id);
        return Ok(result);
    }
}