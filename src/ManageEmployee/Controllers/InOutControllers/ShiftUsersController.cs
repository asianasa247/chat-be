using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.Services.Interfaces.InOuts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace ManageEmployee.Controllers.InOutControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ShiftUsersController : ControllerBase
{
    private readonly IShiftUserService _shiftUserService;

    public ShiftUsersController(IShiftUserService shiftUserService) 
    {
        _shiftUserService = shiftUserService;
    }

    [HttpGet("{month}")]
    public async Task<IActionResult> GetById(int month, [FromHeader] int yearFilter)
    {
        int userId = -1;

        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }

        var model = await _shiftUserService.GetDetail(userId, month, yearFilter);
        return Ok(model);
    }

    [HttpPost("filter")]
    public async Task<IActionResult> Filter([FromBody]ShiftUserFilterModel request)
    {
        request.PageIndex = request.PageIndex > 0 ? request.PageIndex : 1;
        request.PageSize = request.PageSize > 0 ? request.PageSize : 20;

        var model = await _shiftUserService.FilterShiftUser(request);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ShiftUserModel model, [FromHeader] int yearFilter)
    {
        await _shiftUserService.SetData(model, yearFilter);
        return Ok();
    }

    [HttpGet("{id}/sync-user")]
    public async Task<IActionResult> SyncUser(int id, [FromHeader] int yearFilter)
    {
        await _shiftUserService.SyncUser(id, yearFilter);
        return Ok();
    }

    [HttpPost("{id}/user")]
    public async Task<IActionResult> SetShiftUserItem(int id, [FromBody] ShiftUserDetailModel item)
    {
        await _shiftUserService.SetShiftUserItem(id, item);
        return Ok();
    }

    [HttpPost("{shiftUserId}/users")]
    public async Task<IActionResult> SetShiftUserItems([FromRoute]int shiftUserId, [FromBody] List<ShiftUserDetailModel> items)
    {
        var result = await _shiftUserService.UpdateShiftUserItems(shiftUserId, items);
        return result ? Ok() : BadRequest();
    }

    [HttpPost("users")]
    public async Task<IActionResult> SetShiftUser([FromBody] ShiftUserBodyRequestModel request)
    {
        var result = await _shiftUserService.UpdateShiftUsers(request);
        return result ? Ok() : BadRequest();
    }
}