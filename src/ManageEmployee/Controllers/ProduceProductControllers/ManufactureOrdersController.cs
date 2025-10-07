using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ManageEmployee.Services.Interfaces.ProduceProducts;
using ManageEmployee.Extends;
using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using Hangfire;

namespace ManageEmployee.Controllers.ProduceProductControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ManufactureOrdersController : ControllerBase
{
    private readonly IManufactureOrderService _manufactureOrderService;
    private readonly IManufactureOrderExporter _manufactureOrderExporter;
    public ManufactureOrdersController(IManufactureOrderService manufactureOrderService, 
        IManufactureOrderExporter manufactureOrderExporter)
    {
        _manufactureOrderService = manufactureOrderService;
        _manufactureOrderExporter = manufactureOrderExporter;
    }
    [HttpGet]
    public async Task<IActionResult> GetPaging([FromQuery] ProcedurePagingRequestModel param)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        var result = await _manufactureOrderService.GetPaging(param, userId);
        return Ok(result);
    }
    [HttpGet("list")]
    public async Task<IActionResult> GetList()
    {
        var result = await _manufactureOrderService.GetList();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromHeader] int yearFilter, int id)
    {
        var result = await _manufactureOrderService.GetDetail(id, yearFilter);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ManufactureOrderModel model)
    {
        var identityUser = HttpContext.GetIdentityUser();

        await _manufactureOrderService.Create(model, identityUser.Id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] ManufactureOrderModel model)
    {
        var identityUser = HttpContext.GetIdentityUser();

        await _manufactureOrderService.Update(model, identityUser.Id);
        return Ok();
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateForManufacture([FromBody] ManufactureOrderGetDetailModel model)
    {
        var identityUser = HttpContext.GetIdentityUser();

        await _manufactureOrderService.UpdateForManufacture(model, identityUser.Id);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept(int id)
    {
        var identityUser = HttpContext.GetIdentityUser();
        await _manufactureOrderService.Accept(id, identityUser.Id);
        return Ok();
    }

    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        var identityUser = HttpContext.GetIdentityUser();
        await _manufactureOrderService.NotAccept(id, identityUser.Id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _manufactureOrderService.Delete(id);
        return Ok();
    }

    [HttpPost("export/{id}")]
    public async Task<IActionResult> ExportPdf(int id)
    {
        var response = await _manufactureOrderExporter.ExportPdf(id);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }

    [HttpPost("paging-manufacture/{id}")]
    public async Task<IActionResult> UpdateManufactureFromPaging([FromBody] ManufactureOrderPagingModel form)
    {
        await _manufactureOrderService.UpdateManufactureFromPaging(form);
        return Ok();
    }

    [HttpPost("export-detail/{id}")]
    public async Task<IActionResult> ExportGoodDetail(int id)
    {
        var response = await _manufactureOrderExporter.ExportGoodDetail(id);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }

    [HttpPut("responsible-user/{id}")]
    public async Task<IActionResult> SetResponsibleUser(int id, [FromBody] int responsibleUserId)
    {
        var identityUser = HttpContext.GetIdentityUser();
        await _manufactureOrderService.SetResponsibleUser(id, responsibleUserId, identityUser.Id);
        return Ok();
    }
}
