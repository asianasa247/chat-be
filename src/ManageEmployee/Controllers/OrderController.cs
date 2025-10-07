using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(
        IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchOrder([FromQuery] OrderSearchModel search)
    {
        var result = await _orderService.SearchOrder(search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var result = await _orderService.GetDetail(id);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromHeader] int yearFilter, [FromBody] OrderViewModelResponse model)
    {
        var result = await _orderService.Update(model, HttpContext.GetIdentityUser().Id, yearFilter);
        if (string.IsNullOrEmpty(result))
            return Ok(new BaseResponseModel { Data = result });
        return BadRequest(new { msg = "Update bill fail" });
    }

    [Route("notification-order")]
    [HttpGet]
    public async Task<ActionResult<OrderViewModelResponse>> GetNotificationToStaffCount()
    {
        try
        {
            var result = await _orderService.GetListOrderNew();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }
}