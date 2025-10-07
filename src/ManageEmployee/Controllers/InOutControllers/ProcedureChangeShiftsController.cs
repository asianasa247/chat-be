using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.InOuts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.InOutControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProcedureChangeShiftsController : ControllerBase
{
    private readonly IProcedureChangeShiftService _procedureChangeShiftService;

    public ProcedureChangeShiftsController(IProcedureChangeShiftService procedureChangeShiftService)
    {
        _procedureChangeShiftService = procedureChangeShiftService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProcedureRequestOvertimePagingRequestModel param)
    {
        return Ok(await _procedureChangeShiftService.GetPaging(param));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _procedureChangeShiftService.GetDetail(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProcedureChangeShiftModel model)
    {
        await _procedureChangeShiftService.Create(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] ProcedureChangeShiftModel model)
    {
        await _procedureChangeShiftService.Update(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept([FromBody] ProcedureChangeShiftModel model)
    {
        await _procedureChangeShiftService.Accept(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _procedureChangeShiftService.Delete(id);
        return Ok();
    }

    [HttpGet("get-procedure-number")]
    public async Task<IActionResult> GetProcedureNumber()
    {
        var result = await _procedureChangeShiftService.GetProcedureNumber();
        return Ok(result);
    }

    [HttpGet("check-permission-button/{id}")]
    public async Task<IActionResult> CheckButton(int id)
    {
        var result = await _procedureChangeShiftService.CheckButton(id, HttpContext.GetIdentityUser().Id);
        return Ok(result);
    }
}