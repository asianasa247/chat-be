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
public class AdvancePaymentsController : ControllerBase
{
    private readonly IAdvancePaymentService _advancePaymentService;
    private readonly IFileService _fileService;

    public AdvancePaymentsController(IAdvancePaymentService advancePaymentService, IFileService fileService)
    {
        _advancePaymentService = advancePaymentService;
        _fileService = fileService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProcedurePagingRequestModel param)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        return Ok(await _advancePaymentService.GetPaging(param, userId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _advancePaymentService.GetDetail(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AdvancePaymentModel model)
    {
        await _advancePaymentService.Create(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] AdvancePaymentModel model)
    {
        await _advancePaymentService.Update(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept(int id)
    {
        await _advancePaymentService.Accept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }
    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        await _advancePaymentService.NotAccept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _advancePaymentService.Delete(id);
        return Ok();
    }

    [HttpGet("get-procedure-number")]
    public async Task<IActionResult> GetProcedureNumber()
    {
        var result = await _advancePaymentService.GetProcedureNumber();
        return Ok(new BaseResponseCommonModel
        {
            Data = result,
        });
    }

    [HttpGet("check-permission-button/{id}")]
    public async Task<IActionResult> CheckButton(int id)
    {
        var result = await _advancePaymentService.CheckButton(id, HttpContext.GetIdentityUser().Id);
        return Ok(result);
    }

    [HttpPost("uploadfile")]
    public IActionResult Uploadfile([FromForm] IFormFile file)
    {
        var response = _fileService.UploadFile(file, "AdvancePayment", file.FileName);
        return Ok(response);
    }

    [HttpPost("export/{id}")]
    public async Task<IActionResult> Export(int id)
    {
        var response = await _advancePaymentService.Export(id);
        return Ok(new BaseResponseCommonModel
        {
            Data = response,
        });
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList()
    {
        var response = await _advancePaymentService.GetList();
        return Ok(response);
    }
}
