using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.P_ProcedureController;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProceduresController : ControllerBase
{
    private readonly IProcedureService _procedureService;

    public ProceduresController(IProcedureService procedureService)
    {
        _procedureService = procedureService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaging([FromQuery] PagingRequestModel param)
    {
        var response = await _procedureService.GetPaging(param);
        return Ok(response);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList()
    {
        var response = await _procedureService.GetAll();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _procedureService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] P_ProcedureViewModel model)
    {
        await _procedureService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] P_ProcedureViewModel model)
    {
        await _procedureService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _procedureService.Delete(id);
        return Ok();
    }

    [HttpGet("count-procedure")]
    public async Task<IActionResult> GetProcedureNeedAccept()
    {
        var identityUser = HttpContext.GetIdentityUser();
        var response = await _procedureService.GetProcedureNeedAccept(identityUser.Id);

        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }

    [HttpPost("reset-procedure-count")]
    public async Task<IActionResult> ResertProcedureCount([FromQuery] string procedureCode)
    {
        var identityUser = HttpContext.GetIdentityUser();
        await _procedureService.ResetProcedureCountAsync(identityUser.Id, procedureCode);
        return Ok();
    }
}