using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Cars;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.CarControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class DriverRoutersController : ControllerBase
{
    private readonly IDriverRouterService _driverRouterService;

    public DriverRoutersController(IDriverRouterService driverRouterService)
    {
        _driverRouterService = driverRouterService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel searchRequest)
    {
        var result = await _driverRouterService.GetPaging(searchRequest);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var result = await _driverRouterService.GetById(id);
        return Ok(result);
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start(int petrolConsumptionId)
    {
        await _driverRouterService.Start(petrolConsumptionId);
        return Ok();
    }

    [HttpPost("finish")]
    public async Task<IActionResult> Finish(int petrolConsumptionId)
    {
        await _driverRouterService.Finish(petrolConsumptionId);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] DriverRouterModel model)
    {
        await _driverRouterService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _driverRouterService.Delete(id);
        return Ok();
    }

    [HttpGet("list/police-point/{id}")]
    public async Task<IActionResult> GetListPoliceCheckPoint(int id)
    {
        var result = await _driverRouterService.GetListPoliceCheckPoint(id);
        return Ok(result);
    }
}