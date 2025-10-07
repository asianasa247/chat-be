using ManageEmployee.Services.Interfaces.Hanets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.Hanet;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CheckInsController : ControllerBase
{
    private readonly ICheckInService _checkInService;

    public CheckInsController(ICheckInService checkInService)
    {
        _checkInService = checkInService;
    }

    [HttpGet]
    [Route("push-data-checkin")]
    public async Task<IActionResult> GetAccountInfoByPeriod()
    {
        await _checkInService.GetCheckinByPlaceIdInDay();
        return Ok();
    }
}