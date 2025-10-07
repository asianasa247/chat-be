using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Symbols;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SymbolsController : ControllerBase
{
    private readonly ISymbolService _symbolService;

    public SymbolsController(
        ISymbolService symbolService)
    {
        _symbolService = symbolService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var response = await _symbolService.GetAll(param);
        return Ok(response);
    }

    [HttpGet("list")]
    public IActionResult GetSelectList()
    {
        var symbols = _symbolService.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = symbols,
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var model = _symbolService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Symbol symbol)
    {
        // map model to entity and set id
        try
        {
            _symbolService.Create(symbol);

            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Symbol symbol)
    {
        // map model to entity and set id
        try
        {
            _symbolService.Update(symbol);
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
        _symbolService.Delete(id);
        return Ok();
    }
}