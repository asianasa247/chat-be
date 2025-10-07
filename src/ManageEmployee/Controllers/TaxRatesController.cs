using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ManageEmployee.Services.Interfaces.TaxRates;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TaxRatesController : ControllerBase
{
    private readonly ITaxRateService _taxRateService;

    public TaxRatesController(ITaxRateService taxRateService)
    {
        _taxRateService = taxRateService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPage([FromQuery] PagingRequestModel param)
    {
        var data = await _taxRateService.GetPage(param.Page, param.PageSize);
        var totalItems = await _taxRateService.CountAll();
        return Ok(new BaseResponseModel()
        {
            Data = data,
            TotalItems = totalItems,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById([FromHeader] int yearFilter, long id)
    {
        var data = await _taxRateService.GetById(id, yearFilter);
        return Ok(data);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetSelectList()
    {
        var taxRates = await _taxRateService.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = taxRates,
        });
    }


    [HttpGet("get-by-code")]
    public async Task<IActionResult> GetByCode([FromQuery] string? code = "")
    {
        var data = await _taxRateService.GetTaxTypeByCode(code);
        return Ok(data);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaxRate entity)
    {
        var message = await _taxRateService.Create(entity);
        if (string.IsNullOrEmpty(message))
            return Ok(
            new ObjectReturn
            {
                message = message,
                status = Convert.ToInt32(ErrorEnum.SUCCESS)
            });
        return Ok(new ObjectReturn
        {
            message = message,
            status = Convert.ToInt32(ErrorEnum.TAX_RATE_CODE_IS_EXIST)
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] TaxRate entity)
    {
        var message = await _taxRateService.Update(entity);
        if (string.IsNullOrEmpty(message))
            return Ok(
            new ObjectReturn
            {
                message = message,
                status = Convert.ToInt32(ErrorEnum.SUCCESS)
            });
        return Ok(new ObjectReturn
        {
            message = message,
            status = Convert.ToInt32(ErrorEnum.TAX_RATE_CODE_IS_EXIST)
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var message = await _taxRateService.Delete(id);
        if (string.IsNullOrEmpty(message))
            return Ok();
        return Ok(new ObjectReturn
        {
            message = message,
            status = Convert.ToInt32(ErrorEnum.BAD_REQUEST)
        });
    }

    [HttpGet("list-tax-rate-start-r")]
    public async Task<IActionResult> GetListTaxRateStartWithR()
    {
        var data = await _taxRateService.GetListTaxRateStartWithR();
        return Ok( new ObjectReturn
        {
            data = data,
        }
            );
    }

}