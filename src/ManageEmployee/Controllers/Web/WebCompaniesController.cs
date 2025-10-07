using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Services.Interfaces.Companies;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.Web;

[Route("api/[controller]")]
[ApiController]
public class WebCompaniesController : ControllerBase
{
    private readonly ICompanyService _companyService;
    public WebCompaniesController(
        ICompanyService companyService)
    {
        _companyService = companyService;
    }
    [HttpGet("getCompany")]
    public async Task<IActionResult> GetCompany()
    {
        var company = await _companyService.GetCompany();
        return Ok(new CommonWebResponse
        {
            Code = 200,
            State = true,
            Message = "",
            Data = company
        });
    }
    //[HttpGet("introduces")]
    //public IActionResult GetListIntroduce()
    //{
    //    var response = Enum.GetValues(typeof(IntroduceType))
    //        .Cast<IntroduceType>()
    //        .Select(v => new { code = v.ToString(), name = v.GetDescription() })
    //        .ToList();
    //    return Ok(new CommonWebResponse
    //    {
    //        Code = 200,
    //        State = true,
    //        Message = "",
    //        Data = response
    //    });
    //}
}
