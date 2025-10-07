using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.CustomerModels;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Customers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ICustomerReporter _customerReporter;
    private readonly ICustomerQuoteService _customerQuoteService;

    public CustomersController(
        ICustomerService customerService,
        ICustomerReporter customerReporter, ICustomerQuoteService customerQuoteService)
    {
        _customerService = customerService;
        _customerReporter = customerReporter;
        _customerQuoteService = customerQuoteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromHeader] int yearFilter, [FromQuery] CustomersSearchViewModel param)
    {
        var userId = HttpContext.GetIdentityUser().Id;
        param.ExportExcel = 0;
        var result = await _customerService.GetPaging(param, userId, yearFilter);
        return Ok(result);
    }

    [HttpPost("gettotaljob")]
    public async Task<IActionResult> GetTotalJobByUserId([FromBody] CustomersSearchViewModel param)
    {
        var identityUser = HttpContext.GetIdentityUser();
        var result = await _customerService.GetTotalJobByUserId(param.Page, param.PageSize, param.SearchText, param.JobId ?? 0, param.StatusId ?? 0, identityUser.Id);
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpPost("gettotalstatus")]
    public async Task<IActionResult> GetTotalStatusByUserId([FromBody] CustomersSearchViewModel param)
    {
        var identityUser = HttpContext.GetIdentityUser();
        var result = await _customerService.GetTotalStatusByUserId(param.Page, param.PageSize, param.SearchText, param.JobId ?? 0, param.StatusId ?? 0, identityUser.Id);
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetSelectList([FromQuery] List<long> customerId, int type = 0, string? searchText = null)
    {
        var customers = await _customerService.GetListCustomer(type, customerId, searchText);

        return Ok(new ObjectReturn
        {
            data = customers,
            status = 200,
        });
    }

    [HttpGet("list-code-name")]
    public async Task<IActionResult> GetListCustomerWithCodeName()
    {
        var customers = await _customerService.GetListCustomerWithCodeName();

        return Ok(new ObjectReturn
        {
            data = customers,
            status = 200,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromHeader] int yearFilter, int id)
    {
        var model = await _customerService.GetById(id, yearFilter);
        return Ok(new ObjectReturn
        {
            data = model,
            status = 200,
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Customer customer)
    {
        var identityUser = HttpContext.GetIdentityUser();
        customer.UserCreated = identityUser.Id;
        customer.UserUpdated = identityUser.Id;
        await _customerService.ValidateCustomer(customer);
        var result = await _customerService.Create(customer);
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Customer customer)
    {
        var identityUser = HttpContext.GetIdentityUser();
        customer.UserUpdated = identityUser.Id;
        await _customerService.ValidateCustomer(customer);
        var result = await _customerService.Update(customer, identityUser.Role);

        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _customerService.Delete(id);
        return Ok(new ObjectReturn
        {
            status = 200,
        });
    }

    [HttpPost("CreateCustomerQuote/{customerId}")]
    public async Task<IActionResult> CreateCustomerQuote([FromHeader] int yearFilter, int customerId, [FromBody] List<BillDetailModel> model)
    {
        var result = await _customerQuoteService.ExportCustomerQuote(model, customerId, HttpContext.GetIdentityUser().Id, yearFilter);
        return Ok(result);
    }

    [HttpPost("GetListCustomerQuoteHistory")]
    public async Task<IActionResult> SearchOrder(CustomerQuoteSearchModel search)
    {
        var result = await _customerQuoteService.GetListCustomerQuoteHistory(search);
        return Ok(result);
    }

    [HttpGet("GetListCustomerQuoteDetail")]
    public async Task<IActionResult> GetListCustomerQuoteDetail(long CustomerQuoteId)
    {
        var result = await _customerQuoteService.GetListCustomerQuoteDetail(CustomerQuoteId);
        return Ok(result);
    }

    [HttpGet]
    [Route("ReportCustomerQuoteDetail")]
    public async Task<IActionResult> ReportCustomerQuoteDetail(long CustomerQuoteId, string type, int CustomerId)
    {
        var data = await _customerQuoteService.ConvertToHTML_BaoGia(CustomerQuoteId, type, CustomerId);
        return Ok(new BaseResponseModel
        {
            Data = data
        });
    }

    [HttpGet("get-code-customer")]
    public async Task<IActionResult> GetCodeCustomer(int type)
    {
        string result = await _customerService.GetCodeCustomer(type);
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }

    [HttpGet("get-customer-warning")]
    public async Task<IActionResult> CustomerWarning()
    {
        var result = await _customerService.CustomerWarning();
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }

    [HttpGet("get-customer-debit")]
    public async Task<IActionResult> GetCustomerCnById([FromHeader] int yearFilter, int id)
    {
        var customers = await _customerService.CustomerCnById(id, yearFilter);
        return Ok(new ObjectReturn
        {
            data = customers,
            status = 200,
        });
    }

    [HttpGet("export-excel")]
    public async Task<IActionResult> ExportExcel([FromQuery] CustomersSearchViewModel param)
    {
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            var userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            param.ExportExcel = 1;
            var result = await _customerReporter.ExportExcel(param, userId);
            return Ok(new ObjectReturn
            {
                data = result,
            });
        }
        return BadRequest(new { msg = "Không tìm thấy  dữ liệu" });
    }

    [HttpPost("import-excel/{type}")]
    public async Task<IActionResult> ImportExcel(int type, [FromBody] List<CustomerImport> datas)
    {
        var identityUser = HttpContext.GetIdentityUser();
        var result = await _customerReporter.ImportExcel(datas, identityUser.Id, type, identityUser.Role);
        if (string.IsNullOrEmpty(result))
            return Ok(new ObjectReturn
            {
                data = result,
            });
        else
            return BadRequest(new { msg = result });
    }

    [HttpPut("update-user-create")]
    public async Task<IActionResult> UpdateUserCreate([FromBody] ChangeUserCreateRequest request)
    {
        await _customerService.UpdateUserCreate(request);
        return Ok(new ObjectReturn
        {
            data = null,
        });
    }

    [HttpGet("chart-birthday-customer")]
    public async Task<IActionResult> ChartBirthdayForCustomer(int type)
    {
        var identityUser = HttpContext.GetIdentityUser();

        var result = await _customerReporter.ChartBirthdayForCustomer(identityUser.Id, type);

        return Ok(new ObjectReturn
        {
            data = result,
        });
    }

    [HttpGet]
    [Route("GetDataBaoGia")]
    public async Task<IActionResult> GetDataBaoGia([FromHeader] int yearFilter, long customerQuoteId, int customerId)
    {
        var (bill, goods) = await _customerQuoteService.GetDataBaoGia(customerQuoteId);
        var customer = await _customerService.GetById(customerId, yearFilter);
        return Ok(new { bill, customer, goods });
    }

    [HttpPost("accountant-for-customer/{id}")]
    public async Task<IActionResult> AccountanForCustomer(int id, [FromBody] AccountanForCustomerModel form)
    {
        await _customerService.SetAccountanForCustomer(id, form);
        return Ok();
    }

    [HttpPost("sync-account-customer/{type}")]
    public async Task<IActionResult> SyncAccountToCustomer(int type)
    {
        await _customerService.SyncAccountToCustomer(type);
        return Ok();
    }

    [HttpPost("account-customer/{id}")]
    public async Task<IActionResult> CreateAcountFromCash([FromBody] AccountanForCustomerModel form, [FromHeader] int yearFilter, int id)
    {
        await _customerService.CreateAcountFromCash(form, id, yearFilter);
        return Ok();
    }
}