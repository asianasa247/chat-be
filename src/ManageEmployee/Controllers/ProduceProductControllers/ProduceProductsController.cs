using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.ProduceProducts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.ProduceProductControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProduceProductsController : ControllerBase
{
    private readonly IProduceProductService _produceProductService;

    public ProduceProductsController(IProduceProductService produceProductService)
    {
        _produceProductService = produceProductService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaging([FromQuery] ProcedurePagingRequestModel param)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        var result = await _produceProductService.GetPaging(param, userId);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _produceProductService.GetList();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromHeader] int yearFilter, [FromRoute] int id)
    {
        var result = await _produceProductService.GetDetail(id, yearFilter);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] ProduceProductModel model)
    {
        var identityUser = HttpContext.GetIdentityUser();

        await _produceProductService.Update(model, identityUser.Id);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept(int id)
    {
        var identityUser = HttpContext.GetIdentityUser();
        await _produceProductService.Accept(id, identityUser.Id);
        return Ok();
    }

    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        var identityUser = HttpContext.GetIdentityUser();
        await _produceProductService.NotAccept(id, identityUser.Id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _produceProductService.Delete(id);
        return Ok();
    }

    [HttpPost("ledger-import/{id}")]
    public async Task<IActionResult> SetLedgerImportProduct([FromHeader] int yearFilter, int id)
    {
        await _produceProductService.SetLedgerImportProduct(id, yearFilter);
        return Ok();
    }

    [HttpPost("ledger-export/{id}")]
    public async Task<IActionResult> SetLedgerExportProduct([FromHeader] int yearFilter, int id)
    {
        await _produceProductService.SetLedgerExportProduct(id, yearFilter);
        return Ok();
    }
}