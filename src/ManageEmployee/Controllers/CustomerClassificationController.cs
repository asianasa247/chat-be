using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Customers;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CustomerClassificationController : ControllerBase
{
    private ICustomerClassificationService _customerClService;

    public CustomerClassificationController(ICustomerClassificationService customerClService)
    {
        _customerClService = customerClService;
    }
    [HttpGet("list")]
    public IActionResult GetAll()
    {
        var jobs = _customerClService.GetAll();
        return Ok(new BaseResponseModel()
        {
            Data = jobs,
        });
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] PagingRequestModel param)
    {
        var jobs = _customerClService.GetAll(param.Page, param.PageSize, param.SearchText);
        var totalItems = jobs.Count;
        return Ok(new BaseResponseModel
        {
            TotalItems = totalItems,
            Data = jobs,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var model = _customerClService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CustomerClassification model)
    {
        try
        {
            
                _customerClService.Create(model);
            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }
    [HttpPut("{id}")]
    public IActionResult Update([FromBody] CustomerClassification model)
    {
        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                model.UserUpdated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
                model.UserCreated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            }
                _customerClService.Update(model);
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
        try
        {
            _customerClService.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { msg = ex.Message });
        }

    }
}
