using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.ProduceProducts.OrderProduceProducts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.ProduceProductControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrderProduceProductsController : ControllerBase
{
    private readonly IOrderProduceProductService _orderProduceProductService;
    private readonly IOrderProduceProductReporter _orderProduceProductReporter;
    private readonly IOrderProduceProductExcelService _orderProduceProductExcelService;

    public OrderProduceProductsController(IOrderProduceProductService produceProductService,
        IOrderProduceProductReporter orderProduceProductReporter,
        IOrderProduceProductExcelService orderProduceProductExcelService)
    {
        _orderProduceProductService = produceProductService;
        _orderProduceProductReporter = orderProduceProductReporter;
        _orderProduceProductExcelService = orderProduceProductExcelService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] OrderProduceProductPagingRequestModel param)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        var result = await _orderProduceProductService.GetPaging(param, userId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromHeader] int yearFilter, int id)
    {
        var result = await _orderProduceProductService.GetDetail(id, yearFilter);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] OrderProduceProductCreateModel model)
    {
        var identityUser = HttpContext.GetIdentityUser();

        await _orderProduceProductService.Create(model, identityUser.Id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] OrderProduceProductModel model)
    {
        var identityUser = HttpContext.GetIdentityUser();

        await _orderProduceProductService.Update(model, identityUser.Id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _orderProduceProductService.Delete(id);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept([FromHeader] int yearFilter, int id)
    {
        var identityUser = HttpContext.GetIdentityUser();
        await _orderProduceProductService.Accept(id, identityUser.Id, yearFilter);
        return Ok();
    }

    [HttpPost("bill-to-order-produce/{billId}")]
    public async Task<IActionResult> CreateFromBill(int billId)
    {
        var identityUser = HttpContext.GetIdentityUser();
        var response = await _orderProduceProductService.CreateFromBill(billId, identityUser.Id);
        return Ok(response);
    }

    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        var identityUser = HttpContext.GetIdentityUser();
        await _orderProduceProductService.NotAccept(id, identityUser.Id);
        return Ok();
    }

    [HttpGet("get-procedure-number")]
    public async Task<IActionResult> GetProcedureNumber()
    {
        var result = await _orderProduceProductService.GetProcedureNumber();
        return Ok(new BaseResponseCommonModel
        {
            Data = result,
        });
    }

    [HttpPut("canceled/{id}")]
    public async Task<IActionResult> Canceled(int id)
    {
        await _orderProduceProductService.Canceled(id);
        return Ok();
    }

    [HttpGet("export/{id}")]
    public async Task<IActionResult> ExportPdf(int id)
    {
        var response = await _orderProduceProductReporter.ExportPdf(id);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }

    [HttpGet("report")]
    public async Task<IActionResult> Report([FromQuery] OrderProduceProductReportRequestModel param)
    {
        var response = await _orderProduceProductReporter.ReportAsync(param);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }

    [HttpGet("report-export")]
    public async Task<IActionResult> ExportReportAsync([FromQuery] OrderProduceProductReportRequestModel param)
    {
        var response = await _orderProduceProductReporter.ExportReportAsync(param);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }

    [HttpPost("export-excel")]
    public async Task<IActionResult> ExportExcel([FromBody] List<int> ids)
    {
        var response = await _orderProduceProductExcelService.ExportExcel(ids);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }

    [HttpPost("import-excel")]
    public async Task<IActionResult> ImportExcel([FromForm] IFormFile file)
    {
        await _orderProduceProductExcelService.ImportExcel(file, HttpContext.GetIdentityUser().Id);
        return Ok();
    }
}