using ManageEmployee.DataTransferObject.AriseModels;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/config-arise-behaviour")]
public class ConfigAriseBehaviourController : Controller
{
    private readonly IConfigAriseBehaviourService _configAriseBehaviourService;

    public ConfigAriseBehaviourController(
        IConfigAriseBehaviourService configAriseBehaviourService)
    {
        _configAriseBehaviourService = configAriseBehaviourService;
    }

    [HttpGet("documents/{documentId}")]
    public async Task<IActionResult> GetAllAsync(int documentId)
    {
        try
        {
            return Ok(await _configAriseBehaviourService.GetAllByDocumentAsync(documentId));
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }
    [HttpPost("preparation-documents/{documentId}")]
    public async Task<IActionResult> PreparationAriseDocumentBehaviourAsync(int documentId)
    {
        try
        {
            return Ok(await _configAriseBehaviourService.PreparationAriseDocumentBehaviourAsync(documentId));
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{ariseBehaviourId}/document-no-keep-value")]
    public async Task<IActionResult> UpdateNoKeepValueAsync(int ariseBehaviourId, [FromBody] ConfigAriseDocumentBehaviourInputDto input)
    {
        try
        {
            await _configAriseBehaviourService.UpdateNoKeepValueAsync(ariseBehaviourId, input);
            return Ok();
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{ariseBehaviourId}/document-focus-value")]
    public async Task<IActionResult> UpdateFocusValueAsync(int ariseBehaviourId, [FromBody] ConfigAriseDocumentBehaviourInputDto input)
    {
        try
        {
            await _configAriseBehaviourService.UpdateFocusValueAsync(ariseBehaviourId, input);
            return Ok();
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }
}
