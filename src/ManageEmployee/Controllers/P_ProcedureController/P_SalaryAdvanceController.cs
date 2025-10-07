using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Extends;
using ManageEmployee.DataTransferObject.P_ProcedureView.PagingRequest;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject;

namespace ManageEmployee.Controllers.P_ProcedureController;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class P_SalaryAdvanceController : ControllerBase
{
    private readonly IP_SalaryAdvanceService _p_SalaryAdvanceService;

    public P_SalaryAdvanceController(IP_SalaryAdvanceService p_SalaryAdvanceService)
    {
        _p_SalaryAdvanceService = p_SalaryAdvanceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] P_SalaryAdvanceRequestModel param)
    {
        string roles = "";
        int userId = 0;
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            roles = identity.FindFirst(x => x.Type == "RoleName").Value.ToString();
            userId = int.Parse(identity.Claims.First(x => x.Type == "UserId").Value);
        }
        List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);
        return Ok(await _p_SalaryAdvanceService.GetAll(param, listRole, userId));
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var model = _p_SalaryAdvanceService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] P_SalaryAdvanceViewModel model)
    {
        try
        {
            var result = await _p_SalaryAdvanceService.Create(model);
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
    public async Task<IActionResult> Update([FromBody] P_SalaryAdvanceViewModel model)
    {
        try
        {
            var result = await _p_SalaryAdvanceService.Update(model);
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
    public async Task<IActionResult> Accept([FromBody] P_SalaryAdvanceViewModel model)
    {
        try
        {
            var userId = HttpContext.GetIdentityUser().Id;

            var result = await _p_SalaryAdvanceService.Accept(model, userId);
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
            var result = await _p_SalaryAdvanceService.Delete(id);
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
    public async Task<IActionResult> GetProcedureNumber()
    {
        var result = await _p_SalaryAdvanceService.GetProcedureNumber();
        return Ok(result);
    }

    [HttpPost("salary-advance-accountant")]
    public async Task<IActionResult> AddLedger([FromHeader] int yearFilter, int month, int isInternal)
    {
        try
        {
            await _p_SalaryAdvanceService.AddLedger(month, yearFilter, isInternal);
            return Ok();
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPost("salary-advance-for-user")]
    public async Task<IActionResult> CreateForUser([FromBody] P_SalaryAdvance_ItemForUser model)
    {
        var identityUser = HttpContext.GetIdentityUser();

        var result = await _p_SalaryAdvanceService.CreateForUser(model, identityUser.Id);
        if (string.IsNullOrEmpty(result))
            return Ok();
        return BadRequest(new { msg = result });
    }

    [HttpGet("salary-advance-for-user")]
    public async Task<IActionResult> GetAllForUser([FromQuery] PagingRequestModel param)
    {
        var identityUser = HttpContext.GetIdentityUser();
        var result = await _p_SalaryAdvanceService.GetAllForUser(param, identityUser.Id);
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }

    [HttpGet("salary-advance-for-user/{id}")]
    public async Task<IActionResult> GetDetailForUser(int id)
    {
        var result = await _p_SalaryAdvanceService.GetByIdForUser(id);
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }

    [HttpPut("salary-advance-for-user/{id}")]
    public async Task<IActionResult> UpdateForUser(P_SalaryAdvance_ItemForUser model)
    {
        var result = await _p_SalaryAdvanceService.UpdateForUser(model);
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }

    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        var userId = HttpContext.GetIdentityUser().Id;
        await _p_SalaryAdvanceService.NotAccept(id, userId);
        return Ok();
    }
}