using System.Security.Claims;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.FixedAssetsModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FixedAsset242sController : ControllerBase
{
    private readonly IFixedAssets242Service _service;

    public FixedAsset242sController(IFixedAssets242Service service) 
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] FixedAssetsRequestModel request, [FromHeader] int yearFilter)
    {
        var data = await _service.GetListEdit(request.FilterType, request.FilterMonth, yearFilter, request.IsInternal);
        var totalItems = data.Count;

        int nextOrder = 1;
        if (totalItems != 0 && data.Any())
        {
            nextOrder = data.Max(x => x.Id) + 1;
        }

        return Ok(new BaseResponseModel
        {
            Data = data,//data.Skip((request.page - 1) * request._pageSize).Take(request._pageSize),
            TotalItems = data.Count,
            PageSize = request.PageSize,
            CurrentPage = request.Page,
            NextStt = nextOrder
        });

    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
            var data = await _service.GetById(id);

            return Ok(data);
    }
    [HttpDelete]
    public IActionResult Deletes(List<int> id)
    {
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            var message = _service.Deletes(id);
            if (string.IsNullOrEmpty(message))
                return Ok();
            return BadRequest(message);
        }

        return BadRequest(ErrorMessages.AccessDenined);
    }
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            var message = _service.Delete(id);
            if (string.IsNullOrEmpty(message))
                return Ok();
            return BadRequest(message);
        }

        return BadRequest(ErrorMessages.AccessDenined);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromQuery] int isInternal, [FromBody] List<FixedAssetsModelEdit> requests, [FromHeader] int yearFilter)
    {
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            var rs = await _service.UpdateEdit(requests, isInternal, yearFilter);

            if (rs.IsSuccess)
            {
                return Ok(rs);
            }
            else
            {
                return BadRequest(new { msg = rs.ErrorMessage });
            }

        }

        return BadRequest(new { msg = ErrorMessages.AccessDenined });
    }

    [HttpPost("search")]
    public IActionResult SearchFixedAsset(FixedAsset242RequestModel searchRequest)
    {
        var result = _service.SearchFixedAsset(searchRequest);
        return Ok(new BaseResponseModel
        {
            TotalItems = result.TotalItems,
            Data = result.Data,
            PageSize = result.PageSize,
            CurrentPage = result.CurrentPage
        });
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFixedAssets(FixedAssetsModelEdit model, [FromHeader] int yearFilter)
    {
        try
        {
            var result = await _service.Update(model, yearFilter);
            return Ok(result);
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPost("export-excel")]
    public IActionResult ExportExcel(FixedAsset242RequestModel searchRequest, [FromHeader] int yearFilter)
    {
        var result = _service.ExportExcel(searchRequest);
        return Ok(new BaseResponseModel
        {
            Data = result
        });
    }
    [HttpPost("import-excel")]
    public IActionResult ImportExcel([FromBody] List<FixedAssetViewModel> datas, [FromHeader] int yearFilter)
    {
        var result = _service.ImportExcel(datas);
        return Ok(new BaseResponseModel
        {
            Data = result
        });
    }
    [HttpPut("update-account")]
    public async Task<IActionResult> UpdateAccount(List<FixedAssetsModelEdit> requests, [FromHeader] int yearFilter)
    {
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            var rs = await _service.UpdateEditAccount(requests);


            if (rs.IsSuccess)
            {
                return Ok(rs);
            }
            else
            {
                return BadRequest(new { msg = rs.ErrorMessage });
            }

        }

        return BadRequest(new { msg = ErrorMessages.AccessDenined });
    }
}