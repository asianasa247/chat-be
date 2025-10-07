using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Cars;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.CarControllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CarFieldsController : ControllerBase
{
    private readonly ICarFieldService _carFieldService;
    public CarFieldsController(ICarFieldService carFieldService)
    {
        _carFieldService = carFieldService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var result = await _carFieldService.GetPaging(param);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _carFieldService.GetById(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CarFieldModel model)
    {
        await _carFieldService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] CarFieldModel model)
    {
        await _carFieldService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _carFieldService.Delete(id);
        return Ok();
    }
}
