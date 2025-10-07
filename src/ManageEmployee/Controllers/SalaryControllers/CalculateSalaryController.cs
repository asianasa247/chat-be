using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Filters;
using ManageEmployee.Services.Interfaces.Users.Salaries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.SalaryControllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CalculateSalaryController : ControllerBase
{
    private readonly ICalculateSalaryService _calculateSalaryService;
    public CalculateSalaryController(ICalculateSalaryService calculateSalaryService)
    {
        _calculateSalaryService = calculateSalaryService;
    }
    
    [HttpGet("list")]
    public async Task<IActionResult> Calculate([FromQuery]PagingRequestFilterByMonthModel pagingRequest)
    {
        var result = await _calculateSalaryService.GetPaging(pagingRequest);
        return Ok(result);
    }
    
    [HttpPost("calculate")]
    public async Task<IActionResult> Calculate([FromQuery]int month, [FromQuery]int year)
    {
        await _calculateSalaryService.CalculateSalaryByMonth(month, year);
        return Ok();
    }
    
    [HttpPost("export-excel")]
    [TypeFilter(typeof(ResponseWrapperFilterAttribute))]
    public async Task<IActionResult> ExportToExcel([FromQuery]int month, [FromQuery]int year)
    {
        var fileName = await _calculateSalaryService.ExportToExcel(month, year);
        return Ok(fileName);
    }
    
    [HttpPost("export-pdf")]
    [TypeFilter(typeof(ResponseWrapperFilterAttribute))]
    public async Task<IActionResult> ExportToPdf([FromQuery]int month, [FromQuery]int year)
    {
        var fileName =await _calculateSalaryService.ExportToPdf(month, year);
        return Ok(fileName);
    }
}