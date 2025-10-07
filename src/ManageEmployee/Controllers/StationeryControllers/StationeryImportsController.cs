using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Stationery;
using ManageEmployee.Services.Interfaces.Stationeries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.StationeryControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]

public class StationeryImportsController : ControllerBase
{
    private readonly IStationeryImportService _stationeryImportService;
    public StationeryImportsController(IStationeryImportService stationeryImportService)
    {
        _stationeryImportService = stationeryImportService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var result = await _stationeryImportService.GetPaging(param);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _stationeryImportService.GetById(id);
        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] StationeryImportModel model)
    {
        await _stationeryImportService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] StationeryImportModel model)
    {
        await _stationeryImportService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _stationeryImportService.Delete(id);
        return Ok();
    }

    [HttpGet("get-procedure-number")]
    public async Task<IActionResult> GetProcedureNumber()
    {
        var result = await _stationeryImportService.GetProcedureNumber();
        return Ok(result);
    }
}
