using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.P_Procedures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ManageEmployee.Controllers.P_ProcedureController;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class P_InventoryController : ControllerBase
{
    private readonly IP_InventoryService _pInventoryService;

    public P_InventoryController(
        IP_InventoryService p_InventoryService)
    {
        _pInventoryService = p_InventoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        return Ok(await _pInventoryService.GetAll(param.Page, param.PageSize, param.SearchText));
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var model = _pInventoryService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] P_InventoryViewModel model)
    {
        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                if (identity != null)
                {
                    model.UserCreated = int.Parse(identity.Claims.First(x => x.Type == "UserId").Value);
                    model.UserUpdated = int.Parse(identity.Claims.First(x => x.Type == "UserId").Value);
                }
            }
            var result = await _pInventoryService.Create(model);
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
    public async Task<IActionResult> Update([FromBody] P_InventoryViewModel model)
    {
        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                if (identity != null)
                {
                    model.UserUpdated = int.Parse(identity.Claims.First(x => x.Type == "UserId").Value);
                }
            }

            var result = await _pInventoryService.Update(model);
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
    public async Task<IActionResult> Accept(int id)
    {
        try
        {
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                if (identity != null)
                {
                    userId = int.Parse(identity.Claims.First(x => x.Type == "UserId").Value);
                }
            }
            var result = await _pInventoryService.Accept(id, userId);
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
            var result = await _pInventoryService.Delete(id);
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
            var result = _pInventoryService.GetProcedureNumber();
            return Ok(result);
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpGet("get-good-inventory")]
    public IActionResult GetListGood(string? wareHouse, string? account, string? detail1, string? detail2)
    {
        try
        {
            var result = _pInventoryService.GetListGood(wareHouse, account, detail1, detail2);
            return Ok(result);
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpGet("export-inventory")]
    public IActionResult ExportInventoryById(int id)
    {
        try
        {
            var result = _pInventoryService.ExportInventoryById(id);
            return Ok(new { data = result });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }
}