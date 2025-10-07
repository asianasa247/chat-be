using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.InOuts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.InOutControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ShiftsController : ControllerBase
{
    private readonly IShiftService _shiftService;

    public ShiftsController(IShiftService shiftService)
    {
        _shiftService = shiftService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var result = await _shiftService.GetPaging(param);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _shiftService.GetDetail(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ShiftModel model)
    {
        await _shiftService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] ShiftModel model)
    {
        await _shiftService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _shiftService.Delete(id);
        return Ok();
    }
}