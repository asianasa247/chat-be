using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ManageEmployee.Services.Interfaces.Mails;
using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WebMailsController : ControllerBase
{
    private readonly IWebMailService _webMailService;

    public WebMailsController(IWebMailService webMailService)
    {
        _webMailService = webMailService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMail(string? email, string? phoneNumber, string? note)
    {
        int? userId = 0;
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            if( identity?.FindFirst(x => x.Type == "UserId") != null)
            {
                userId = int.Parse(identity?.FindFirst(x => x.Type == "UserId").Value);
            }
        }
        await _webMailService.CreateMail(email, phoneNumber,note, userId);
        return Ok();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPaging([FromQuery] PagingRequestModel param)
    {
        var response = await _webMailService.GetPaging(param);
        return Ok(response);
    }
}