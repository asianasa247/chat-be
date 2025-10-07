using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class PlanMissionCountryTaxReportController : Controller
{
    private readonly IPlanMissionCountryTaxService _voucherService;
    public PlanMissionCountryTaxReportController(IPlanMissionCountryTaxService ledgerService)
    {
        _voucherService = ledgerService;
    }

    [HttpPost("get-report-plan-mission-country-tax")]
    public async Task<IActionResult> ExportData([FromHeader] int yearFilter, PlanMissionCountryTaxParam param, bool isNoiBo = false)
    {
        string _value = await _voucherService.ExportDataReport(param, isNoiBo, yearFilter);
        if (string.IsNullOrEmpty(_value))
            return BadRequest();

        return Ok(new BaseResponseModel()
        {
            Data = _value,
        });
    }

}


