using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Configurations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[ApiController]
[Route("api/configuration")]
[Authorize]
public class ConfigurationController: ControllerBase
{
    private readonly IConfigurationService _configurationService;

    public ConfigurationController(IConfigurationService configurationService)
    {
        _configurationService = configurationService;
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> GetAsync(string? key)
    {
        
        var result = await _configurationService.GetAsync<Dictionary<string, string>>(key, HttpContext.GetIdentityUser().Id);
        return Ok(result);
    }
    
    [HttpPost("{key}")]
    public async Task<IActionResult> SetAsync(string? key, [FromBody] Dictionary<string, string> data)
    {
        await _configurationService.SetAsync(key, data, HttpContext.GetIdentityUser().Id);
        return Ok();
    }
}