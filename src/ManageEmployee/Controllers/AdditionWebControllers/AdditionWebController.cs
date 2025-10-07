using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.AdditionWebs;
using ManageEmployee.Entities;
using ManageEmployee.Services.Interfaces.AdditionWebServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.AdditionWebControllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdditionWebController : ControllerBase
{
    private readonly IAdditionWebService _service;
    public AdditionWebController(IAdditionWebService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdditionWeb>>> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AdditionWeb>> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<AdditionWeb>> AddOrUpdate([FromBody] AdditionWebModel model)
    {
        var result = await _service.AddOrUpdateAsync(model);
        return Ok(result);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AdditionWeb>> Update(int id, [FromBody] AdditionWebModel model)
    {
        var result = await _service.UpdateAsync(id, model);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("{id:int}/company-info")]
    public async Task<ActionResult<AdditionWebCompanyResult>> GetCompanyInfo(int id)
    {
        var result = await _service.GetCompanyInfo(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpGet("companies")]
    public async Task<ActionResult> GetCompanies()
    {
        var result = await _service.GetCompaniesInfo();
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("companies-short-info")]
    public async Task<ActionResult> GetCompaniesShort()
    {
        var result = await _service.GetCompaniesShortInfo();
        return Ok(result);
    }
}