using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ManageEmployee.Services.Interfaces.Bills;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TillManagersController : ControllerBase
{
    private readonly ITillManagerService _tillManagerService;
    private readonly IMapper _mapper;
    public TillManagersController(ITillManagerService tillManagerService, IMapper mapper)
    {
        _tillManagerService = tillManagerService;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        return Ok(await _tillManagerService.GetAll(param.Page, param.PageSize, param.SearchText));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _tillManagerService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TillManagerModel model)
    {
        TillManager tillManager = _mapper.Map<TillManager>(model);
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            tillManager.UserId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }
        tillManager.FromAt = DateTime.Now;
        tillManager.IsFinish = false;

        var result = await _tillManagerService.Create(tillManager);

        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] TillManager model)
    {
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            model.UserId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            await _tillManagerService.Update(model);
        }

        return Ok(new { code = 200, msg = "" });
    }

    [HttpGet("get-total-amount")]
    public async Task<IActionResult> CaculateAmountInTill()
    {

        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            var userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            var result = await _tillManagerService.CaculateAmountInTill(userId);

            return Ok(new ObjectReturn
            {
                data = result,
                status = 200,
            });
        }

        return Ok(new { code = 400, msg = "" });
    }

    [HttpGet("get-current-till")]
    public async Task<IActionResult> GetCurrentTillManager()
    {

        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            var userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            var result = await _tillManagerService.GetCurrentTillManager(userId);

            return Ok(new ObjectReturn
            {
                data = result,
                status = 200,
            });
        }

        return Ok(new { code = 400, msg = "" });
    }
}
