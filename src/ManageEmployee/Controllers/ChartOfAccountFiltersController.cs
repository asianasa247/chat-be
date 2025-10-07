using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ChartOfAccountFiltersController : ControllerBase
{
    private readonly IChartOfAccountFilterService _chartOfAccountFilterService;
    public ChartOfAccountFiltersController(IChartOfAccountFilterService chartOfAccountFilterService)
    {
        _chartOfAccountFilterService = chartOfAccountFilterService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {

        var result = await _chartOfAccountFilterService.GetPaging(param);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList()
    {

        var result = await _chartOfAccountFilterService.GetList();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail([FromRoute] int id)
    {

        var result = await _chartOfAccountFilterService.GetDetail(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ChartOfAccountFilter form)
    {

        await _chartOfAccountFilterService.Create(form);
        return Ok();
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(ChartOfAccountFilter form, int id)
    {
        form.Id = id;
        await _chartOfAccountFilterService.Update(form);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await _chartOfAccountFilterService.Delete(id);
        return Ok();
    }
}
