using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ManageEmployee.Controllers.P_ProcedureController;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class P_LeaveController : ControllerBase
{
    private readonly IP_LeaveService _p_LeaveService;

    public P_LeaveController(
        IP_LeaveService P_LeaveService)
    {
        _p_LeaveService = P_LeaveService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProcedurePagingRequestModel param)
    {
        var user = HttpContext.GetIdentityUser();
        string roles = user.Role;
        int userId = user.Id;
        List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

        return Ok(await _p_LeaveService.GetAll(param, listRole, userId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _p_LeaveService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] P_LeaveViewModel model)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        await _p_LeaveService.Create(model, userId);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] P_LeaveViewModel model)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        await _p_LeaveService.Update(model, userId);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept([FromBody] P_LeaveViewModel model)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        await _p_LeaveService.Accept(model, userId);
        return Ok();
    }
    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        await _p_LeaveService.NotAccept(id, userId);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _p_LeaveService.Delete(id);
        return Ok();
    }

    [HttpGet("get-procedure-number")]
    public async Task<IActionResult> GetProcedureNumber()
    {
        var result = await _p_LeaveService.GetProcedureNumber();
        return Ok(result);
    }
}