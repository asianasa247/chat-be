using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.CustomerModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CustomerTaxInformationController : ControllerBase
{
    private readonly ICustomerTaxInformationService _customerService;

    public CustomerTaxInformationController(
       ICustomerTaxInformationService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] PagingRequestModel param)
    {
        var result = _customerService.GetAll(param.Page, param.PageSize, param.SearchText);
        return Ok(result);
    }

    [HttpGet("list")]
    public IActionResult GetSelectList()
    {
        var customers = _customerService.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = customers,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await _customerService.GetById(id);
        return Ok(response);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<IActionResult> GetByCustomerId(int customerId)
    {
        var response = await _customerService.GetByCustomerId(customerId);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomerTaxInformationModel model)
    {
        // map model to entity and set id
        try
        {
            var result = await _customerService.Create(model);
            if (result != null)
                return Ok(result);
            return BadRequest(new { msg = result });
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{customerId}")]
    public async Task<IActionResult> Update(int customerId, [FromBody] CustomerTaxInformationModel model)
    {
        // map model to entity and set id
        try
        {
            var result = await _customerService.Update(model, customerId);

            if (result != null)
                return Ok(result);
            return BadRequest(new { msg = result });
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }
}