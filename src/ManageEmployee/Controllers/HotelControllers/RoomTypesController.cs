using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Hotels;
using ManageEmployee.ViewModels.HotelModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.HotelControllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class RoomTypesController : ControllerBase
{
    private readonly IGoodRoomTypeService _goodRoomTypeService;
    public RoomTypesController(IGoodRoomTypeService goodRoomTypeService)
    {
        _goodRoomTypeService = goodRoomTypeService;
    }
    [HttpGet]
    public async Task<IActionResult> Getpaging([FromQuery] PagingRequestModel param)
    {
        var result = await _goodRoomTypeService.GetPaging(param);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _goodRoomTypeService.GetAll();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var result = await _goodRoomTypeService.GetById(id);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(GoodRoomTypeModel param)
    {
        await _goodRoomTypeService.Update(param);
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("room-type-for-order")]
    public async Task<IActionResult> GetRoomTypeForOrder([FromQuery] DateTime fromAt, DateTime toAt)
    {
        var result = await _goodRoomTypeService.GetRoomTypeForOrder(fromAt, toAt);
        return Ok(result);
    }
}
