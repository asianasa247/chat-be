using Common.Extensions;
using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.Services.Interfaces.Assets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost]
    public IActionResult Create([FromForm] FileModel model)
    {
        try
        {
            var img = _fileService.Upload(HttpContext.Request.Form.Files[0], model.folderName);
            return Ok(new { url = img.FilePathAsUrl() });
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }
}