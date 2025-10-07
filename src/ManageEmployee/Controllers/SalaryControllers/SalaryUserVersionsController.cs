using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.UserModels.SalaryModels;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Users.Salaries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.SalaryControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SalaryUserVersionsController : ControllerBase
{
    private readonly ISalaryUserVersionService _salaryUserVersionService;

    public SalaryUserVersionsController(ISalaryUserVersionService salaryUserVersionService)
    {
        _salaryUserVersionService = salaryUserVersionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        return Ok(await _salaryUserVersionService.GetPaging(param));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _salaryUserVersionService.GetDetail(id);
        return Ok(model);
    }

    [HttpPost]
    [HttpPut("{id}")]
    public async Task<IActionResult> SetData([FromBody] SalaryUserVersionUpdateModel model)
    {
        var identityUser = HttpContext.GetIdentityUser();
        await _salaryUserVersionService.SetData(model, identityUser.Id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _salaryUserVersionService.Delete(id);
        return Ok();
    }


    [HttpPost("import")]
    public async Task<IActionResult> ImportExcel([FromForm] IFormFile file)
    {
        await _salaryUserVersionService.ImportExcel(file);
        return Ok();
    }

    [HttpPost("export")]
    public async Task<IActionResult> ExportExcel()
    {
        var fileName = await _salaryUserVersionService.ExportExcel();
        return Ok(fileName);
    }
}