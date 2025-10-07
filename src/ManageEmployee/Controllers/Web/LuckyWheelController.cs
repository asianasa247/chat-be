using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Services.Interfaces.Addresses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.Web
{
    [Route("api/[controller]")]
    [ApiController]
    public class LuckyWheelController : ControllerBase
    {

        //private readonly IW _provinceService;
        //private readonly IDistrictService _districtService;
        //private readonly IWardService _wardService;

        //public LuckyWheelController(
        //    IProvinceService provinceService,
        //    IDistrictService districtService,
        //    IWardService wardService)
        //{
        //    _provinceService = provinceService;
        //    _districtService = districtService;
        //    _wardService = wardService;
        //}
        //[HttpGet("getProvince")]
        //public async Task<IActionResult> GetProductForPrize()
        //{
        //    var result = await _provinceService.GetAll();
        //    return Ok(new CommonWebResponse
        //    {
        //        Code = 200,
        //        State = true,
        //        Message = "",
        //        Data = result
        //    });
        //}

    }
}
