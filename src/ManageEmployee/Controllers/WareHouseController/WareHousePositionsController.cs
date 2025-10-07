using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.WareHouseEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.WareHouses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.WareHouseController;

[Route("api/[controller]")]
[ApiController]
[Authorize]

public class WareHousePositionsController : ControllerBase
{
    private readonly IWareHousePositionService _wareHousePositionService;
    public WareHousePositionsController(IWareHousePositionService wareHousePositionService)
    {
        _wareHousePositionService = wareHousePositionService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var response = await _wareHousePositionService.GetAll(param);
        return Ok(response);
    }


    [HttpGet("list")]
    public async Task<IActionResult> GetSelectList()
    {
        var model = await _wareHousePositionService.GetAll();
        return Ok(new BaseResponseModel
        {
            Data = model,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _wareHousePositionService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WareHousePosition request)
    {
        try
        {
           await  _wareHousePositionService.Create(request);

            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] WareHousePosition request, int id)
    {
        // map model to entity and set id
        request.Id = id;

        try
        {
            await _wareHousePositionService.Update(request);
            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _wareHousePositionService.Delete(id);
        return Ok();
    }
}
