using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ChartOfAccountForCashiersController : ControllerBase
{
    private readonly IChartOfAccountForCashierService _chartOfAccountForCashierService;
    public ChartOfAccountForCashiersController(IChartOfAccountForCashierService chartOfAccountForCashierService)
    {
        _chartOfAccountForCashierService = chartOfAccountForCashierService;
    }
    [HttpGet]
    public async Task<IActionResult> GetPaging([FromHeader] int yearFilter, [FromQuery] ChartOfAccountForCashierPagingRequestModel param)
    {
        var data = await _chartOfAccountForCashierService.GetPagingChartOfAccountForCashier(param, yearFilter);
        return Ok(data);
    }
}
