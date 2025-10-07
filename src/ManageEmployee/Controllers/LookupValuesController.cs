using ManageEmployee.Services.Interfaces.LookupValues;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class LookupValuesController : ControllerBase
{
    private readonly ILookupValueService _lookupValueService;

    public LookupValuesController(ILookupValueService lookupValueService)
    {
        _lookupValueService = lookupValueService;
    }

    [HttpGet]
    public async Task<IActionResult> LookupValues(string? scope)
    {
        return Ok(await _lookupValueService.GetLookupValues(scope));
    }
}