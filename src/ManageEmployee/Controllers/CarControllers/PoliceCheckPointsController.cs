using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.CarEntities;
using ManageEmployee.Services.Interfaces.Cars;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.CarControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PoliceCheckPointsController : ControllerBase
{
    private readonly IPoliceCheckPointService _policeCheckPointService;

    public PoliceCheckPointsController(IPoliceCheckPointService policeCheckPointService)
    {
        _policeCheckPointService = policeCheckPointService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaging([FromQuery] PagingRequestModel searchRequest)
    {
        var result = await _policeCheckPointService.GetPaging(searchRequest);
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var result = await _policeCheckPointService.GetDetail(id);
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PoliceCheckPoint model)
    {
        await _policeCheckPointService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] PoliceCheckPoint model)
    {
        await _policeCheckPointService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _policeCheckPointService.Delete(id);
        return Ok();
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _policeCheckPointService.GetAll();
        return Ok(new BaseResponseCommonModel
        {
            Data = result,
        });
    }
}