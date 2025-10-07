using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.InOuts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.InOutControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class InOutReportsController : ControllerBase
{
    private readonly IInOutReportService _inOutReportService;

    public InOutReportsController(IInOutReportService inOutReportService) 
    {
        _inOutReportService = inOutReportService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param, int month, [FromHeader] int yearFilter)
    {
        return Ok(await _inOutReportService.GetPaging(param, month, yearFilter));
    }

    [HttpPost]
    public async Task<IActionResult> SetData([FromQuery] int month, [FromHeader] int yearFilter)
    {
        await _inOutReportService.SetData(month, yearFilter);
        return Ok();
    }
}