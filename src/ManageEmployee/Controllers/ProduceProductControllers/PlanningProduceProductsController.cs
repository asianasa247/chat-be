using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.ProduceProducts.PlanningProduceProducts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.ProduceProductControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PlanningProduceProductsController : ControllerBase
{
    private readonly IPlanningProduceProductService _planningProduceProductService;
    private readonly IPlanningProduceProductExportService _planningProduceProductExportService;
    private readonly IFileService _fileService;
    private readonly IPlanningWithLedgerService _planningWithLedgerService;
    public PlanningProduceProductsController(IPlanningProduceProductService planningProduceProductService,
        IPlanningProduceProductExportService planningProduceProductExportService,
        IFileService fileService,
        IPlanningWithLedgerService planningWithLedgerService)
    {
        _planningProduceProductService = planningProduceProductService;
        _planningProduceProductExportService = planningProduceProductExportService;
        _fileService = fileService;
        _planningWithLedgerService = planningWithLedgerService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProcedurePagingRequestModel param)
    {
        return Ok(await _planningProduceProductService.GetPaging(param, HttpContext.GetIdentityUser().Id));
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll(ProcedureForCreatePlanningProduct? procedureCode)
    {
        return Ok(await _planningProduceProductService.GetList(procedureCode));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _planningProduceProductService.GetDetail(id, HttpContext.GetIdentityUser().Id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PlanningProduceProductModel model)
    {
        await _planningProduceProductService.Create(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] PlanningProduceProductModel model)
    {
        await _planningProduceProductService.Update(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("planning/{id}")]
    public async Task<IActionResult> UpdatePlanning([FromBody] PlanningProduceProductGetDetailModel model)
    {
        await _planningProduceProductService.UpdatePlanning(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept([FromHeader] int yearFilter, int id)
    {
        await _planningProduceProductService.Accept(id, HttpContext.GetIdentityUser().Id, yearFilter);
        return Ok();
    }

    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        await _planningProduceProductService.NotAccept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("canceled/{id}")]
    public async Task<IActionResult> Canceled(int id)
    {
        await _planningProduceProductService.Canceled(id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _planningProduceProductService.Delete(id);
        return Ok();
    }

    [HttpPost("export/{id}")]
    public async Task<IActionResult> Export(int id)
    {
        var response = await _planningProduceProductExportService.ExportFull(id);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }

    [HttpPost("export-for-car/{id}")]
    public async Task<IActionResult> ExportForCar(int? carId, string carName, int id)
    {
        var response = await _planningProduceProductExportService.ExportForCar(carId, carName, id);
        return Ok(response);
    }

    [HttpPost("export-gate-pass/{id}")]
    public async Task<IActionResult> ExportGatePass(int? carId, string carName, int id)
    {
        var response = await _planningProduceProductExportService.ExportGatePass(carId, carName, id);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }

    [HttpPost("export-payment-proposal/{id}")]
    public async Task<IActionResult> ExportPaymentProposal(int? carId, string carName, int id)
    {
        var response = await _planningProduceProductExportService.ExportPaymentProposal(carId, carName, id);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }

    [HttpPost("car-delivery/{id}")]
    public async Task<IActionResult> SetCarDelivery(CarDeliveryModel carDelivery, int id)
    {
        await _planningProduceProductService.SetCarDelivery(carDelivery, id);
        return Ok();
    }

    [HttpGet("car-delivery/{id}")]
    public async Task<IActionResult> GetCarDelivery(int? carId, string carName, int id)
    {
        var res = await _planningProduceProductService.GetCarDelivery(carId, carName, id);
        return Ok(res);
    }

    [HttpGet("list-car")]
    public async Task<IActionResult> GetListCar(int id)
    {
        var res = await _planningProduceProductService.GetListCar(id);
        return Ok(res);
    }

    [HttpPost("payment-proposal/{id}")]
    public async Task<IActionResult> SetPaymentProposal(PaymentProposalModel model, int? carId, string carName, int id)
    {
        await _planningProduceProductService.SetPaymentProposal(model, carId, carName, id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpGet("payment-proposal/{id}")]
    public async Task<IActionResult> GetPaymentProposal(int? carId, string carName, int id)
    {
        var res = await _planningProduceProductService.GetPaymentProposal(carId, carName, id);
        return Ok(res);
    }

    [HttpPost("uploadfile")]
    public IActionResult Uploadfile([FromForm] IFormFile file)
    {
        var response = _fileService.UploadFile(file, "Planning", file.FileName);
        return Ok(response);
    }

    [HttpPut("{id}/cancel-detail")]
    public async Task<IActionResult> CancelPlanningDetail(int id, [FromBody] List<int> detailIds)
    {
        await _planningProduceProductService.CancelPlanningDetail(id, detailIds);
        return Ok();
    }

    [HttpPost("{id}/ledger")]
    public async Task<IActionResult> AddLedger([FromHeader] int yearFilter, int id, int carId, string carName)
    {
        await _planningProduceProductService.AddLedger(id, carId, carName, yearFilter);
        return Ok();
    }

    [HttpPut("{id}/should-export")]
    public async Task<IActionResult> UpDateShouldExportDetail(int id, int carId, string carName)
    {
        await _planningProduceProductService.UpDateShouldExportDetail(id, carId, carName, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPost("ledger-import")]
    [HttpPut("{id}/ledger-import")]

    public async Task<IActionResult> CreateFromLedger([FromBody] PlanningProduceProductModel form)
    {
        await _planningWithLedgerService.SetDataAsync(form, HttpContext.GetIdentityUser().Id);
        return Ok();
    }
}