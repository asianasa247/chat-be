using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.WarehouseModel;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.WareHouses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.WareHouseController;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class WareHouseFloorsController : ControllerBase
{
    private readonly IWareHouseFloorService _wareHouseFloorService;

    public WareHouseFloorsController(IWareHouseFloorService wareHouseFloorService)
    {
        _wareHouseFloorService = wareHouseFloorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var response = await _wareHouseFloorService.GetAll(param);
        return Ok(response);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetSelectList()
    {
        var model = await _wareHouseFloorService.GetAll();
        return Ok(new BaseResponseModel
        {
            Data = model,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _wareHouseFloorService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WarehouseFloorSetterModel request)
    {
        try
        {
            await _wareHouseFloorService.Create(request);

            return Ok();
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] WarehouseFloorSetterModel request, int id)
    {
        request.Id = id;
        try
        {
            await _wareHouseFloorService.Update(request);
            return Ok();
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _wareHouseFloorService.Delete(id);
        return Ok();
    }
}