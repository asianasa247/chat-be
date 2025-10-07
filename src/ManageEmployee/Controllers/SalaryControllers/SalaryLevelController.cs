using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Users.Salaries;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.SalaryEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers.SalaryControllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SalaryLevelController : ControllerBase
{
    private ISalaryLevelService _positionService;

    public SalaryLevelController(
        ISalaryLevelService positionService
        )
    {
        _positionService = positionService;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] PagingRequestModel param)
    {
        var model = _positionService.GetAll(param.Page, param.PageSize, param.SearchText);
        var totalItems = _positionService.Count(param.SearchText);
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
    public IActionResult Save([FromBody] SalaryLevel model, int id = 0)
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
                _positionService.Update(model);
            }
            else
            {
                _positionService.Create(model);
            }
            // update user

            return Ok(new ObjectReturn
            {
                message = ResultErrorConstants.SUCCESS,
                status = 200,
                data = model
            });
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
        _positionService.Delete(id);
        return Ok(new ObjectReturn
        {
            message = ResultErrorConstants.SUCCESS,
            status = 200,
        });
    }
}