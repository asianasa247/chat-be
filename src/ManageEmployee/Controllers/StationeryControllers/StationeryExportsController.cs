using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Stationery;
using ManageEmployee.Services.Interfaces.Stationeries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.StationeryControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]

public class StationeryExportsController : ControllerBase
{
    private readonly IStationeryExportService _stationeryExportService;
    public StationeryExportsController(IStationeryExportService stationeryExportService)
    {
        _stationeryExportService = stationeryExportService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var result = await _stationeryExportService.GetPaging(param);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _stationeryExportService.GetById(id);
        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StationeryExportModel model)
    {
        await _stationeryExportService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] StationeryExportModel model)
    {
        await _stationeryExportService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _stationeryExportService.Delete(id);
        return Ok();
    }

    [HttpGet("get-procedure-number")]
    public async Task<IActionResult> GetProcedureNumber()
    {
        var result = await _stationeryExportService.GetProcedureNumber();
        return Ok(result);
    }
}
