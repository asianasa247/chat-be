
using System.Security.Claims;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.FixedAssetsModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Assets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FixedAssetUsersController : ControllerBase
{
    private readonly IFixedAssetsUserService _service;

    public FixedAssetUsersController(IFixedAssetsUserService service) 
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetPagingData([FromQuery] PagingRequestModel request)
    {
        if (HttpContext.User.Identity is ClaimsIdentity)
        {
            var data = _service.GetListEdit(request);
            var totalItems = await data.CountAsync();

            int nextOrder = 1;
            if (totalItems != 0 && data.Any())
            {
                nextOrder = data.Max(x => x.Id) + 1;
            }

            return Ok(new BaseResponseModel
            {
                Data = await data.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync(),
                TotalItems = totalItems,
                PageSize = request.PageSize,
                CurrentPage = request.Page,
                NextStt = nextOrder
            });
        }

        return BadRequest(ErrorMessages.AccessDenined);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
            var data = await _service.GetById(id);

            return Ok(data);
    }

    [HttpGet("v2/{id}")]
    public async Task<IActionResult> GetByIdV2([FromHeader] int yearFilter, int id)
    {
        var data = await _service.GetByIdV2(id, yearFilter);
        return Ok(data);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (HttpContext.User.Identity is ClaimsIdentity)
        {
            var message = _service.Delete(id);
            if (string.IsNullOrEmpty(message))
                return Ok();
            return BadRequest(message);
        }

        return BadRequest(ErrorMessages.AccessDenined);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFixedAssets(FixedAssetUser model)
    {
        try
        {
            var result = await _service.UpdateEdit(model);
            return Ok(result);
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPost("export-excel")]
    public async Task<IActionResult> ExportExcel([FromBody] PagingRequestModel searchRequest)
    {
        var result = await _service.ExportExcel(searchRequest);
        return Ok(new BaseResponseModel
        {
            Data = result
        });
    }
    [HttpPost("import-excel")]
    public IActionResult ImportExcel([FromBody] List<FixedAssetViewModel> datas)
    {
        var result = _service.ImportExcel(datas);
        return Ok(new BaseResponseModel
        {
            Data = result
        });
    }
    [HttpPost]
    public async Task<IActionResult> Create(FixedAssetUser model, [FromHeader] int yearFilter)
    {
        try
        {
            var result = await _service.Create(model, yearFilter);
            return Ok(result);
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }
}