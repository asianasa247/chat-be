using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Users;
using ManageEmployee.Services.Interfaces.Allowances;
using ManageEmployee.DataTransferObject.UserModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]

public class AllowanceUsersController : ControllerBase
{
    private readonly IAllowanceUserService _allowanceService;
    private readonly IUserService _userService;

    public AllowanceUsersController(
        IAllowanceUserService allowanceService, IUserService userService)
    {
        _allowanceService = allowanceService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var model = _allowanceService.GetAll(param.Page,param.PageSize,param.SearchText);
        var totalItems = await _userService.GetMany(x => !x.IsDelete 
        && (string.IsNullOrEmpty(param.SearchText) ||  x.FullName.ToLower().Contains(param.SearchText.ToLower())));
        return Ok(new BaseResponseModel
        {
            TotalItems = totalItems.Count(),
            Data = model,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpPost]
    public IActionResult Save([FromBody] AllowanceUserViewModel model)
    {
        // map model to entity and set id

        try
        {
            _allowanceService.Update(model);
            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }
}
