using AutoMapper;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.Web;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class IsoftHistoryController : ControllerBase
{
    private readonly IIsoftHistoryService _isoftHistoryService;
    private readonly IMapper _mapper;

    public IsoftHistoryController(
        IIsoftHistoryService isoftHistoryService,
        IMapper mapper)
    {
        _isoftHistoryService = isoftHistoryService;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] PagingRequestTypeModel param)
    {
        var result = _isoftHistoryService.GetAll(param.Page, param.PageSize, param.SearchText);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var item = _isoftHistoryService.GetById(id);
        var model = _mapper.Map<IsoftHistory>(item);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] IsoftHistory model)
    {
        try
        {
            model.CreatedAt = DateTime.Now;
            model.UpdatedAt = DateTime.Now;
            model.IsDelete = false;
            var result = await _isoftHistoryService.Create(model);
            if (string.IsNullOrEmpty(result))
                return Ok(new ObjectReturn
                {
                    data = model,
                    status = 200
                });
            return BadRequest(new { msg = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] IsoftHistory model)
    {
        try
        {
            model.CreatedAt = DateTime.Now;
            var result = await _isoftHistoryService.Update(model);
            if (string.IsNullOrEmpty(result))
                return Ok(new ObjectReturn
                {
                    data = model,
                    status = 200
                });
            return BadRequest(new { msg = result });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _isoftHistoryService.Delete(id);
        return Ok(new ObjectReturn
        {
            data = id,
            status = 200
        });
    }

    [AllowAnonymous]
    [HttpGet("ByClassName")]
    public IActionResult GetByClassName(string? className, string? keyword)
    {
        var item = _isoftHistoryService.GetHistoryByClassName(className, keyword);
        var model = _mapper.Map<HistoryByClassNameModel>(item);
        return Ok(model);
    }
}