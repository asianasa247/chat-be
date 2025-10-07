using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ManageEmployee.Services.Interfaces.Menus;
using ManageEmployee.Extends;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.MenuModels;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MenusController : ControllerBase
{
    private readonly IMenuService _menuService;

    public MenusController(
        IMenuService menuService)
    {
        _menuService = menuService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] MenuPagingationRequestModel param)
    {
        var identityUser = HttpContext.GetIdentityUser();
        int userId = identityUser.Id;
        string roles = identityUser.Role;

        List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

        return Ok(await _menuService.GetAll(param.Page, param.PageSize, param.SearchText, param.isParent, param.CodeParent, listRole, userId, param.userRoleId));
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetMenu([FromQuery] MenuPagingationRequestModel param)
    {
        var results = await _menuService.GetAll(param.isParent);
        return Ok(new BaseResponseCommonModel
        {
            Data = results,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var identityUser = HttpContext.GetIdentityUser();
        int userId = identityUser.Id;
        string roles = identityUser.Role;

        List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

        var model = await _menuService.GetById(id, listRole, userId);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MenuViewModel model)
    {
        await _menuService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] MenuViewModel model)
    {
        var identityUser = HttpContext.GetIdentityUser();
        int userId = identityUser.Id;
        string roles = identityUser.Role;

        List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);


        await _menuService.Update(model, listRole, userId);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _menuService.Delete(id);
        return Ok();
    }

    [HttpGet("check-role")]
    public async Task<IActionResult> CheckRole(string? MenuCode)
    {
        var identityUser = HttpContext.GetIdentityUser();
        string roles = identityUser.Role;
        List<string> listRole = JsonConvert.DeserializeObject<List<string>>(roles);

        var result = await _menuService.CheckRole(MenuCode, listRole);
        return Ok(new ObjectReturn
        {
            status = 200,
            data = result,
        });

    }
}
