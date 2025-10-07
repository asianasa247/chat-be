using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Cars;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.CarControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PetrolConsumptionsController : ControllerBase
{
    private readonly IPetrolConsumptionService _petrolConsumptionService;
    public PetrolConsumptionsController(IPetrolConsumptionService petrolConsumptionService)
    {
        _petrolConsumptionService = petrolConsumptionService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var result = await _petrolConsumptionService.GetPaging(param);
        return Ok(result);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _petrolConsumptionService.GetById(id);
        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PetrolConsumptionModel model)
    {
        await _petrolConsumptionService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] PetrolConsumptionModel model)
    {
        await _petrolConsumptionService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _petrolConsumptionService.Delete(id);
        return Ok();
    }

    [HttpGet("report")]
    public async Task<IActionResult> ReportAsync([FromQuery] PetrolConsumptionReportRequestModel param)
    {
        var result = await _petrolConsumptionService.ReportAsync(param);
        return Ok(result);
    }
}
