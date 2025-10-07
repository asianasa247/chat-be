using ManageEmployee.DataTransferObject.GoodsModels.GoodsQuotaModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Goods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.GoodsQuotaControllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class GoodsQuotasController : ControllerBase
{
    private readonly IGoodsQuotaService _goodsQuotaService;
    public GoodsQuotasController(IGoodsQuotaService goodsQuotaService)
    {
        _goodsQuotaService = goodsQuotaService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetPaging([FromQuery] GoodsQuotasRequestModel param)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        var response = await _goodsQuotaService.GetPaging(param, userId);
        return Ok(response);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll()
    {
        var response = await _goodsQuotaService.GetAll();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail([FromRoute] int id)
    {
        var response = await _goodsQuotaService.GetDetail(id);
        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GoodsQuotaModel model)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        await _goodsQuotaService.Create(model, userId);
        return Ok();
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] GoodsQuotaModel model)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        await _goodsQuotaService.Update(model, userId);
        return Ok();
    }

    [HttpPost("goods-quota-for-goods-detail")]
    public async Task<IActionResult> GoodsQuotaForGoodsDetail([FromBody] List<int> goodIds, [FromQuery] int goodsQuotaId)
    {
        await _goodsQuotaService.GoodsQuotaForGoodsDetail(goodIds, goodsQuotaId);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept(int id)
    {
        var identityUser = HttpContext.GetIdentityUser();
        await _goodsQuotaService.Accept(id, identityUser.Id);
        return Ok();
    }

    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        var identityUser = HttpContext.GetIdentityUser();
        await _goodsQuotaService.NotAccept(id, identityUser.Id);
        return Ok();
    }

    [HttpGet("procedure-number")]
    public async Task<IActionResult> GetProcedureNumber()
    {
        var result = await _goodsQuotaService.GetProcedureNumber();
        return Ok(result);
    }

}
