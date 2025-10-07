using ManageEmployee.DataTransferObject.NumberOfMealModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.InOuts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.InOutControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NumberOfMealsController : ControllerBase
{
    private readonly INumberOfMealService _numberOfMealService;

    public NumberOfMealsController(INumberOfMealService numberOfMealService)
    {
        _numberOfMealService = numberOfMealService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaging([FromQuery] PagingRequestFilterDateModel param)
    {
        var result = await _numberOfMealService.GetPaging(param);
        return Ok(result);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetDetail(DateTime date, string timeType)
    {
        var result = await _numberOfMealService.GetDetail(date, timeType);
        return Ok(result);
    }

    [HttpPost("detail")]
    public async Task<IActionResult> SetDetail([FromBody] NumberOfMealDetailModel form)
    {
        await _numberOfMealService.SetDetail(form);
        return Ok();
    }

    [HttpDelete("{mealId}")]
    public async Task<IActionResult> DeleteMeal([FromRoute] int mealId)
    {
        var result = await _numberOfMealService.DeleteMeal(mealId);
        return result ? Ok() : BadRequest();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteMeals([FromBody] List<int> mealIds)
    {
        var result = await _numberOfMealService.DeleteMeals(mealIds);
        return result ? Ok() : BadRequest();
    }

    [HttpDelete("detail/{mealDetailId}")]
    public async Task<IActionResult> DeleteMealDetails([FromRoute] int mealDetailId)
    {
        var result = await _numberOfMealService.DeleteMealDetail(mealDetailId);
        return result ? Ok() : BadRequest();
    }

    [HttpDelete("details")]
    public async Task<IActionResult> DeleteMealDetails([FromBody] List<int> mealDetailIds)
    {
        var result = await _numberOfMealService.DeleteMealDetails(mealDetailIds);
        return result ? Ok() : BadRequest();
    }

    [HttpPost("meal")]
    public async Task<IActionResult> UpdateMeal(DateTime date)
    {
        await _numberOfMealService.UpdateMeal(date);
        return Ok();
    }
}