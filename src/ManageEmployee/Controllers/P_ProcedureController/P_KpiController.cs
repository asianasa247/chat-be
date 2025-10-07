using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Extends;
using ManageEmployee.DataTransferObject.P_ProcedureView.PagingRequest;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers.P_ProcedureController;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class P_KpiController : ControllerBase
{
    private readonly IP_KpiService _p_KpiService;

    public P_KpiController(
        IP_KpiService p_KpiService)
    {
        _p_KpiService = p_KpiService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] P_KpiRequestModel param)
    {
        string roles = "";
        int userId = 0;
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            roles = identity.FindFirst(x => x.Type == "RoleName").Value.ToString();
            userId = int.Parse(identity.Claims.First(x => x.Type == "UserId").Value);
        }
        List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

        return Ok(await _p_KpiService.GetPaging(param));
    }


    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var model = _p_KpiService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] P_KpiViewModel model)
    {
        try
        {
            var userId = HttpContext.GetIdentityUser().Id;

            var result = await _p_KpiService.Create(model, userId);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return Ok(new { code = 400, msg = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] P_KpiViewModel model)
    {
        try
        {
            var userId = HttpContext.GetIdentityUser().Id;
            var result = await _p_KpiService.Update(model, userId);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return Ok(new { code = 400, msg = result });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }
    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept([FromBody] P_KpiViewModel model)
    {
        try
        {
            var userId = HttpContext.GetIdentityUser().Id;

            var result = await _p_KpiService.Accept(model, userId);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return Ok(new { code = 400, msg = result });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _p_KpiService.Delete(id);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return BadRequest(new { msg = result });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpGet("get-procedure-number")]
    public IActionResult GetProcedureNumber()
    {
        try
        {
            var result = _p_KpiService.GetProcedureNumber();
            return Ok(result);
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }
    [HttpGet("report-kpi")]
    public async Task<IActionResult> ReportKPI(int? UserId, int? DepartmentId, int? BranchId, int Month)
    {
        try
        {
            var result = await _p_KpiService.ReportKPI(UserId, DepartmentId, BranchId, Month);
            return Ok(result);
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }
    [HttpGet("report-export-kpi")]
    public async Task<IActionResult> ReportExportKPI(int? UserId, int? DepartmentId, int? BranchId, int Month)
    {
        try
        {
            var result = await _p_KpiService.ExportExcel_Report_Kpi(UserId, DepartmentId, BranchId, Month);
            return Ok(result);
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpGet("get-user-kpi")]
    public IActionResult GetAllUserActive(int? departmentId)
    {
        var result = _p_KpiService.GetAllUserActive(departmentId);
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }
    [HttpGet("get-report-kpi-full-for-month")]
    public async Task<IActionResult> ReportKpiAll(int month)
    {
        var result = await _p_KpiService.ReportKpiAll(month);
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpGet("point-for-user")]
    public async Task<IActionResult> GetPointForUser(int month, int userId)
    {
        var result = await _p_KpiService.GetPointForUser(userId, month);
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }
}