using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Services.Interfaces.Goods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.GoodsQuotaControllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class GoodsQuotaRecipesController : ControllerBase
{
    private readonly IGoodsQuotaRecipeService _goodsQuotaRecipeService;
    public GoodsQuotaRecipesController(IGoodsQuotaRecipeService goodsQuotaRecipeService)
    {
        _goodsQuotaRecipeService = goodsQuotaRecipeService;
    }
    [HttpGet()]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var response = await _goodsQuotaRecipeService.GetPaging(param);
        return Ok(response);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _goodsQuotaRecipeService.GetAll();
        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GoodsQuotaRecipe model)
    {
        await _goodsQuotaRecipeService.Create(model);
        return Ok();
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] GoodsQuotaRecipe model)
    {
        await _goodsQuotaRecipeService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _goodsQuotaRecipeService.Delete(id);
        return Ok();
    }
}
