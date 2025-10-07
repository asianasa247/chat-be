using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Services.Interfaces.Reports;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

//[Authorize]
[ApiController]
[Route("[controller]")]
public class GeneralDiaryReportController : Controller
{
    private readonly IGeneralDiaryService _generalDiaryService;

    public GeneralDiaryReportController(
        IGeneralDiaryService generalDiaryService
        )
    {
        _generalDiaryService = generalDiaryService;
    }

    [HttpGet]
    [Route("get-report-general-diary")]
    public async Task<IActionResult> ExportData([FromHeader] int yearFilter, [FromQuery] GeneralDiaryReportParam param)
    {
        string _value = await _generalDiaryService.GenerateGeneralDiaryReport(param, yearFilter);
        if (string.IsNullOrEmpty(_value))
            return BadRequest();

        return Ok(new BaseResponseModel()
        {
            Data = _value,
        });
    }



}
