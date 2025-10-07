using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.FinalStandards;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class FinalStandardController : ControllerBase
{
    private readonly IFinalStandardService _finalStandardService;
    private readonly IMapper _mapper;

    public FinalStandardController(
        IFinalStandardService finalStandardService,
        IMapper mapper)
    {
        _finalStandardService = finalStandardService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var response = await _finalStandardService.GetPaging(param);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int Id)
    {
        var document = _finalStandardService.GetById(Id);

        return Ok(new { data = document });
    }

    [HttpPost]
    public IActionResult Create([FromBody] FinalStandard model)
    {
        var currenDocument = _mapper.Map<FinalStandard>(model);
        currenDocument.Id = model.Id;

        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                currenDocument.UserUpdated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
                currenDocument.UserCreated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            }
            if (_finalStandardService.GetAll().Any(x => x.CreditCode == currenDocument.CreditCode
            && x.DebitCode == currenDocument.DebitCode && x.Type == currenDocument.Type))
            {
                throw new ErrorException(ResultErrorConstants.CODE_EXIST);
            }
            _finalStandardService.Create(currenDocument);

            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update([FromBody] FinalStandard model)
    {
        var currenDocument = _mapper.Map<FinalStandard>(model);
        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                currenDocument.UserUpdated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
                currenDocument.UserCreated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            }
            if (_finalStandardService.GetAll().Where(x => x.Id != currenDocument.Id && x.CreditCode == currenDocument.CreditCode
            && x.DebitCode == currenDocument.DebitCode && x.Type == currenDocument.Type).Any())
            {
                throw new ErrorException(ResultErrorConstants.CODE_EXIST);
            }
            _finalStandardService.Update(currenDocument);
            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _finalStandardService.Delete(id);
        return Ok();
    }

    [HttpGet("GetDetails")]
    public async Task<IActionResult> GetDetails([FromHeader] int yearFilter, [FromQuery] PagingationFinalStandardRequestModel param)
    {
        var data = await _finalStandardService.GetFinalStandardDetail(param, yearFilter);
        return Ok(new BaseResponseModel
        {
            TotalItems = data.Count,
            Data = data.Skip(param.PageSize * param.Page).Take(param.PageSize),
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpPost("ledger")]
    public async Task<IActionResult> SetIntoLedger([FromHeader] int yearFilter, List<FinalStandardDetailModel> finalStandards, int isInternal)
    {
        await _finalStandardService.SetIntoLedger(finalStandards, isInternal, yearFilter);
        return Ok();
    }
}