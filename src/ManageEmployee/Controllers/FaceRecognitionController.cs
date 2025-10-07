using ManageEmployee.Services.Interfaces.FaceRecognitions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FaceRecognitionController : ControllerBase
{
    private readonly IFaceRecognitionService _faceRecognitionService;
    public FaceRecognitionController(IFaceRecognitionService faceRecognitionService)
    {
        _faceRecognitionService = faceRecognitionService;
    }
    
    [HttpPost("training")]
    [AllowAnonymous]
    public async Task<IActionResult> FaceTraining([FromQuery] string dbName)
    {
        await _faceRecognitionService.Training(dbName);
        return Ok();
    }
    
    [HttpPost("identify-face-from-image")]
    public async Task<IActionResult> IdentifyFace(IFormFile file)
    {
        // Accessing a specific header
        if (Request.Headers.TryGetValue("dbName", out StringValues dbName))
        {
            var user = await _faceRecognitionService.DetectAndIdentifyFacesAsync(dbName, file);
            return Ok(user);
        }
        throw new Exception("Không tìm thấy database");
    }
}