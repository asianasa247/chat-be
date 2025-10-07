using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Certificates;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CertificatesController : ControllerBase
{
    private ICertificateService _certificateService;
    private IMapper _mapper;

    public CertificatesController(
        ICertificateService certificateService,
        IMapper mapper)
    {
        _certificateService = certificateService;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] PagingRequestModel param)
    {
        var wards = _certificateService.GetAll(param.Page,param.PageSize,param.SearchText);
        var totalItems = _certificateService.Count(param.SearchText);
        var model = _mapper.Map<IList<CertificateModel>>(wards);
        return Ok(new BaseResponseModel
        {
            TotalItems = totalItems,
            Data = model,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }


    [HttpGet("list")]
    public IActionResult GetSelectList()
    {
        var certificates = _certificateService.GetAll();
     
        var model = _mapper.Map<IList<SelectListModel>>(certificates);
        return Ok(new BaseResponseModel
        {
            Data = model,
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var ward = _certificateService.GetById(id);
        var model = _mapper.Map<CertificateModel>(ward);
        return Ok(model);
    }

    [HttpPut("{id}")]
    [HttpPost]
    public async Task<IActionResult> Save( [FromBody] CertificateModel model, int id = 0)
    {
        // map model to entity and set id
        var certificate = _mapper.Map<Certificate>(model);
        certificate.Id = id;

        try
        {
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                certificate.UserUpdated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
                certificate.UserCreated = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            }
            if (certificate.Id != 0)
            {
                await _certificateService.Update(certificate);
            }
            else
            {
                _certificateService.Create(certificate);
            }
            // update user 

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
        await _certificateService.Delete(id);
        return Ok();
    }
}
