using Common.Helpers;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Services.Interfaces.Hotels;
using ManageEmployee.ViewModels.HotelModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.HotelControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]

public class RoomConfigureTypesController : ControllerBase
{
    private readonly IRoomConfigureTypeService _roomConfigureTypeService;

    public RoomConfigureTypesController(IRoomConfigureTypeService RoomConfigureTypeService)
    {
        _roomConfigureTypeService = RoomConfigureTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaging([FromQuery] PagingRequestModel param)
    {
        var result = await _roomConfigureTypeService.GetPaging(param);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _roomConfigureTypeService.GetById(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(RoomConfigureTypeModel form)
    {
        await _roomConfigureTypeService.Create(form);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(RoomConfigureTypeModel form)
    {
        await _roomConfigureTypeService.Update(form);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _roomConfigureTypeService.Delete(id);
        return Ok();
    }

    [HttpGet("type/{type}")]
    public async Task<IActionResult> GetRoomConfigureForType([FromRoute] RoomConfigureTypeEnum type)
    {
        var result = await _roomConfigureTypeService.GetRoomConfigureForType(type);
        return Ok(result);
    }

    [HttpGet("room-config-types")]
    public IActionResult GetRoomType()
    {
        var roomTypeEnums = Enum.GetValues(typeof(RoomConfigureTypeEnum))
                           .Cast<RoomConfigureTypeEnum>()
                           .Select(v => new
                           {
                               id = v,
                               name = v.GetDescription()
                           })
                           .ToList();
        return Ok(roomTypeEnums);
    }
}