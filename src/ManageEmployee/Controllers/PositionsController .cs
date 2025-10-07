using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Positions;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PositionsController : ControllerBase
{
    private IPositionService _positionService;
    private IMapper _mapper;

    public PositionsController(
        IPositionService positionService,
        IMapper mapper)
    {
        _positionService = positionService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        return Ok(await _positionService.GetAll(param.Page, param.PageSize, param.SearchText));
    }


    [HttpGet("list")]
    public IActionResult GetSelectList()
    {
        var positions = _positionService.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = positions,
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var model = _positionService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Position model)
    {
        // map model to entity and set id
        var position = _mapper.Map<Position>(model);
        try
        {
            _positionService.Create(position);

            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return Ok(new { code = 400, msg = ex.Message });
        }
    }
    [HttpPut("{id}")]
    public IActionResult Save(int id, [FromBody] Position model)
    {
        // map model to entity and set id
        var position = _mapper.Map<Position>(model);
        try
        {
            _positionService.Update(position);

            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return Ok(new { code = 400, msg = ex.Message });
        }
    }
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _positionService.Delete(id);
        return Ok();
    }
}
