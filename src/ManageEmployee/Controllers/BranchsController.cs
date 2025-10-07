using AutoMapper;
using ManageEmployee.DataTransferObject.AddressModels;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.AddressEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Addresses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BranchsController : ControllerBase
{
    private readonly IBranchService _branchService;
    private readonly IMapper _mapper;

    public BranchsController(
        IBranchService branchService,
        IMapper mapper)
    {
        _branchService = branchService;
        _mapper = mapper;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        return Ok(await _branchService.GetAll(param.Page, param.PageSize, param.SearchText));
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetBranch()
    {
        var results = await _branchService.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = results,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ward = await _branchService.GetById(id);
        var model = _mapper.Map<BranchModel>(ward);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BranchModel model)
    {
        try
        {
            Branch item = _mapper.Map<Branch>(model);
            var result = await _branchService.Create(item);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return Ok(new { code = 400, msg = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] Branch model)
    {
        try
        {
            Branch item = _mapper.Map<Branch>(model);

            var result = await _branchService.Update(item);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return Ok(new { code = 400, msg = result });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var result = await _branchService.Delete(id);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return BadRequest(new { msg = result });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }
}
