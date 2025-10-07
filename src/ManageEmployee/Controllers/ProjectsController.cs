using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.ProjectModels;
using ManageEmployee.Services.Interfaces.Projects;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        return Ok(await _projectService.GetAll(param.Page, param.PageSize, param.SearchText));
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetListData()
    {
        var results = await _projectService.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = results,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _projectService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProjectModel model)
    {
        try
        {
            var result = await _projectService.Create(model);
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
    public async Task<IActionResult> Update([FromBody] ProjectModel model)
    {

        var result = await _projectService.Update(model);
        if (string.IsNullOrEmpty(result))
            return Ok();
        return BadRequest(new { msg = result });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _projectService.Delete(id);
        if (string.IsNullOrEmpty(result))
            return Ok();
        return BadRequest(new { msg = result });
    }
}
