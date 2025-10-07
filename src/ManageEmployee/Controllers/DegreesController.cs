using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Degrees;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DegreesController : ControllerBase
{
    private readonly IDegreeService _degreeService;

    public DegreesController(
        IDegreeService degreeService)
    {
        _degreeService = degreeService;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] PagingRequestModel param)
    {
        var model = _degreeService.GetAll(param.Page,param.PageSize,param.SearchText);
        var totalItems = _degreeService.Count(param.SearchText);
        return Ok(new BaseResponseModel
        {
            TotalItems = totalItems,
            Data = model,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }


    [HttpGet("list")]
    public IActionResult GetSelectList()
    {
        var model = _degreeService.GetAll();
        return Ok(new BaseResponseModel
        {
            Data = model,
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var model = _degreeService.GetById(id);
        return Ok(model);
    }

    [HttpPut("{id}")]
    [HttpPost]
    public IActionResult Save(int id, [FromBody] Degree degree)
    {
        // map model to entity and set id
        degree.Id = id;

        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                degree.UserUpdated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
                degree.UserCreated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            }
            if (degree.Id != 0)
            {
                _degreeService.Update(degree);
            }
            else
            {
                _degreeService.Create(degree);
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
        _degreeService.Delete(id);
        return Ok();
    }
}
