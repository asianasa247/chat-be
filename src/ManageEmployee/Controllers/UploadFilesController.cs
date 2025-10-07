using ManageEmployee.Services.Interfaces.Assets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UploadFilesController : ControllerBase
{
    private readonly IFileService _fileService;

    public UploadFilesController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost]
    public IActionResult Uploadfile([FromForm] IFormFile file, string controllerName)
    {
        var response = _fileService.UploadFile(file, controllerName, file.FileName);
        return Ok(response);
    }
}