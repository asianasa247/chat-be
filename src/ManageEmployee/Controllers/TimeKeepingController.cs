using ManageEmployee.DataTransferObject.TimeKeeperModels;
using ManageEmployee.Extends;
using ManageEmployee.Filters;
using ManageEmployee.Services.Interfaces.TimeKeepings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
[TypeFilter(typeof(ResponseWrapperFilterAttribute))]
public class TimeKeepingController : ControllerBase
{
    private readonly ITimeKeepingService _timeKeepingService;

    public TimeKeepingController(ITimeKeepingService timekeepingService)
    {
        _timeKeepingService = timekeepingService;
    }

    [HttpPost("checkin")]
    public async Task<IActionResult> ValidateIpAddress([FromForm] TimeKeepingValidationRequest request)
    {
        // Accessing a specific header
        if (Request.Headers.TryGetValue("dbName", out StringValues dbName))
        {
            // Get the remote IP address from the HttpContext
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
            var result = await _timeKeepingService.InOutValidateAsync(
                dbName: dbName,
                user: HttpContext.GetIdentityUser(),
                remoteIpAddress: remoteIpAddress,
                request: request
            );
            return Ok(result);
        }
        throw new Exception("Không tìm thấy database");
        
    }
    [HttpPost("qr-checkin")]
    public async Task<IActionResult> QRCheckin([FromBody] TimeKeepingQrValidationRequest request)
    {
        try
        {
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
            var result = await _timeKeepingService.InOutQRValidateAsync(
                user: HttpContext.GetIdentityUser(),
                remoteIpAddress: remoteIpAddress,
                request: request
            );
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = "Lỗi khi quét QR", error = ex.Message });
        }
    }


    [HttpGet("checkin")]
    public async Task<IActionResult> GetCheckin()
    {
        var result = await _timeKeepingService.GetInOutHistoryByDate(HttpContext.GetIdentityUser(), DateTime.Today);
        return Ok(result);
    }
    
    [HttpGet("get-public-ip")]
    public Task<IActionResult> GetIpClient()
    {
        var result = _timeKeepingService.GetIpClient(HttpContext.Connection.RemoteIpAddress);
        return Task.FromResult<IActionResult>(Ok(result));
    }
}