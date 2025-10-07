using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.P_Procedures.Supplies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.SupplyControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class PaymentProposalsController : ControllerBase
{
    private readonly IPaymentProposalService _paymentProposalService;
    private readonly IFileService _fileService;

    public PaymentProposalsController(IPaymentProposalService paymentProposalService, IFileService fileService)
    {
        _paymentProposalService = paymentProposalService;
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProcedurePagingRequestModel param)
    {
        var userId = HttpContext.GetIdentityUser().Id;

        return Ok(await _paymentProposalService.GetPaging(param, userId));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _paymentProposalService.GetDetail(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PaymentProposalModel model)
    {
        await _paymentProposalService.Create(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] PaymentProposalModel model)
    {
        await _paymentProposalService.Update(model, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept(int id)
    {
        await _paymentProposalService.Accept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        await _paymentProposalService.NotAccept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _paymentProposalService.Delete(id);
        return Ok();
    }

    [HttpGet("get-procedure-number")]
    public async Task<IActionResult> GetProcedureNumber()
    {
        var result = await _paymentProposalService.GetProcedureNumber();
        return Ok(new BaseResponseCommonModel
        {
            Data = result,
        });
    }

    [HttpGet("check-permission-button/{id}")]
    public async Task<IActionResult> CheckButton(int id)
    {
        var result = await _paymentProposalService.CheckButton(id, HttpContext.GetIdentityUser().Id);
        return Ok(result);
    }

    [HttpPost("export/{id}")]
    public async Task<IActionResult> Export(int id)
    {
        var response = await _paymentProposalService.Export(id);
        return Ok(new BaseResponseCommonModel
        {
            Data = response,
        });
    }

    [HttpPost("uploadfile")]
    public IActionResult Uploadfile([FromForm] IFormFile file)
    {
        var response = _fileService.UploadFile(file, "PaymentProposal", file.FileName);
        return Ok(response);
    }
}