using System.Security.Claims;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Documents;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DocumentTypeController : ControllerBase
{
    private IDocumentTypeService _documentTypeService;

    public DocumentTypeController(
        IDocumentTypeService documentTypeService
        )
    {
        _documentTypeService = documentTypeService;
    }
    [HttpGet]
    public IActionResult GetAll([FromQuery] PagingRequestModel param)
    {
        var model = _documentTypeService.GetAll(param.Page, param.PageSize, param.SearchText);
        var totalItems = _documentTypeService.Count(param.SearchText);
        return Ok(new BaseResponseModel
        {
            TotalItems = totalItems,
            Data = model,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpGet("list")]
    public IActionResult GetAllByActive()
    {
        var result = _documentTypeService.GetAllByActive();
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpPut("{id}")]
    [HttpPost]
    public IActionResult Save(int id, [FromBody] DocumentType model)
    {
        // map model to entity and set id
        model.Id = id;

        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                model.UserUpdated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
                model.UserCreated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            }
            if (model.Id != 0)
            {
                _documentTypeService.Update(model);
            }
            else
            {
                _documentTypeService.Create(model);
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
    public IActionResult Delete(int id)
    {
        _documentTypeService.Delete(id);
        return Ok();
    }
}
