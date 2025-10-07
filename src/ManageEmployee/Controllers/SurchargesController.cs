using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Surcharges;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SurchargesController : ControllerBase
{
    private readonly ISurchargeService _surchargeService;
    public SurchargesController(ISurchargeService surchargeService)
    {
        _surchargeService = surchargeService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {

        return Ok(await _surchargeService.GetAll(param.Page, param.PageSize, param.SearchText));
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _surchargeService.GetById(id);
        return Ok(model);
    }
    [HttpGet("get-current-surcharge")]
    public async Task<IActionResult> GetCurrent()
    {
        var model = await _surchargeService.GetCurrent();
        return Ok(model);
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Surcharge model)
    {
        try
        {
            var result = await _surchargeService.Create(model);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return Ok(new { code = 400, msg = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] Surcharge model)
    {
        try
        {

            var result = await _surchargeService.Update(model);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return Ok(new { code = 400, msg = result });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult>  Delete(int id)
    {
        try
        {
            var result = await _surchargeService.Delete(id);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return BadRequest(new { msg = result });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }
}
