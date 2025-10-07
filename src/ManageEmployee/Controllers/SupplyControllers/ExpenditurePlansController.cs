using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.P_Procedures.ExpenditurePlans;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.SupplyControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ExpenditurePlansController : ControllerBase
{
    private readonly IExpenditurePlanService _expenditurePlanService;
    private readonly IFileService _fileService;
    private readonly IExpenditurePlanExporter _expenditurePlanExporter;
    public ExpenditurePlansController(IExpenditurePlanService expenditurePlanService, IFileService fileService, IExpenditurePlanExporter expenditurePlanExporter)
    {
        _expenditurePlanService = expenditurePlanService;
        _fileService = fileService;
        _expenditurePlanExporter = expenditurePlanExporter;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProcedurePagingRequestModel param)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        return Ok(await _expenditurePlanService.GetPaging(param, userId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _expenditurePlanService.GetDetail(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ExpenditurePlanSetterModel model)
    {
        await _expenditurePlanService.Create(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] ExpenditurePlanSetterModel model)
    {
        await _expenditurePlanService.Update(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept(int id)
    {
        await _expenditurePlanService.Accept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        await _expenditurePlanService.NotAccept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _expenditurePlanService.Delete(id);
        return Ok();
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetProcedureNumber()
    {
        var result = await _expenditurePlanService.GetList();
        return Ok(new BaseResponseCommonModel
        {
            Data = result,
        });
    }

    [HttpPut("expenditure-plan/{id}")]
    public async Task<IActionResult> UpdateExpenditure([FromBody] ExpenditurePlanModel model)
    {
        await _expenditurePlanService.UpdateExpenditure(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPost("uploadfile")]
    public IActionResult Uploadfile([FromForm] IFormFile file)
    {
        var response = _fileService.UploadFile(file, "ExpenditurePlan", file.FileName);
        return Ok(response);
    }

    [HttpPost("export/{id}")]
    public async Task<IActionResult> Export(int id)
    {
        var response = await _expenditurePlanExporter.Export(id);
        return Ok(new BaseResponseCommonModel
        {
            Data = response,
        });
    }

    [HttpPost("export-advance/{id}")]
    public async Task<IActionResult> ExportAdvanceSettlement(int id)
    {
        var response = await _expenditurePlanExporter.ExportAdvanceSettlement(id);
        return Ok(new BaseResponseCommonModel
        {
            Data = response,
        });
    }
}