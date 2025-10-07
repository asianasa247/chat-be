using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ManageEmployee.Services.Interfaces.P_Procedures.Supplies;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Extends;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers.SupplyControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RequestEquipmentOrdersController : ControllerBase
{
    private readonly IRequestEquipmentOrderService _requestEquipmentOrderService;
    private readonly IFileService _fileService;

    public RequestEquipmentOrdersController(IRequestEquipmentOrderService requestEquipmentOrderService, IFileService fileService)
    {
        _requestEquipmentOrderService = requestEquipmentOrderService;
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProcedurePagingRequestModel param)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        return Ok(await _requestEquipmentOrderService.GetPaging(param, userId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _requestEquipmentOrderService.GetDetail(id);
        return Ok(model);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] RequestEquipmentOrderModel model)
    {
        await _requestEquipmentOrderService.Update(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept(int id)
    {
        await _requestEquipmentOrderService.Accept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        await _requestEquipmentOrderService.NotAccept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _requestEquipmentOrderService.Delete(id);
        return Ok();
    }

    [HttpPost("uploadfile")]
    public IActionResult Uploadfile([FromForm] IFormFile file)
    {
        var response = _fileService.UploadFile(file, "RequestEquipmentOrder", file.FileName);
        return Ok(response);
    }

    [HttpPost("export/{id}")]
    public async Task<IActionResult> Export(int id)
    {
        var response = await _requestEquipmentOrderService.Export(id);
        return Ok(new BaseResponseCommonModel
        {
            Data = response,
        });
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList()
    {
        var response = await _requestEquipmentOrderService.GetList();
        return Ok(response);
    }
}
