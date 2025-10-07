using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ReportTaxController : ControllerBase
{
    private readonly IReportTaxService _reportTaxService;
    public ReportTaxController(IReportTaxService reportTaxService) 
    {
        _reportTaxService = reportTaxService;
    }
    [HttpGet("export-report-xml")]
    public IActionResult GetPage()
    {

        var data = _reportTaxService.ExportXML();
        return Ok(new BaseResponseModel()
        {
            Data = data,
        });
    }
    [HttpPost("export-report-pdf")]
    public async Task<IActionResult> ExportPDF([FromBody] ReportTaxRequest request, [FromHeader] int yearFilter, bool isNoiBo)
    {
        var data = await _reportTaxService.ExportPDF(request, isNoiBo, yearFilter);
        return Ok(new BaseResponseModel()
        {
            Data = data,
        });
    }
}
