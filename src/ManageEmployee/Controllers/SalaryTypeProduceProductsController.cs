using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.SalaryModels;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Salarys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SalaryTypeProduceProductsController : ControllerBase
{
    private readonly ISalaryTypeProduceProductService _salaryTypeProduceProductService;
    public SalaryTypeProduceProductsController(ISalaryTypeProduceProductService salaryTypeProduceProductService)
    {
        _salaryTypeProduceProductService = salaryTypeProduceProductService;
    }
    [HttpGet]
    public async Task<IActionResult> GetPage([FromQuery] PagingRequestModel param)
    {
        var data = await _salaryTypeProduceProductService.GetPaging(param);
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _salaryTypeProduceProductService.GetById(id);
        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SalaryTypeProduceProductModel form)
    {
        await _salaryTypeProduceProductService.Create(form, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] SalaryTypeProduceProductModel form)
    {
        await _salaryTypeProduceProductService.Update(form, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _salaryTypeProduceProductService.Delete(id);
        return Ok();
    }
}
