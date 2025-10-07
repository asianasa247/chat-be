using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Cars;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.CarControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RoadRoutesController : ControllerBase
{
    private readonly IRoadRouteService _roadRouteService;
    public RoadRoutesController(IRoadRouteService roadRouteService)
    {
        _roadRouteService = roadRouteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel searchRequest)
    {
        var result = await _roadRouteService.GetPaging(searchRequest);
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var result = await _roadRouteService.GetDetail(id);
        return Ok(new BaseResponseCommonModel
        {
            Data = result,
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RoadRouteModel model)
    {
        await _roadRouteService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] RoadRouteModel model)
    {
        await _roadRouteService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _roadRouteService.Delete(id);
        return Ok();
    }
}
