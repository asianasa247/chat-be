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
public class WareHouseShelvesController : ControllerBase
{
    private readonly IWareHouseShelvesService _wareHouseShelvesService;

    public WareHouseShelvesController(IWareHouseShelvesService wareHouseShelvesService)
    {
        _wareHouseShelvesService = wareHouseShelvesService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var response = await _wareHouseShelvesService.GetAll(param);
        return Ok(response);
    }


    [HttpGet("list")]
    public async Task<IActionResult> GetSelectList()
    {
        var model = await _wareHouseShelvesService.GetAll();
        return Ok(new BaseResponseModel
        {
            Data = model,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _wareHouseShelvesService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WarehouseShelveSetterModel request)
    {
        try
        {
            await _wareHouseShelvesService.Create(request);

            return Ok();
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] WarehouseShelveSetterModel request, int id)
    {
        request.Id = id;
        try
        {
            await _wareHouseShelvesService.Update(request);
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
        await _wareHouseShelvesService.Delete(id);
        return Ok();
    }
}
