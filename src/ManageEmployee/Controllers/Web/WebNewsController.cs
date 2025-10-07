using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.Web;

[Route("api/[controller]")]
[ApiController]
public class WebNewsController : ControllerBase
{
    private readonly IWebNewsService _webNewsService;

    public WebNewsController(
        IWebNewsService webNewsService)
    {
        _webNewsService = webNewsService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchNews([FromQuery] WebNewPagingRequestModel searchRequest)
    {
        var result = await _webNewsService.SearchNews(searchRequest);
        return Ok(result);
    }

    [HttpGet("getnewheadline")]
    public async Task<IActionResult> GetNewHeadLine([FromQuery] WebNewHeadLinePagingRequestModel searchRequest)
    {
        var result = await _webNewsService.GetNewHeadLine(searchRequest);
        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromForm] NewsViewSetupModel model)
    {
        await _webNewsService.CreateOrUpdate(model);
        return Ok(new ObjectReturn
        {
            data = model,
            status = 200
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromForm] NewsViewSetupModel model)
    {

        await _webNewsService.CreateOrUpdate(model);
        return Ok(new ObjectReturn
        {
            data = model,
            status = 200
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _webNewsService.Delete(id);
        return Ok(new ObjectReturn
        {
            data = id,
            status = 200
        });
    }
    [HttpGet("list")]
    public async Task<IActionResult> GetAll()
    {
        var results = await _webNewsService.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = results,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var career = await _webNewsService.GetById(id);
        return Ok(career);
    }
}