using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.LedgerModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Ledgers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.LedgerControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LedgerProcedurePaymentsController : ControllerBase
{
    private readonly ILedgerProcedureProductService _ledgerProcedureProductService;
    private readonly ILedgerProcedureProductExporter _ledgerProcedureProductExporter;

    public LedgerProcedurePaymentsController(ILedgerProcedureProductService ledgerProduceExportService, 
        ILedgerProcedureProductExporter ledgerProcedureProductExporter)
    {
        _ledgerProcedureProductService = ledgerProduceExportService;
        _ledgerProcedureProductExporter = ledgerProcedureProductExporter;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProcedurePagingRequestModel param)
    {
        var userId = HttpContext.GetIdentityUser().Id;
        return Ok(await _ledgerProcedureProductService.GetPaging(param, userId, "PC"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _ledgerProcedureProductService.GetDetail(id);
        return Ok(model);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] LedgerProduceProductModel model)
    {
        var userId = HttpContext.GetIdentityUser().Id;
        await _ledgerProcedureProductService.Update(model, userId);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept([FromHeader] int yearFilter, int id)
    {
        var userId = HttpContext.GetIdentityUser().Id;
        await _ledgerProcedureProductService.Accept(id, userId, yearFilter);
        return Ok();
    }

    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        var userId = HttpContext.GetIdentityUser().Id;
        await _ledgerProcedureProductService.NotAccept(id, userId);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _ledgerProcedureProductService.Delete(id);
        return Ok();
    }

    [HttpPost("export/{id}")]
    public async Task<IActionResult> Export(int id)
    {
        string generatedFileName = await _ledgerProcedureProductExporter.Export(id);
        return Ok(new BaseResponseCommonModel
        {
            Data = generatedFileName,
        });
    }

    [HttpPost("uploadfile/{id}")]
    public async Task<IActionResult> Uploadfile([FromForm] List<IFormFile> files, [FromRoute] int id)
    {
        var response = await _ledgerProcedureProductService.UploadFile(files, id);
        return Ok(response);
    }
}