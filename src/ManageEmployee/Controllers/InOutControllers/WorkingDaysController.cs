using ManageEmployee.DataTransferObject.InOutModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.InOuts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.InOutControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WorkingDaysController : ControllerBase
{
    private readonly IWorkingDayService _workingDayService;

    public WorkingDaysController(IWorkingDayService workingDayService)
    {
        _workingDayService = workingDayService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetPaging([FromQuery] PagingRequestModel param)
    {
        var response = await _workingDayService.GetPaging(param);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail([FromRoute] int id)
    {
        var response = await _workingDayService.GetDetail(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WorkingDayModel model)
    {
        await _workingDayService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] WorkingDayModel model)
    {
        await _workingDayService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _workingDayService.Delete(id);
        return Ok();
    }
}