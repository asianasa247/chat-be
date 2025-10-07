using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Inventorys;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class InvoiceDeclarationController : ControllerBase
{
    private readonly IInvoiceDeclarationService _invoiceDeclarationService;
    private readonly IMapper _mapper;

    public InvoiceDeclarationController(
        IInvoiceDeclarationService InvoiceDeclarationService,
        IMapper mapper) 
    {
        _invoiceDeclarationService = InvoiceDeclarationService;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var result = await _invoiceDeclarationService.GetAll(param.Page, param.PageSize, param.SearchText);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetInvoiceDeclaration()
    {
        var results = await _invoiceDeclarationService.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = results,
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var ward = _invoiceDeclarationService.GetById(id);
        var model = _mapper.Map<InvoiceDeclarationModel>(ward);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] InvoiceDeclarationModel model)
    {
        try
        {
            
            var result = await _invoiceDeclarationService.Create(model);
            if (string.IsNullOrEmpty(result))
                return Ok(new ObjectReturn
                {
                    data = model,
                    status = 200
                });
            return BadRequest(new { msg = result });
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] InvoiceDeclarationModel model)
    {
        try
        {
            
            var result = await _invoiceDeclarationService.Update(model);

            return Ok(new ObjectReturn
            {
                data = result,
                status = 200
            });
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _invoiceDeclarationService.Delete(id);
        return Ok(new ObjectReturn
        {
            data = id,
            status = 200
        });
    }
    [HttpPut("invoice/{id}")]
    public async Task<IActionResult> UpdateInvoice([FromHeader] int yearFilter, int id)
    {
        try
        {

            var result = await _invoiceDeclarationService.UpdateInvoice(id, yearFilter);
            return Ok(new ObjectReturn
            {
                data = result,
                status = 200
            });
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("reset-invoice/{id}")]
    public async Task<IActionResult> ResetInvoice(int id)
    {
        try
        {
           var result =  await _invoiceDeclarationService.ResetInvoice(id);
            return Ok(new ObjectReturn
            {
                data = result,
            });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }
}
