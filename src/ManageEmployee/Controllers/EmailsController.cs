using ManageEmployee.Emails;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmailsController : ControllerBase
{
    private readonly IEmailLogin _emailLogin;
    public EmailsController(IEmailLogin emailLogin)
    {
        _emailLogin = emailLogin;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        await _emailLogin.GetProfileFromEmail();
        return Ok();
    }
}
