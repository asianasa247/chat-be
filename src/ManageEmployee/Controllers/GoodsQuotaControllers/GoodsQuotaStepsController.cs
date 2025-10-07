using ManageEmployee.DataTransferObject.GoodsModels.GoodsQuotaModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Goods;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.GoodsQuotaControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GoodsQuotaStepsController : ControllerBase
{
    private readonly IGoodsQuotaStepService _goodsQuotaStepService;

    public GoodsQuotaStepsController(IGoodsQuotaStepService goodsQuotaStepService)
    {
        _goodsQuotaStepService = goodsQuotaStepService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var response = await _goodsQuotaStepService.GetPaging(param);
        return Ok(response);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _goodsQuotaStepService.GetAll();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GoodsQuotaStepModel model)
    {
        var result = await _goodsQuotaStepService.Create(model);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] GoodsQuotaStepModel model)
    {
        await _goodsQuotaStepService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _goodsQuotaStepService.Delete(id);
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var res = await _goodsQuotaStepService.GetDetail(id);
        return Ok(res);
    }
}