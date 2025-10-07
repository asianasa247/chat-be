using System.Security.Claims;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.DecideEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.DecisionTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DecisionTypeController : ControllerBase
{
    private readonly IDecisionTypeService _decisionTypeService;

    public DecisionTypeController(
        IDecisionTypeService decisionTypeService
        )
    {
        _decisionTypeService = decisionTypeService;
    }
    [HttpGet]
    public IActionResult GetAll([FromQuery] PagingRequestModel param)
    {
        var model = _decisionTypeService.GetAll(param.Page, param.PageSize, param.SearchText);
        var totalItems = _decisionTypeService.Count(param.SearchText);
        return Ok(new BaseResponseModel
        {
            TotalItems = totalItems,
            Data = model,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpPut("{id}")]
    [HttpPost]
    public IActionResult Save(int id, [FromBody] DecisionType model)
    {
        // map model to entity and set id
        model.Id = id;

        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                model.UserUpdated = int.Parse(identity.FindFirst(ClaimTypes.Name).Value);
                model.UserCreated = int.Parse(identity.FindFirst(ClaimTypes.Name).Value);
            }
            if (model.Id != 0)
            {
                _decisionTypeService.Update(model);
            }
            else
            {
                _decisionTypeService.Create(model);
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

    [HttpPost("delete/{id}")]
    public IActionResult Delete(int id)
    {
        _decisionTypeService.Delete(id);
        return Ok();
    }
    
    [HttpGet("list")]
    public IActionResult GetAllList()
    {
        var model = _decisionTypeService.GetAllList();
        return Ok(new BaseResponseModel
        {   
            Data = model
        });
    }
}
