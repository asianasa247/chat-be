using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.P_ProcedureController;

[Route("api/[controller]")]
[ApiController]
[Authorize]

public class ProcedureStatusStepsController : ControllerBase
{
    private readonly IProcedureStatusStepService _procedureStatusStepService;
    public ProcedureStatusStepsController(IProcedureStatusStepService procedureStatusStepService)
    {
        _procedureStatusStepService = procedureStatusStepService;
    }
    [HttpGet("{procedureId}")]
    public async Task<IActionResult> Getter(int procedureId)
    {
        var response = await _procedureStatusStepService.Getter(procedureId);
        return Ok(response);
    }

    [HttpPost("{procedureId}")]
    public async Task<IActionResult> Setter(int procedureId, [FromBody] List<P_ProcedureStatusStepModel> form)
    {
        await _procedureStatusStepService.Setter(procedureId, form);
        return Ok();
    }
}
