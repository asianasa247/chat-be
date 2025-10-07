using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Targets;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]

public class TargetsController : ControllerBase
{
    private ITargetService _targetService;
    private IMapper _mapper;

    public TargetsController(
        ITargetService targetService,
        IMapper mapper)
    {
        _targetService = targetService;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] PagingRequestModel param)
    {
        var model = _targetService.GetAll(param.Page, param.PageSize, param.SearchText);

        return Ok(new BaseResponseModel
        {
            TotalItems = model.Count(),
            Data = model,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }


    [HttpGet("list")]
    public IActionResult GetSelectList()
    {
        var targets = _targetService.GetAll();

        var model = _mapper.Map<IList<Target>>(targets);
        return Ok(new BaseResponseModel
        {
            Data = model,
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var model = _targetService.GetById(id);
        return Ok(model);
    }
    [HttpPost]
    public IActionResult Create([FromBody] Target target)
    {
        // map model to entity and set id
        try
        {
            _targetService.Create(target);
            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Target target)
    {
        // map model to entity and set id
        try
        {
            _targetService.Update(target);
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
        _targetService.Delete(id);
        return Ok();
    }
}
