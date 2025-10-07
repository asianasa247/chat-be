using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Decides;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.DecideEntities;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DecideController : ControllerBase
{
    private readonly IDecideService _contextService;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public DecideController(
        IDecideService decideService,
        IMapper mapper,
         IFileService fileService
        )
    {
        _contextService = decideService;
        _mapper = mapper;
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var model = await _contextService.GetAll(param.Page, param.PageSize, param.SearchText);
        return Ok(model);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _contextService.GetById(id);
        return Ok(model);
    }

    [HttpPut("{id}")]
    [HttpPost]
    public async Task<IActionResult> Save([FromForm] DecideViewModel model, int id = 0)
    {
        // map model to entity and set id
        var decide = _mapper.Map<Decide>(model);
        decide.Id = id;

        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                decide.UserUpdated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
                decide.UserCreated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            }

            decide.FileUrl = "";
            if (model.File != null)
            {
                var fileName = _fileService.Upload(model.File, "Decide", model.File.FileName);
                decide.FileUrl = fileName;
                decide.FileName = model.File.FileName;
            }

            if (decide.Id != 0)
            {
                await _contextService.Update(decide, model.File);
            }
            else
            {
                await _contextService.Create(decide);
            }
            // update user

            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _contextService.Delete(id);
        return Ok();
    }
}