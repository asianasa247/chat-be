using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.AdditionWebServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.AdditionWebControllers;

[ApiController]
[Route("api/AdditionWeb")]
[Authorize]
public class AdditionWebOrdersController : ControllerBase
{
    private readonly IAdditionWebOrderService _additionWebOrderService;
    public AdditionWebOrdersController(IAdditionWebOrderService additionWebOrderService)
    {
        _additionWebOrderService = additionWebOrderService;
    }

    [HttpGet("{id:int}/orders")]
    public async Task<IActionResult> SearchOrder([FromRoute] int id, [FromQuery] OrderSearchModel search)
    {
        var result = await _additionWebOrderService.SearchOrder(id, search);
        return Ok(result);
    }
}