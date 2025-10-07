using ManageEmployee.DataTransferObject.AdditionWebs;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.Services.Interfaces.AdditionWebServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.AdditionWebControllers;

[ApiController]
[Route("api/AdditionWeb")]
[Authorize]
public class AdditionWebGoodsController : ControllerBase
{
    private readonly IAdditionWebGoodsService _additionWebGoodsService;
    public AdditionWebGoodsController(IAdditionWebGoodsService additionWebProductService)
    {
        _additionWebGoodsService = additionWebProductService;
    }

    [HttpGet("{id:int}/goods")]
    public async Task<IActionResult> GetAllGoodsByWebId(
        [FromRoute] int id, 
        [FromQuery] SearchViewModel param, 
        [FromQuery] int? yearFilter = null)
    {
        var result = await _additionWebGoodsService.GetAllGoodsByWebId(id, param, yearFilter ?? DateTime.Now.Year);
        return Ok(new BaseResponseModel
        {
            TotalItems = result.TotalItems,
            Data = result.Goods,
            PageSize = result.PageSize,
            CurrentPage = result.pageIndex
        });
    }

    [AllowAnonymous]
    [HttpGet("{id:int}/goods/selected")]
    public async Task<IActionResult> GetGoodsSelected(
        [FromRoute] int id, 
        [FromQuery] SearchViewModel param, 
        [FromQuery] int? yearFilter = null)
    {
        var result = await _additionWebGoodsService.GetAllGoodsSelectedByWebId(id, param, yearFilter ?? DateTime.Now.Year);

        return Ok(new BaseResponseModel
        {
            TotalItems = result.TotalItems,
            Data = result.Goods,
            PageSize = result.PageSize,
            CurrentPage = result.pageIndex
        });
    }

    [HttpPost("{id:int}/goods")]
    public async Task<IActionResult> SaveGoods(
        [FromRoute] int id,
        [FromBody] AdditionWebGoodsRequestModel body)
    {
        var saved = await _additionWebGoodsService.SaveGoodsToAdditionWebAsync(id, body);

        return Ok(new
        {
            webId = id,
            appended = saved
        });
    }

    [HttpDelete("{id:int}/goods/{goodsId}")]
    public async Task<IActionResult> RemoveGoods(
       [FromRoute] int id,
       [FromRoute] int goodsId)
    {
        await _additionWebGoodsService.RemoveGoods(id, goodsId);
        return Ok();
    }

    [HttpPost("{id:int}/goods/remove-selected")]
    public async Task<IActionResult> RemoveGoodsSelected(
       [FromRoute] int id,
       [FromBody] AdditionWebGoodsRemoveRequestModel body)
    {
        await _additionWebGoodsService.RemoveGoodsSelected(id, body.GoodsIds);
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("goods/selected")]
    public async Task<ActionResult<IEnumerable<AdditionWebSelectedGroupResult>>> GetAllSelected(
       [FromQuery] SearchViewModel param,
       [FromQuery] int? yearFilter = null)
    {
        var result = await _additionWebGoodsService.GetAllGoodsSelectedAsync(param, yearFilter ?? DateTime.Now.Year);
        return Ok(result);
    }
}
