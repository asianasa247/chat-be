using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ManageEmployee.Services.Interfaces.P_Procedures.Supplies;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Extends;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.SupplyModels.VehicleRepairRequestModels;

namespace ManageEmployee.Controllers.SupplyControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class VehicleRepairRequestsController : ControllerBase
{
    private readonly IVehicleRepairRequestService _vehicleRepairRequestService;

    public VehicleRepairRequestsController(IVehicleRepairRequestService vehicleRepairRequestService)
    {
        _vehicleRepairRequestService = vehicleRepairRequestService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProcedurePagingRequestModel param)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        return Ok(await _vehicleRepairRequestService.GetPaging(param, userId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        var model = await _vehicleRepairRequestService.GetDetail(id, userId);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] VehicleRepairRequestModel model)
    {
        await _vehicleRepairRequestService.Create(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] VehicleRepairRequestModel model)
    {
        await _vehicleRepairRequestService.Update(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept(int id)
    {
        await _vehicleRepairRequestService.Accept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        await _vehicleRepairRequestService.NotAccept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _vehicleRepairRequestService.Delete(id);
        return Ok();
    }

    [HttpGet("get-procedure-number")]
    public async Task<IActionResult> GetProcedureNumber()
    {
        var result = await _vehicleRepairRequestService.GetProcedureNumber();
        return Ok(new BaseResponseCommonModel
        {
            Data = result,
        });
    }

    [HttpGet("get-good-type")]
    public ActionResult GetGoodType()
    {
        var result = Enum.GetValues(typeof(GoodTypeEnum))
                    .Cast<GoodTypeEnum>()
                    .Select(v => new
                    {
                        id = v,
                       value = v.ToString()
                    })
                    .ToList();
        return Ok(new BaseResponseCommonModel
        {
            Data = result,
        });
    }

    //[HttpPost("uploadfile")]
    //public IActionResult Uploadfile([FromForm] IFormFile file)
    //{
    //    var response = _fileService.UploadFile(file, "VehicleRepairRequest", file.FileName);
    //    return Ok(response);
    //}

    [HttpGet("list")]
    public async Task<IActionResult> GetList()
    {
        var response = await _vehicleRepairRequestService.GetList();
        return Ok(response);
    }

    [HttpPost("export/{id}")]
    public async Task<IActionResult> Export(int id)
    {
        var response = await _vehicleRepairRequestService.Export(id);
        return Ok(new BaseResponseCommonModel
        {
            Data = response,
        });
    }
}
