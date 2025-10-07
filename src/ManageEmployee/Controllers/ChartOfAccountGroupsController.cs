using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChartOfAccountGroupsController : ControllerBase
{
    private readonly IChartOfAccountGroupService _ChartOfAccountGroupService;

    public ChartOfAccountGroupsController(
        IChartOfAccountGroupService ChartOfAccountGroupService) 
    {
        _ChartOfAccountGroupService = ChartOfAccountGroupService;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] PagingRequestModel param, [FromHeader] int yearFilter)
    {
        var model = _ChartOfAccountGroupService.GetAll(param.Page, param.PageSize, yearFilter).ToList();
        return Ok(new BaseResponseModel
        {
            TotalItems = model.Count(),
            Data = model,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }


    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var model = _ChartOfAccountGroupService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public IActionResult Create([FromBody] ChartOfAccountGroup model, [FromHeader] int yearFilter)
    {
        try
        {
            // update user 
            _ChartOfAccountGroupService.Create(model, yearFilter);
            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] ChartOfAccountGroup model, [FromHeader] int yearFilter)
    {
        // map model to entity and set id
        model.Id = id;

        try
        {
            // update user 
            _ChartOfAccountGroupService.Update(model);
            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _ChartOfAccountGroupService.Delete(id);
        return Ok();
    }
}
