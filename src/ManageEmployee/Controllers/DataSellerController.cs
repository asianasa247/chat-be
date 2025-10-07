using ManageEmployee.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ManageEmployee.Filters;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.CompanyModels;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DataSellerController : ControllerBase
{
    [HttpGet("get-company-data")]
    public IActionResult GetCompanyData()
    {
        return Ok( new ObjectReturn()
        {
            data = DataSellerHelper.GetData()
        });
    }

    [HttpPost("verify/{companyDbName}")]
    [TypeFilter(typeof(ResponseWrapperFilterAttribute))]
    public IActionResult VerifyCompany(string? companyDbName)
    {
        #if DEBUG
        return Ok(true);
        #endif
        var valid = DataSellerHelper.GetData()?.Companies.Any(x => string.Equals(x.Name, companyDbName.Trim(), StringComparison.OrdinalIgnoreCase));
        return Ok(valid);
    }
    
    [HttpPost]
    public IActionResult Create(CompanyDataModel param)
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "DataSeller\\CompanyData.json");
        DataSellerModel item;
        bool isHasData = false;
        using (StreamReader r = new(path))
        {
            string json = r.ReadToEnd();
            item = JsonSerializer.Deserialize<DataSellerModel>(json);
            
            foreach(var company in item.Companies)
            {
                if (!company.IsDisplay)
                {
                    company.Id = param.Id;
                    company.Name = param.Name;
                    company.IsDisplay = true;
                    isHasData = true;
                    break;
                }   
            }
        }

        using (StreamWriter sw = new(path))
        {
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(item, Newtonsoft.Json.Formatting.Indented);
            sw.Write(output);
        }
        if (isHasData)
            return Ok();

        return BadRequest();
    }
}
