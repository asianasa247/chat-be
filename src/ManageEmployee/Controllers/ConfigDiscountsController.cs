using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ConfigDiscountsController : ControllerBase
{
    private readonly IConfigDiscountService _configDiscountService;
    public ConfigDiscountsController(IConfigDiscountService configDiscountService)
    {
        _configDiscountService = configDiscountService;
    }
    [HttpGet]
    public async Task<IActionResult> GetPage([FromQuery] PagingRequestModel param)
    {
        var data = await _configDiscountService.GetPaging(param);
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _configDiscountService.GetById(id);
        return Ok(data);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ConfigDiscountModel form)
    {
        await _configDiscountService.Create(form);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] ConfigDiscountModel form)
    {
        await _configDiscountService.Update(form);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _configDiscountService.Delete(id);
        return Ok();
    }
}
