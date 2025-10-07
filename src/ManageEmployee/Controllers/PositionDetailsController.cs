using AutoMapper;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Services.Interfaces.Positions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PositionDetailsController : ControllerBase
{
    private IPositionDetailService _positionService;
    private IMapper _mapper;

    public PositionDetailsController(
        IPositionDetailService positionService, IMapper mapper
        )
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
    public async Task<IActionResult> GetSelectList()
    {
        var departments = await _positionService.GetAll();
        return Ok(new BaseResponseModel
        {
            Data = departments,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var positionDetail = await _positionService.GetById(id);
        var model = _mapper.Map<PositionDetail>(positionDetail);
        return Ok(model);
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PositionDetail model)
    {
        await _positionService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] PositionDetail model)
    {
        // map model to entity and set id

        await _positionService.Update(model);

        return Ok();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _positionService.Delete(id);
        return Ok();
    }
}
