using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ManageEmployee.Services.Interfaces.P_Procedures.Supplies;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Extends;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers.SupplyControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RequestEquipmentsController : ControllerBase
{
    private readonly IRequestEquipmentService _requestEquipmentService;
    private readonly IFileService _fileService;

    public RequestEquipmentsController(IRequestEquipmentService requestEquipmentService, IFileService fileService)
    {
        _requestEquipmentService = requestEquipmentService;
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProcedurePagingRequestModel param)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        return Ok(await _requestEquipmentService.GetPaging(param, userId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        var model = await _requestEquipmentService.GetDetail(id, userId);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RequestEquipmentModel model)
    {
        await _requestEquipmentService.Create(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] RequestEquipmentModel model)
    {
        await _requestEquipmentService.Update(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept(int id)
    {
        await _requestEquipmentService.Accept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        await _requestEquipmentService.NotAccept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _requestEquipmentService.Delete(id);
        return Ok();
    }

    [HttpGet("get-procedure-number")]
    public async Task<IActionResult> GetProcedureNumber()
    {
        var result = await _requestEquipmentService.GetProcedureNumber();
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

    [HttpPost("uploadfile")]
    public IActionResult Uploadfile([FromForm] IFormFile file)
    {
        var response = _fileService.UploadFile(file, "RequestEquipmentOrder", file.FileName);
        return Ok(response);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList()
    {
        var response = await _requestEquipmentService.GetList();
        return Ok(response);
    }

    [HttpPost("export/{id}")]
    public async Task<IActionResult> Export(int id)
    {
        var response = await _requestEquipmentService.Export(id);
        return Ok(new BaseResponseCommonModel
        {
            Data = response,
        });
    }
}
