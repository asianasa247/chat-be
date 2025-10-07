using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Addresses;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TypeWorksController : ControllerBase
{
    private ITypeWorkService _typeWorkService;

    public TypeWorksController(
        ITypeWorkService typeWorkService)
    {
        _typeWorkService = typeWorkService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var data = await _typeWorkService.GetAll(param.Page, param.PageSize);
        return Ok(data);
    }

    [HttpGet("list")]
    public IActionResult GetList()
    {
        var result = _typeWorkService.GetAll().ToList();
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var result = _typeWorkService.GetById(id);
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TypeWorkModel model)
    {
        try
        {
            // update user 
            await _typeWorkService.Create(model);
            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] TypeWorkModel model)
    {
        try
        {
            await _typeWorkService.Update(model);
            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _typeWorkService.Delete(id);
        return Ok();
    }
}
