using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ManageEmployee.Services.Interfaces.Allowances;
using ManageEmployee.Extends;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.AllowanceModels;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AllowancesController : ControllerBase
{
    private readonly IAllowanceService _allowanceService;

    public AllowancesController(
        IAllowanceService allowanceService)
    {
        _allowanceService = allowanceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var response = await _allowanceService.GetAll(param.Page,param.PageSize,param.SearchText);
        return Ok(response);
    }


    [HttpGet("list")]
    public async Task<IActionResult> GetSelectList()
    {
        var response = await _allowanceService.GetAll();
        return Ok(new BaseResponseModel
        {
            Data = response,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _allowanceService.GetById(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AllowanceViewModel model)
    {
        var identityUser = HttpContext.GetIdentityUser();
        await _allowanceService.Create(model, identityUser.Id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] AllowanceViewModel model, int id = 0)
    {
        var identityUser = HttpContext.GetIdentityUser();
        model.Id = id;
        await _allowanceService.Update(model, identityUser.Id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _allowanceService.Delete(id);
        return Ok();
    }
}
