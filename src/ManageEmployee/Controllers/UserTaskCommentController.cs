using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.Users;
using ManageEmployee.Extends;
using ManageEmployee.DataTransferObject.UserModels;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserTaskCommentController : ControllerBase
{
    private readonly IUserTaskCommentService _userTaskCommentService;
    private readonly IFileService _fileService;

    public UserTaskCommentController(IUserTaskCommentService userTaskCommentService, IFileService fileService)
    {
        _userTaskCommentService = userTaskCommentService;
        _fileService = fileService;
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] UserTaskCommentModel param)
    {
        var currentUser = HttpContext.GetIdentityUser();
        param.UserId = currentUser.Id;
        param.Type = "edit";
        var fileLink = param.FileLink;
        var result = await _userTaskCommentService.Add(param, fileLink);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UserTaskCommentModel model)
    {
        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                var result = await _userTaskCommentService.Edit(model);
                if (result != null)
                    return Ok(result);
                else
                    return NotFound(result);
            }
            else
            {
                return BadRequest("Bạn không có quyền truy cập.");
            }
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpGet("GetByTask")]
    public async Task<IActionResult> GetByTask(int id)
    {
        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                var result = await _userTaskCommentService.GetByTaskId(id, int.Parse(identity.FindFirst(x => x.Type == "UserId").Value));
                if (result != null)
                    return Ok(result);
                return NotFound();
            }
            return BadRequest("Bạn không có quyền truy cập.");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("uploadfile")]
    public IActionResult UploadFile([FromForm] IFormFile file)
    {
        try
        {
            if (file != null)
            {
                string fileType = System.IO.Path.GetExtension(file.FileName);
                string FileId = Guid.NewGuid().ToString();
                var fileName = _fileService.Upload(file, "usertask/", FileId + fileType);
                UserTaskFileModel fileUpload = new UserTaskFileModel()
                {
                    FileId = FileId + fileType,
                    FileName = fileName
                };
                return Ok(fileUpload);
            }
            else
            {
                return BadRequest("Bạn không có quyền truy cập.");
            }
        }
        catch (System.Exception ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }
}