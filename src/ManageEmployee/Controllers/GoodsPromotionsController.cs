using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Goods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GoodsPromotionsController : ControllerBase
{
    private readonly IGoodsPromotionService _goodsPromotionService;

    public GoodsPromotionsController(IGoodsPromotionService goodsPromotionService)
    {
        _goodsPromotionService = goodsPromotionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var result = await _goodsPromotionService.GetPaging(param);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _goodsPromotionService.GetList();
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromHeader] int yearFilter, int id)
    {
        var result = await _goodsPromotionService.GetById(id, yearFilter);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GoodsPromotionSetterModel model)
    {
        await _goodsPromotionService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] GoodsPromotionSetterModel model)
    {
        await _goodsPromotionService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _goodsPromotionService.Delete(id);
        return Ok();
    }

    [HttpGet("promotions-for-sale")]
    public async Task<IActionResult> GetListForSale()
    {
        var response = await _goodsPromotionService.GetListForSale();
        return Ok(response);
    }
}