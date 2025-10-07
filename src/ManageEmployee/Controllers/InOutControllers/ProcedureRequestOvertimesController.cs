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
public class ProcedureRequestOvertimesController : ControllerBase
{
    private readonly IProcedureRequestOvertimeService _procedureRequestOvertimeService;

    public ProcedureRequestOvertimesController(IProcedureRequestOvertimeService procedureRequestOvertimeService)
    {
        _procedureRequestOvertimeService = procedureRequestOvertimeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProcedureRequestOvertimePagingRequestModel param)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        return Ok(await _procedureRequestOvertimeService.GetPaging(param, userId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _procedureRequestOvertimeService.GetDetail(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProcedureRequestOvertimeModel model)
    {
        await _procedureRequestOvertimeService.Create(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] ProcedureRequestOvertimeModel model)
    {
        await _procedureRequestOvertimeService.Update(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept(int id)
    {
        await _procedureRequestOvertimeService.Accept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _procedureRequestOvertimeService.Delete(id);
        return Ok();
    }

    [HttpGet("get-procedure-number")]
    public async Task<IActionResult> GetProcedureNumber()
    {
        var result = await _procedureRequestOvertimeService.GetProcedureNumber();
        return Ok(result);
    }

    [HttpPost("copy/{id}")]
    public async Task<IActionResult> Copy(int id, [FromBody] List<int> userIds)
    {
        await _procedureRequestOvertimeService.Copy(id, userIds, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        var userId = HttpContext.GetIdentityUser().Id;
        await _procedureRequestOvertimeService.NotAccept(id, userId);
        return Ok();
    }
}