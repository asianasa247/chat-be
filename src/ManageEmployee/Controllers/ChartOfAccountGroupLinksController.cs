using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChartOfAccountGroupLinksController : ControllerBase
{
    private readonly IChartOfAccountGroupLinkService _ChartOfAccountGroupLinkService;
    private readonly IMapper _mapper;

    public ChartOfAccountGroupLinksController(
        IChartOfAccountGroupLinkService ChartOfAccountGroupLinkService,
        IMapper mapper) 
    {
        _ChartOfAccountGroupLinkService = ChartOfAccountGroupLinkService;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] PagingRequestModel param, [FromHeader] int yearFilter)
    {
        var model = _ChartOfAccountGroupLinkService.GetAll(param.Page, param.PageSize, yearFilter).ToList();
        return Ok(new BaseResponseModel
        {
            TotalItems = model.Count(),
            Data = model,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }


    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var model = _ChartOfAccountGroupLinkService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public IActionResult Create([FromBody] ChartOfAccountGroupLink model, [FromHeader] int yearFilter)
    {
        // map model to entity and set id
        var ChartOfAccountGroupLink = _mapper.Map<ChartOfAccountGroupLink>(model);

        try
        {
            // update user 
            _ChartOfAccountGroupLinkService.Create(ChartOfAccountGroupLink, yearFilter);
            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] ChartOfAccountGroupLink model, [FromHeader] int yearFilter)
    {
        // map model to entity and set id
        var ChartOfAccountGroupLink = _mapper.Map<ChartOfAccountGroupLink>(model);
        ChartOfAccountGroupLink.Id = id;

        try
        {
            // update user 
            _ChartOfAccountGroupLinkService.Update(ChartOfAccountGroupLink, yearFilter);
            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id, [FromHeader] int yearFilter)
    {
        _ChartOfAccountGroupLinkService.Delete(id);
        return Ok();
    }

    [HttpGet("available-selection")]
    public async Task<IActionResult> GetAllAccountGroupLinks([FromHeader] int yearFilter)
    {
        var data = await _ChartOfAccountGroupLinkService.GetAllAccountGroupLinks(yearFilter);
        return Ok(data);
    }
}
