using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.Web;

[Route("api/[controller]")]
[ApiController]
public class WebCareerController : ControllerBase
{
    private IWebCareerService _webCareerService;

    public WebCareerController(
        IWebCareerService webCareerService)
    {
        _webCareerService = webCareerService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchCareer([FromQuery] CareerPagingRequestModel searchRequest)
    {
        var result = await _webCareerService.SearchCareer(searchRequest);
        return Ok(result);
    }

    [HttpGet("list")]
    public IActionResult GetAll()
    {
        var results = _webCareerService.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = results,
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var career = _webCareerService.GetById(id);
        return Ok(career);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CareerViewModel model)
    {
        try
        {
            var result = _webCareerService.Create(model);
            if (result != null)
                return Ok(new ObjectReturn
                {
                    data = model,
                    status = 200
                });
            return BadRequest(new { msg = "Thêm mới không thành công" });
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update([FromBody] CareerViewModel model)
    {
        try
        {
            var result = _webCareerService.Update(model);
            if (result != null)
                return Ok(new ObjectReturn
                {
                    data = model,
                    status = 200
                });
            return BadRequest(new { msg = "Cập nhật không thành công" });
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _webCareerService.Delete(id);
        return Ok(new ObjectReturn
        {
            data = id,
            status = 200
        });
    }
}