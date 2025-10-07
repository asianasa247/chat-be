using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Services.Interfaces.Introduces;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IntroduceController : ControllerBase
{
    private readonly IIntroduceService _introduceService;

    public IntroduceController(
        IIntroduceService introduceService)
    {
        _introduceService = introduceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestTypeModel param)
    {
        var result = await _introduceService.GetAll(param);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetIntroduce()
    {
        var results = await _introduceService.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = results,
        });
    }

    [HttpGet("single-page-types")]
    public IActionResult GetPageType()
    {
        var types = new HashSet<SinglePageTypeModel>()
        {
            new() { Id = 1, Name = "Giới thiệu", Url = "/introduce" },
            new() { Id = 2, Name = "Lãnh đạo", Url = "/leader" },
            new() { Id = 3, Name = "Phương thức thanh toán", Url = "/payment-method" },
            new() { Id = 4, Name = "Bảo hành", Url = "/warranty" },
            new() { Id = 5, Name = "Đổi trả", Url = "/good-return" }
        };
        return Ok(types);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _introduceService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] IntroduceModel model)
    {
        await _introduceService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] IntroduceModel model)
    {
        //if (model.IntroduceTypeId == IntroduceType.Leader)
        //{
        //    if (HttpContext.Request.Form.Files != null && HttpContext.Request.Form.Files.Count > 0)
        //    {
        //        var img = _fileService.Upload(HttpContext.Request.Form.Files[0], "Sliders");
        //        if (img?.Length > 0)
        //        {
        //            model.Content = img;
        //        }
        //    }
        //    else
        //    {
        //        var item = await _introduceService.GetById((int)model.Id);
        //        model.Content = item.Content;
        //    }
        //}
        await _introduceService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _introduceService.Delete(id);
        return Ok();
    }
}