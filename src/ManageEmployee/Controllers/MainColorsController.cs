using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.MainColors;
using ManageEmployee.Extends;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MainColorsController : ControllerBase
{
    private readonly IMainColorService _MainColorService;

    public MainColorsController(
        IMainColorService MainColorService)
    {
        _MainColorService = MainColorService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        return Ok(await _MainColorService.GetAll(param.Page, param.PageSize, param.SearchText, HttpContext.GetIdentityUser().Id));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _MainColorService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MainColor model)
    {
        try
        {
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            }
            var result = await _MainColorService.Create(model, userId);
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
    public async Task<IActionResult> Update([FromBody] MainColor model)
    {
        try
        {
            var result = await _MainColorService.Update(model);
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
            var result = await _MainColorService.Delete(id);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return BadRequest(new { msg = result });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }
    [HttpGet("get-by-user")]
    public async Task<IActionResult> GetByUserId()
    {
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            var userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            var model = await _MainColorService.GetByUserId(userId);
            return Ok(model);
        }
        return BadRequest();
    }

}
