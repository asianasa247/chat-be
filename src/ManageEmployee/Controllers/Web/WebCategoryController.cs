using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Services.Interfaces.Categories;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.Web;

[Route("api/[controller]")]
[ApiController]
public class WebCategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public WebCategoryController(
        ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("getAll")]
    public async Task<IActionResult> GetCategory()
    {
        var categories = await _categoryService.GetCategoryForWeb();

        return Ok(new CommonWebResponse()
        {
            State = true,
            Code = 200,
            Message = "",
            Data = categories
        });
    }
}