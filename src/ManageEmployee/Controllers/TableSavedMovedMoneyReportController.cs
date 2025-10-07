using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class TableSavedMovedMoneyReportController : Controller
{
    private readonly ISavedMovedMoneyReportService _voucherService;
    public TableSavedMovedMoneyReportController(ISavedMovedMoneyReportService ledgerService)
    {
        _voucherService = ledgerService;
    }

    [HttpPost("get-report-saved-moved-money")]
    public async Task<IActionResult> ExportData([FromHeader] int yearFilter, SavedMoneyReportParam param, bool isNoiBo = false)
    {
        string _value = await _voucherService.ExportDataReport(param, yearFilter, isNoiBo);
        if (string.IsNullOrEmpty(_value))
            return BadRequest();

        return Ok(new BaseResponseModel()
        {
            Data = _value,
        });
    }

}


