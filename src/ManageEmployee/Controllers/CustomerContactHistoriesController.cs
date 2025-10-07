using AutoMapper;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.CustomerModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CustomerContactHistoriesController : ControllerBase
{
    private readonly ICustomerContactHistoryService _customerService;
    private readonly IMapper _mapper;
    private readonly IFileService _fileService;

    public CustomerContactHistoriesController(
            ICustomerContactHistoryService customerService,
            IMapper mapper,
        IFileService fileService)
    {
        _customerService = customerService;
        _mapper = mapper;
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromBody] PagingRequestModel param)
    {
        var result = await _customerService.GetAllPaging(param.Page, param.PageSize, param.SearchText);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetSelectList()
    {
        var customers = await _customerService.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = customers,
        });
    }

    [HttpGet("get-by-customer")]
    public async Task<IActionResult> GetByCustomerId([FromQuery] CustomersHistoryRequestModel param)
    {
        var result = await _customerService.GetByCustomerId(param);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        var response = await _customerService.GetDetail(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CustomerContactHistoryDto model)
    {
        // map model to entity and set id
        var contact = _mapper.Map<CustomerContactHistoryDetailModel>(model);
        List<string> listFileName = new List<string>();
        // upload file to server
        contact.FileLinkStr = "";
        if (model.FileLink != null && model.FileLink.Count > 0)
        {
            foreach (IFormFile file in model.FileLink)
            {
                var fileName = string.Empty;
                fileName = _fileService.Upload(file, "customerv2/contacthistory");
                listFileName.Add(fileName);
            }
            contact.FileLinkStr = JsonSerializer.Serialize(listFileName);
        }

        var result = await _customerService.Create(contact, HttpContext.GetIdentityUser().Id);

        if (result != null)
            return Ok(result);
        return BadRequest(new { msg = result });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromForm] CustomerContactHistoryDto model)
    {
        // map model to entity and set id
        var contact = _mapper.Map<CustomerContactHistoryDetailModel>(model);
        List<string> listFileName = new List<string>();
        // upload file to server
        contact.FileLinkStr = "";
        if (model.FileLink != null && model.FileLink.Count > 0)
        {
            foreach (IFormFile file in model.FileLink)
            {
                var fileName = string.Empty;
                fileName = _fileService.Upload(file, "customerv2/contacthistory");
                listFileName.Add(fileName);
            }
            contact.FileLinkStr = JsonSerializer.Serialize(listFileName);
        }

        var result = await _customerService.Update(contact, HttpContext.GetIdentityUser().Id);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _customerService.Delete(id);
        return Ok();
    }

    [HttpGet("contacts-for-customer")]
    public async Task<IActionResult> GetListContactForCustomer(int customerId)
    {
        var response = await _customerService.GetListContactForCustomer(customerId);
        return Ok(new BaseResponseModel
        {
            Data = response,
        });
    }

    [HttpPost("contacts-for-customer/{customerId}")]
    public async Task<IActionResult> GetListContactForCustomer([FromBody] SelectListContactForCustomer form, [FromRoute] int customerId)
    {
        await _customerService.AddContactForCustomer(form, customerId);
        return Ok();
    }

    [HttpGet("count-customer-contact")]
    public async Task<IActionResult> CountCustomerContact()
    {
        return Ok(await _customerService.CountCustomerContact());
    }

    [HttpGet("customer-contact-notification")]
    public async Task<IActionResult> GetAllCustomerContact()
    {
        return Ok(await _customerService.GetAllCustomerContact());
    }
}