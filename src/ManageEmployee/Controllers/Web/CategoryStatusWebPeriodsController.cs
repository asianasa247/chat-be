using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ManageEmployee.Services.Interfaces.Categories;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.CategoryModels;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers.Web;

[ApiController]
[Route("api/[controller]")]
public class CategoryWebsController : ControllerBase
{
    private readonly ICategoryStatusWebPeriodService _categoryStatusWebPeriodService;

    public CategoryWebsController(
        ICategoryStatusWebPeriodService categoryStatusWebPeriodService)
    {
        _categoryStatusWebPeriodService = categoryStatusWebPeriodService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var model = await _categoryStatusWebPeriodService.GetAll(param.Page, param.PageSize);
        return Ok(model);
    }

    [HttpGet("list-category")]
    public async Task<IActionResult> GetListCategory()
    {
        var model = await _categoryStatusWebPeriodService.GetListCategoryStatusWeb();
        return Ok(new BaseResponseModel
        {
            Data = model,
        });
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetSelectList()
    {
        var model = await _categoryStatusWebPeriodService.GetAll();
        return Ok(new BaseResponseModel
        {
            Data = model,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _categoryStatusWebPeriodService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryStatusWebPeriodModel form)
    {
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            form.UserId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }

        await _categoryStatusWebPeriodService.Create(form);

        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryStatusWebPeriodModel form)
    {
        // map model to entity and set id
        form.Id = id;
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            form.UserId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }
        await _categoryStatusWebPeriodService.Update(form);

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryStatusWebPeriodService.Delete(id);
        return Ok();
    }
}