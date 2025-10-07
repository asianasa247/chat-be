using AutoMapper;
using ManageEmployee.DataTransferObject.DocumentModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DocumentType1Controller : ControllerBase
{
    private readonly IDocumentType1Service _contextService;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public DocumentType1Controller(
        IDocumentType1Service documentType1Service,
         IMapper mapper,
         IFileService fileService
        )
    {
        _contextService = documentType1Service;
        _mapper = mapper;
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DocumentType1RequestModel param)
    {
        var result = await _contextService.GetAll(param);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _contextService.GetByIdAsync(id);
        return Ok(model);
    }

    [HttpPut("{id}")]
    [HttpPost]
    public async Task<IActionResult> Save([FromForm] DocumentType1ViewModel model, int id = 0)
    {
        // map model to entity and set id
        var document = _mapper.Map<DocumentType1>(model);
        document.Id = id;

        document.FileUrl = "";
        if (model.File != null)
        {
            var fileName = _fileService.Upload(model.File, "DocumentType1", model.File.FileName);
            document.FileUrl = fileName;
            document.FileName = model.File.FileName;
        }
        var identityUser = HttpContext.GetIdentityUser();

        document.UserUpdated = identityUser.Id;
        document.UserCreated = identityUser.Id;
        if (document.Id != 0)
        {
            await _contextService.UpdateAsync(document);
        }
        else
        {
            await _contextService.CreateAsync(document);
        }
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _contextService.DeleteAsync(id);
        return Ok();
    }
}