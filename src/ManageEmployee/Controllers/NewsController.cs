using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NewsController : ControllerBase
{
    private readonly IWebNewsService _webNewsService;

    public NewsController(IWebNewsService webNewsService)
    {
        _webNewsService = webNewsService;
    }

    [HttpGet]
    public async Task<IActionResult> SearchNews([FromQuery] WebNewPagingRequestModel searchRequest)
    {
        var result = await _webNewsService.SearchNews(searchRequest);
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
}