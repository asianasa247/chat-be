using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Hotels;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.Web;

[Route("api/[controller]")]
public class WebOrderController : ControllerBase
{
    private readonly IWebOrderService _webOrderService;
    private readonly IGoodRoomTypeService _goodRoomTypeService;

    public WebOrderController(IWebOrderService webOrderService, IGoodRoomTypeService goodRoomTypeService)
    {
        _webOrderService = webOrderService;
        _goodRoomTypeService = goodRoomTypeService;
    }
    [Authorize]

    [HttpGet("room-type")]
    public async Task<IActionResult> GetRoomType()
    {
        var response = await _goodRoomTypeService.GetAll();
        return Ok(response);
    }

    [HttpPost("createOrder")]
    public async Task<IActionResult> CreateOrder([FromBody] OrderViewModel request)
    {
        int? userId = null;
        try
        {
            var identityUser = HttpContext.GetIdentityUser();
            userId = identityUser.Id;
        }
        catch
        {

        }
        await _webOrderService.Create(request, userId);
        return Ok();
    }

    [Authorize]
    [HttpPost("order-by-customer")]
    public async Task<IActionResult> GetByCustomer(WebOrderSearchModel request)
    {
        return Ok(await _webOrderService.GetByCustomer(request));
    }
}