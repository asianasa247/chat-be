using System.Security.Claims;
using AutoMapper;
using ManageEmployee.DataTransferObject.DocumentModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DocumentType2Controller : ControllerBase
{
    private readonly IDocumentType2Service _contextService;
    private readonly IFileService _fileService;
    private  readonly IMapper _mapper;

    public DocumentType2Controller(
        IDocumentType2Service documentType2Service,
        IMapper mapper,
         IFileService fileService
        )
    {
        _contextService = documentType2Service;
        _mapper = mapper;
        _fileService = fileService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DocumentType1RequestModel param)
    {
        var res = await _contextService.GetAll(param);
        return Ok(res);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _contextService.GetByIdAsyncd(id);
        return Ok(model);
    }
    [HttpPut("{id}")]
    [HttpPost]
    public async Task<IActionResult> Save([FromForm] DocumentType2ViewModel model, int id =0)
    {
        // map model to entity and set id
        var document = _mapper.Map<DocumentType2>(model);
        document.Id = id;

        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                document.UserUpdated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
                document.UserCreated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            }

            document.FileUrl = "";
            if (model.File != null)
            {
                var fileName = _fileService.Upload(model.File, "DocumentType2", model.File.FileName);
                document.FileUrl = fileName;
                document.FileName = model.File.FileName;
            }

            if (document.Id != 0)
            {
                await _contextService.UpdateAsync(document);
            }
            else
            {
                await _contextService.CreateAsync(document);
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
        await _contextService.DeleteAsync(id);
        return Ok();
    }
}
