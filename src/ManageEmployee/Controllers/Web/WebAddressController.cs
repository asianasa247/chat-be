using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Services.Interfaces.Addresses;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.Web;

[Route("api/[controller]")]
[ApiController]
public class WebAddressController : ControllerBase
{
    private readonly IProvinceService _provinceService;
    private readonly IDistrictService _districtService;
    private readonly IWardService _wardService;

    public WebAddressController(
        IProvinceService provinceService,
        IDistrictService districtService,
        IWardService wardService)
    {
        _provinceService = provinceService;
        _districtService = districtService;
        _wardService = wardService;
    }

    [HttpGet("getProvince")]
    public async Task<IActionResult> GetProvince()
    {
        var result = await _provinceService.GetAll();
        return Ok(new CommonWebResponse
        {
            Code = 200,
            State = true,
            Message = "",
            Data = result
        });
    }

    [HttpGet("getDistrict/{id}")]
    public async Task<IActionResult> GetDistrict(int id)
    {
        var result = await _districtService.GetAll(x => x.ProvinceId == id);
        return Ok(new CommonWebResponse
        {
            Code = 200,
            State = true,
            Message = "",
            Data = result
        });
    }

    [HttpGet("getWard/{id}")]
    public async Task<IActionResult> GetWard(int id)
    {
        var result = await _wardService.GetAll(x => x.DistrictId == id);
        return Ok(new CommonWebResponse
        {
            Code = 200,
            State = true,
            Message = "",
            Data = result
        });
    }
}