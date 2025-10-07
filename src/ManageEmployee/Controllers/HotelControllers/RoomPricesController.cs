using ManageEmployee.Services.Interfaces.Hotels;
using ManageEmployee.ViewModels.HotelModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.HotelControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RoomPricesController : ControllerBase
{
    private readonly IGoodRoomPriceService _goodRoomPriceService;

    public RoomPricesController(IGoodRoomPriceService goodRoomPriceService)
    {
        _goodRoomPriceService = goodRoomPriceService;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GoodRoomPriceRequestModel form)
    {
        var result = await _goodRoomPriceService.Get(form);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Update(GoodRoomPriceModel form)
    {
        await _goodRoomPriceService.Update(form);
        return Ok();
    }
}