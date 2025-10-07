using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.Menus;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.MenuEntities;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class MenuKpisController : ControllerBase
{
    private IMenuKpiService _MenuKpiService;
    private IMapper _mapper;
    private IFileService _fileService;

    public MenuKpisController(
        IMenuKpiService MenuKpiService,
        IMapper mapper, IFileService fileService)
    {
        _MenuKpiService = MenuKpiService;
        _mapper = mapper;
        _fileService = fileService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] KpiRequestModel param)
    {
       
        return Ok(await _MenuKpiService.GetAll(param.Page, param.PageSize, param.SearchText, param.Type));
    }

    [HttpGet("list")]
    public IActionResult GetMenuKpi()
    {
        var results = _MenuKpiService.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = results,
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var model = _MenuKpiService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MenuKpi model)
    {
        try
        {
            var result = await _MenuKpiService.Create(model);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return Ok(new { code = 400, msg = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] MenuKpi model)
    {
        try
        {
          
            var result = await _MenuKpiService.Update(model);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return Ok(new { code = 400, msg = result });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var result = _MenuKpiService.Delete(id);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return BadRequest(new { msg = result });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }
}
