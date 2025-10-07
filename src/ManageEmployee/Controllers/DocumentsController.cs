using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Documents;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.DataTransferObject.DocumentModels;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _documentService;
    private readonly IMapper _mapper;

    public DocumentsController(
        IMapper mapper,
       IDocumentService documentService)
    {
        _mapper = mapper;
        _documentService = documentService;
    }

    [HttpGet()]
    public IActionResult GetAll([FromQuery] PagingRequestModel param)
    {
        var document = _documentService.GetAll(param.SearchText);
        var totalItems = document.Count();

        return Ok(new BaseResponseModel
        {
            TotalItems = totalItems,
            Data = document.Skip(param.PageSize * (param.Page - 1)).Take(param.PageSize),
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpGet("list")]
    public IActionResult GetAllByActive()
    {
        var userId = "";
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            userId = identity.FindFirst(x => x.Type == "UserId").Value;
        }

        var result = _documentService.GetAllByUser(userId);
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var document = _documentService.GetById(id);
        var model = _mapper.Map<DocumentModel.MapDocument>(document);
        return Ok(model);
    }

    [HttpPost]
    [HttpPut("{id}")]
    public IActionResult Save([FromBody] DocumentModel.MapDocument model, int id = 0)
    {
        // map model to entity and set id
        var currenDocument = _mapper.Map<Document>(model);
        currenDocument.Id = id;

        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                currenDocument.UserUpdated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
                currenDocument.UserCreated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            }
            if (currenDocument.Id != 0)
            {
                if (_documentService.GetAll().Where(x => x.Id != id && x.Code.ToLower() == currenDocument.Code.ToLower()).Any())
                {
                    return Ok(new ObjectReturn
                    {
                        message = ResultErrorConstants.CODE_EXIST,
                        status = Convert.ToInt32(ErrorEnum.SUCCESS)
                    });
                }
                _documentService.Update(currenDocument);
            }
            else
            {
                if (_documentService.GetAll().Where(x => x.Code.ToLower() == currenDocument.Code.ToLower()).Any())
                {
                    return Ok(new ObjectReturn
                    {
                        message = ResultErrorConstants.CODE_EXIST,
                        status = Convert.ToInt32(ErrorEnum.SUCCESS)
                    });
                }
                currenDocument.Stt = _documentService.GetLastIdentity();
                _documentService.Create(currenDocument);
            }
            // update user

            return Ok(new ObjectReturn
            {
                message = "",
                status = Convert.ToInt32(ErrorEnum.SUCCESS)
            });
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return Ok(new ObjectReturn
            {
                message = ex.Message,
                status = Convert.ToInt32(ErrorEnum.BAD_REQUEST)
            });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _documentService.Delete(id);
        return Ok(new ObjectReturn
        {
            message = "",
            status = Convert.ToInt32(ErrorEnum.SUCCESS)
        });
    }
}