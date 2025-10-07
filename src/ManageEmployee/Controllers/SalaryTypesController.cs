using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.SalaryModels;
using ManageEmployee.Services.Interfaces.Salarys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SalaryTypesController : ControllerBase
{
    private readonly ISalaryTypeService _salaryTypeService;
    public SalaryTypesController(ISalaryTypeService salaryTypeService)
    {
        _salaryTypeService = salaryTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPage([FromQuery] PagingRequestModel param)
    {
        var data = await _salaryTypeService.GetPaging(param);
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _salaryTypeService.GetById(id);
        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SalaryTypeModel form)
    {
        await _salaryTypeService.Create(form);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] SalaryTypeModel form)
    {
        await _salaryTypeService.Update(form);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _salaryTypeService.Delete(id);
        return Ok();
    }
}
