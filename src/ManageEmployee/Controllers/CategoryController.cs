using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.CategoryModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(
        ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestTypeModel param)
    {
        var result = await _categoryService.GetAll(param.Page, param.PageSize, param.SearchText, param.Type);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetCategory()
    {
        var categories = await _categoryService.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = categories,
        });
    }

    [HttpGet("list-with-types")]
    public async Task<IActionResult> GetCategory([FromQuery] List<int> types)
    {
        var categories = await _categoryService.GetAll(types);

        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _categoryService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CategoryModel model)
    {
        try
        {
            var result = await _categoryService.Create(model);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return BadRequest(new { msg = result });
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] CategoryModel model)
    {
        try
        {
            var result = await _categoryService.Update(model);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return BadRequest(new { msg = result });
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _categoryService.DeleteAsync(id);
        return Ok();
    }

    [HttpGet("{code}/goods/existing")]
    public async Task<IActionResult> CheckExistInGoodsAsync(string? code)
    {
        return Ok(await _categoryService.CheckExistInGoodsAsync(code));
    }

    [HttpPost("import")]
    public async Task<IActionResult> ImportAsync(List<CategoryImport> form)
    {
        await _categoryService.ImportAsync(form);
        return Ok(new ObjectReturn
        {
            status = 200,
        });
    }

    [HttpGet("export")]
    public async Task<IActionResult> ExportAsync(int type = 0)
    {
        var response = await _categoryService.ExportAsync(type);
        return Ok(new
        {
            data = response
        });
    }
}