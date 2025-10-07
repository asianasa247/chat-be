using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserRolesController : ControllerBase
{
    private readonly IUserRoleService _userRoleService;

    public UserRolesController(
        IUserRoleService userRoleService)
    {
        _userRoleService = userRoleService;
    }

    [HttpGet]
    public async Task<IActionResult> GePaging([FromQuery] PagingRequestModel param)
    {
        var identityUser = HttpContext.GetIdentityUser();
        int userId = identityUser.Id;
        string roles = identityUser.Role;

        List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

        var data = await _userRoleService.GetAll(userId, listRole);
        return Ok(new BaseResponseModel
        {
            TotalItems = data.Count(),
            Data = data.Skip(param.PageSize * (param.Page - 1))
            .Take(param.PageSize),
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll()
    {
        var identityUser = HttpContext.GetIdentityUser();
        int userId = identityUser.Id;
        string roles = identityUser.Role;
        List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

        var userRoles = await _userRoleService.GetAll(userId, listRole);

        return Ok(new BaseResponseModel
        {
            Data = userRoles,
        });
    }

    [HttpPost]
    [HttpPut("{id}")]
    public async Task<IActionResult> Save([FromBody] UserRole userRole)
    {
        if (userRole == null)
        {
            return BadRequest(new { msg = ResultErrorConstants.MODEL_NULL });
        }
        if (String.IsNullOrEmpty(userRole.Title))
        {
            return BadRequest(new { msg = ResultErrorConstants.MODEL_MISS });
        }
        var identityUser = HttpContext.GetIdentityUser();

        int userId = identityUser.Id;
        
        if (userRole.Id > 0)
        {
            userRole = await _userRoleService.Update(userRole);
        }
        else
        {
            userRole.UserCreated = userId;
            userRole = await _userRoleService.Create(userRole);
        }
        return Ok(userRole);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _userRoleService.Delete(id);
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var data = await _userRoleService.GetById(id);
        return Ok(new ObjectReturn
        {
            data = data,
            status = 200,
        });
    }
}