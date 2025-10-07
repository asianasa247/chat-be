using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Services.Interfaces.Introduces;
using ManageEmployee.Services.Interfaces.Sliders;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.Web;

[ApiController]
public class WebController : ControllerBase
{
    private readonly IWebSocialService _webSocialService;
    private readonly ISliderService _sliderService;
    private readonly IIntroduceTypeService _introduceTypeService;

    public WebController(IWebSocialService webSocialService, 
        ISliderService sliderService, 
        IIntroduceTypeService introduceTypeService)
    {
        _webSocialService = webSocialService;
        _sliderService = sliderService;
        _introduceTypeService = introduceTypeService;
    }

    [HttpGet("api/WebSocial/list")]
    public async Task<IActionResult> GetAll()
    {
        var results = await _webSocialService.GetAll();

        return Ok(results);
    }

    [HttpGet("api/WebSlider/getAll")]
    public async Task<IActionResult> GetCategory()
    {
        var results = await _sliderService.GetAll();

        return Ok(new CommonWebResponse()
        {
            State = true,
            Code = 200,
            Message = "",
            Data = results
        });
    }

    [HttpGet("api/WebIntroduceType/list")]
    public async Task<IActionResult> GetListIntroduce()
    {
        var results = await _introduceTypeService.GetList();

        return Ok(new CommonWebResponse()
        {
            State = true,
            Code = 200,
            Message = "",
            Data = results
        });
    }
}