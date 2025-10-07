using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.SignatureBlockModels;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Signatures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SignatureBlocksController : ControllerBase
{
    private readonly ISignatureBlockService _signatureBlockService;

    public SignatureBlocksController(ISignatureBlockService signatureBlockService)
    {
        _signatureBlockService = signatureBlockService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaging([FromQuery] ProcedurePagingRequestModel param)
    {
        var response = await _signatureBlockService.GetPaging(param);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaging(int id)
    {
        var response = await _signatureBlockService.GetById(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> Create(SignatureBlockModel form)
    {
        await _signatureBlockService.Create(form, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(SignatureBlockModel form)
    {
        await _signatureBlockService.Update(form, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("not-accept/{id}")]
    public async Task<IActionResult> NotAccept(int id)
    {
        await _signatureBlockService.NotAccept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpPut("accept/{id}")]
    public async Task<IActionResult> Accept(int id)
    {
        await _signatureBlockService.Accept(id, HttpContext.GetIdentityUser().Id);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete(int id)
    {
        await _signatureBlockService.Delete(id);
        return Ok();
    }

    [HttpGet("export/{id}")]
    public async Task<IActionResult> Export(int id)
    {
        var response = await _signatureBlockService.Export(id);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }
}