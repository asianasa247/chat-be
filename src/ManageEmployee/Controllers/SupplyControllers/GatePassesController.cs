using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.P_Procedures.Supplies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.SupplyControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GatePassesController : ControllerBase
{
    private readonly IGatePassService _gatePassService;
    private readonly IFileService _fileService;

    public GatePassesController(IGatePassService gatePassService, IFileService fileService)
    {
        _gatePassService = gatePassService;
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProcedurePagingRequestModel param)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        return Ok(await _gatePassService.GetPaging(param, userId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _gatePassService.GetDetail(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GatePassModel model)
    {
        await _gatePassService.Create(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] GatePassModel model)
    {
        await _gatePassService.Update(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept(int id)
    {
        await _gatePassService.Accept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        await _gatePassService.NotAccept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _gatePassService.Delete(id);
        return Ok();
    }

    [HttpGet("get-procedure-number")]
    public async Task<IActionResult> GetProcedureNumber()
    {
        var result = await _gatePassService.GetProcedureNumber();
        return Ok(new BaseResponseCommonModel
        {
            Data = result,
        });
    }

    [HttpGet("check-permission-button/{id}")]
    public async Task<IActionResult> CheckButton(int id)
    {
        var result = await _gatePassService.CheckButton(id, HttpContext.GetIdentityUser().Id);
        return Ok(result);
    }

    [HttpPost("export/{id}")]
    public async Task<IActionResult> Export(int id)
    {
        var response = await _gatePassService.Export(id);
        return Ok(new BaseResponseCommonModel
        {
            Data = response,
        });
    }


    [HttpPost("uploadfile")]
    public IActionResult Uploadfile([FromForm] IFormFile file)
    {
        var response = _fileService.UploadFile(file, "GatePass", file.FileName);
        return Ok(response);
    }
}