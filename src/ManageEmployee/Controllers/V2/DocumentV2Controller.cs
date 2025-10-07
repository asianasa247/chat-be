using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ManageEmployee.Services.Interfaces.Documents;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers.V2;

[Authorize]
[ApiController]
[Route("api/v2/document")]
public class DocumentV2Controller : ControllerBase
{
    private readonly IDocumentV2Service _documentService;

    public DocumentV2Controller(
       IDocumentV2Service documentService) 
    {
        _documentService = documentService;
    }


    [HttpGet("by-current-user")]
    public async Task<IActionResult> GetByCurrentUserAsync([FromHeader] int yearFilter)
    {
        var userId = "";
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            userId = identity.FindFirst(x => x.Type == "UserId").Value;
        }
        var result = await _documentService.GetByUserAsync(userId, yearFilter);
        return Ok(new BaseResponseModel
        {
            Data = result,
        });

    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id, [FromHeader] int yearFilter)
    {
        try
        {
            var result = await _documentService.GetByIdAsync(id, yearFilter);
            if (result is null)
            {
                return BadRequest(new { message = "Không tìm thấy dữ liệu" });
            }
            return Ok(result);
        }
        catch(Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}