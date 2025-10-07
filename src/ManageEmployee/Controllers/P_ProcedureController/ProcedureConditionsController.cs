using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.P_ProcedureController;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProcedureConditionsController : ControllerBase
{
    private readonly IProcedureConditionService _procedureConditionService;

    public ProcedureConditionsController(IProcedureConditionService procedureConditionService)
    {
        _procedureConditionService = procedureConditionService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll(string? procedureCode)
    {
        var result = await _procedureConditionService.GetList(procedureCode);
        return Ok(result);
    }
}