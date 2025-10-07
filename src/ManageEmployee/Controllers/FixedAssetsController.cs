using System.Security.Claims;
using Common.Helpers;
using ManageEmployee.DataTransferObject;
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
public class FixedAssetsController : ControllerBase
{
    private readonly IFixedAssetsService _service;
    public FixedAssetsController(IFixedAssetsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] FixedAssetsRequestModel request, [FromHeader] int yearFilter)
    {
        if (HttpContext.User.Identity is ClaimsIdentity)
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

        return BadRequest(ErrorMessages.AccessDenined);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
            var data = await _service.GetById(id);

            return Ok(data);
    }
    [HttpDelete("{id}")]
    public IActionResult Delete(List<int> id)
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
    
    [HttpPut("add-fixed-sset242-from-fixed-asset")]
    public async Task<IActionResult> AddFixedAsset242FromFixedAsset(List<FixedAssetsModelEdit> requests, [FromHeader] int yearFilter)
    {
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
           await _service.AddFixedAsset242FromFixedAsset(requests, yearFilter);
            return Ok(new ObjectReturn
            {
                data = null
            }); ;
        }

        return BadRequest(new { msg = ErrorMessages.AccessDenined });
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchFixedAsset(PagingRequestModel searchRequest)
    {
        var result = await _service.SearchFixedAsset(searchRequest);
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
        if (model.UsedDateUnix > 0)
            model.UsedDate = DateHelpers.UnixTimeStampToDateTime(model.UsedDateUnix ?? 0);
        // map model to entity and set id
        await _service.UpdateEdit(model, yearFilter);
        return Ok();

    }

    [HttpPost("export-excel")]
    public IActionResult ExportExcel(PagingRequestModel searchRequest, [FromHeader] int yearFilter)
    {
        var result = _service.ExportExcel(searchRequest);
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
    [HttpPut("update-account")]
    public async Task<IActionResult> UpdateAccount([FromBody] List<FixedAssetsModelEdit> requests, [FromHeader] int yearFilter, [FromQuery] bool IsAutoAddDetail = false)
    {
        var rs = await  _service.UpdateEditAccount(requests, IsAutoAddDetail, yearFilter);

        if (rs.IsSuccess)
        {
            return Ok(rs);
        }
        else
        {
            return BadRequest(new { msg = rs.ErrorMessage });
        }
    }
}