using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.P_ProcedureController;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProcedureStatusController : ControllerBase
{
    private readonly IProcedureStatusService _procedureStatusService;

    public ProcedureStatusController(IProcedureStatusService procedureStatusService)
    {
        _procedureStatusService = procedureStatusService;
    }

    [HttpGet("list/{procedureId}")]
    public async Task<IActionResult> GetStatus(int procedureId)
    {
        var response = await _procedureStatusService.GetStatus(procedureId);
        return Ok(response);
    }


    [HttpGet("status-for-filter")]
    public async Task<IActionResult> GetStatusForFilter(string? procedureCode)
    {
        var response = await _procedureStatusService.GetStatusForFilter(procedureCode);
        return Ok(response);
    }
}