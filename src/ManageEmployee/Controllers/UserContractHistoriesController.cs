using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserContractHistoriesController : ControllerBase
{
    private readonly IUserContractHistoryService _userContractHistoryService;

    public UserContractHistoriesController(IUserContractHistoryService userContractHistoryService)
    {
        _userContractHistoryService = userContractHistoryService;
    }

    [HttpGet]
    public async Task<IActionResult> UserContractHistoriesPaging([FromQuery] PagingRequestModel param)
    {
        var result = await _userContractHistoryService.GetPagingAsync(param);
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> UserContractHistoryDetail(int id)
    {
        var result = await _userContractHistoryService.GetDetail(id);
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }
}