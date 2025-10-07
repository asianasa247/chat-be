using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.OrderEntities;
using ManageEmployee.Services.Interfaces.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PayerController : ControllerBase
{
    private readonly IPayerServices _payerServices;
    public PayerController(IPayerServices payerServices)
    {
        _payerServices = payerServices;
    }
    [HttpGet]
    public async Task<IActionResult> GetPage([FromQuery] PayerPagingationRequestModel param)
    {
        var data = await _payerServices.GetPage(param.Page, param.PageSize, param.SearchText, param.PayerType);
        var totalItems = await _payerServices.CountPageTotal(param.SearchText, param.PayerType);
        return Ok(new BaseResponseModel
        {
            Data = data,
            TotalItems = totalItems,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetSelectList()
    {
        var payers = await _payerServices.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = payers,
        });
    }

    [HttpGet("payer-customer")]
    public async Task<IActionResult> GetSelectList(string? searchText)
    {
        var customers = await _payerServices.GetAll(searchText);

        return Ok(new ObjectReturn
        {
            data = customers,
            status = 200,
        });
    }

    [HttpGet("payer-customer-tax")]
    public async Task<IActionResult> GetListPayerWithCustomerTax([FromQuery] PayerPagingationRequestModel param)
    {
        var payers = await _payerServices.GetListPayerWithCustomerTax(param);

        return Ok(payers);
    }

    [HttpPost("get-tax-codes")]
    public async Task<IActionResult> GetTaxCodes([FromBody] PagingRequestModel param)
    {
        var data = await _payerServices.GetTaxCodes(param.Page, param.PageSize, param.SearchText);
        var totalItems = await _payerServices.CountTaxCodeTotal(param.SearchText);
        return Ok(new BaseResponseModel
        {
            Data = data,
            TotalItems = totalItems,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Payer entity)
    {
        var result = await _payerServices.Create(entity);
        if(string.IsNullOrEmpty(result))
            return Ok(new ObjectReturn
            {
                message= result,
                status = 200
            });
        return Ok(new ObjectReturn
        {
            message = result,
            status = 200
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] Payer entity)
    {
        var result = await _payerServices.Update(entity);
        if (string.IsNullOrEmpty(result))
            return Ok(new ObjectReturn
            {
                message = result,
                status = 200
            });
        return Ok(new ObjectReturn
        {
            message = result,
            status = 200
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _payerServices.Delete(id);
        if (string.IsNullOrEmpty(result))
            return Ok(new ObjectReturn
            {
                message = result,
                status = 200
            });
        return Ok(new ObjectReturn
        {
            message = result,
            status = 200
        });
    }


    [HttpPost("delete-many")]
    public async Task<IActionResult> Delete(List<long> id)
    {
        await _payerServices.Delete(id);
        return Ok();
    }
}