using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Services.Interfaces.Webs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SocialController : ControllerBase
{
    private readonly IWebSocialService _webSocialService;

    public SocialController(
        IWebSocialService webSocialService)
    {
        _webSocialService = webSocialService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var result = await _webSocialService.SearchNews(param);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll()
    {
        var results = await _webSocialService.GetAll();

        return Ok(results);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var career = await _webSocialService.GetById(id);
        return Ok(career);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] SocialViewModel model)
    {
        await _webSocialService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromForm] SocialViewModel model)
    {
        await _webSocialService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _webSocialService.Delete(id);
        return Ok();
    }
}