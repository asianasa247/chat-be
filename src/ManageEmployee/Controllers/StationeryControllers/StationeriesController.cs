using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Stationery;
using ManageEmployee.Services.Interfaces.Stationeries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.StationeryControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class StationeriesController : ControllerBase
{
    private readonly IStationeryService _stationeryService;

    public StationeriesController(IStationeryService stationeryService)
    {
        _stationeryService = stationeryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var result = await _stationeryService.GetPaging(param);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _stationeryService.GetList();
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _stationeryService.GetById(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StationeryModel model)
    {
        await _stationeryService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] StationeryModel model)
    {
        await _stationeryService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _stationeryService.Delete(id);
        return Ok();
    }
}