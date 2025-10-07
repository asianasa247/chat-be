using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AccountantBalanceSheetReportController : Controller
{
    private readonly IBalanceSheetService _accountBalanceSheetService;

    public AccountantBalanceSheetReportController(
        IBalanceSheetService accountBalanceSheetService)
    {
        _accountBalanceSheetService = accountBalanceSheetService;
    }

    [HttpPost]
    [Route("get-report-accountant-balance")]
    public async Task<IActionResult> GetReportAccountantBalance([FromHeader] int yearFilter, [FromBody] LedgerReportParam request, bool isNoiBo = false)
    {
        var data = await _accountBalanceSheetService.ExportDataAccountantBalance(request, yearFilter);
        return Ok(new BaseResponseModel
        {
            Data = data
        });
    }

}
















