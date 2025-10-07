using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.ProduceProducts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.ProduceProductControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WarehouseProduceProductsController : ControllerBase
{
    private readonly IWarehouseProduceProductService _warehouseProduceProductService;

    public WarehouseProduceProductsController(IWarehouseProduceProductService warehouseProduceProductService)
    {
        _warehouseProduceProductService = warehouseProduceProductService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProcedurePagingRequestModel param)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        var result = await _warehouseProduceProductService.GetPaging(param, userId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromHeader] int yearFilter, [FromRoute] int id)
    {
        var result = await _warehouseProduceProductService.GetDetail(id, yearFilter);
        return Ok(result);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] WarehouseProduceProductGetDetailModel model)
    {
        var identityUser = HttpContext.GetIdentityUser();

        await _warehouseProduceProductService.Update(model, identityUser.Id);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept(int id)
    {
        var identityUser = HttpContext.GetIdentityUser();
        await _warehouseProduceProductService.Accept(id, identityUser.Id);
        return Ok();
    }

    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        var identityUser = HttpContext.GetIdentityUser();
        await _warehouseProduceProductService.NotAccept(id, identityUser.Id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _warehouseProduceProductService.Delete(id);
        return Ok();
    }


    [HttpGet("payment-proposal/{id}")]
    public async Task<IActionResult> GetPaymentProposal(int? carId, string carName, int id)
    {
        var res = await _warehouseProduceProductService.GetPaymentProposal(carId, carName, id);
        return Ok(res);
    }

    [HttpGet("car-delivery/{id}")]
    public async Task<IActionResult> GetCarDelivery(int? carId, string carName, int id)
    {
        var res = await _warehouseProduceProductService.GetCarDelivery(carId, carName, id);
        return Ok(res);
    }

    [HttpPost("car-delivery/{id}")]
    public async Task<IActionResult> SetCarDelivery(CarDeliveryModel carDelivery, int id)
    {
        await _warehouseProduceProductService.SetCarDelivery(carDelivery, id);
        return Ok();
    }

    [HttpPost("payment-proposal/{id}")]
    public async Task<IActionResult> SetPaymentProposal(PaymentProposalModel model, int? carId, string carName, int id)
    {
        await _warehouseProduceProductService.SetPaymentProposal(model, carId, carName, id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    //[HttpPost("export-gate-pass/{id}")]
    //public async Task<IActionResult> ExportGatePass(int? carId, string carName, int id)
    //{
    //    var response = await _warehouseProduceProductService.ExportGatePass(carId, carName, id);
    //    return Ok(new BaseResponseCommonModel
    //    {
    //        Data = response
    //    });
    //}

    //[HttpPost("export-payment-proposal/{id}")]
    //public async Task<IActionResult> ExportPaymentProposal(int? carId, string carName, int id)
    //{
    //    var response = await _warehouseProduceProductService.ExportPaymentProposal(carId, carName, id);
    //    return Ok(new BaseResponseCommonModel
    //    {
    //        Data = response
    //    });
    //}
}