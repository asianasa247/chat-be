using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ManageEmployee.Extends;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.DataTransferObject.WeeklyScheduleModels;

namespace ManageEmployee.Controllers.SupplyControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WeeklySchedulesController : ControllerBase
{
    private readonly IWeeklyScheduleService _weeklyScheduleService;

    public WeeklySchedulesController(IWeeklyScheduleService weeklyScheduleService)
    {
        _weeklyScheduleService = weeklyScheduleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProcedurePagingRequestModel param)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        return Ok(await _weeklyScheduleService.GetPaging(param, userId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        var model = await _weeklyScheduleService.GetDetail(id, userId);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WeeklyScheduleModel model)
    {
        await _weeklyScheduleService.Create(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] WeeklyScheduleModel model)
    {
        await _weeklyScheduleService.Update(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept(int id)
    {
        await _weeklyScheduleService.Accept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        await _weeklyScheduleService.NotAccept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _weeklyScheduleService.Delete(id);
        return Ok();
    }

    [HttpGet("get-procedure-number")]
    public async Task<IActionResult> GetProcedureNumber()
    {
        var result = await _weeklyScheduleService.GetProcedureNumber();
        return Ok(new BaseResponseCommonModel
        {
            Data = result,
        });
    }
}
