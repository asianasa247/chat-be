using Common.Helpers;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.BillReportModels.BusinessOverviews;
using ManageEmployee.DataTransferObject.CompanyModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Models;
using ManageEmployee.Services.Interfaces.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;
    private readonly IOtherCompanyInfoGetter _otherCompanyInfoGetter;

    public CompaniesController(
        ICompanyService companyService, IOtherCompanyInfoGetter otherCompanyInfoGetter)
    {
        _companyService = companyService;
        _otherCompanyInfoGetter = otherCompanyInfoGetter;
    }

    [HttpGet("get-company")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCompany()
    {
        var company = await _companyService.GetCompany();
        return Ok(new BaseResponseModel
        {
            Data = company,
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetLogCompany([FromQuery] PagingRequestModel model)
    {
        var companyLogs = await _companyService
            .GetAll(model.Page, model.PageSize);
        return Ok(new BaseResponseModel
        {
            Data = companyLogs,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _companyService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Company model)
    {
        await _companyService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] CompanyViewModel model)
    {
        var companyResult = await _companyService.Update(model);
        return Ok(companyResult);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _companyService.Delete(id);
        return Ok();
    }

    [HttpGet("languages")]
    public IActionResult GetLanguage()
    {
        var roomTypeEnums = Enum.GetValues(typeof(LanguageEnum))
                           .Cast<LanguageEnum>()
                           .Select(v => new
                           {
                               id = v,
                               name = v.GetDescription()
                           })
                           .ToList();
        return Ok(roomTypeEnums);
    }

    [HttpGet("infor-other")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Response<OtherCompanyInfomationModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByInfor(string taxCode)
    {
        var response = await _otherCompanyInfoGetter.GetInforCompany(taxCode);
        return Ok(response);
    }
}