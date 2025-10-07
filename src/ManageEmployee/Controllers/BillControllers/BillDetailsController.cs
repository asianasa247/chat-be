using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.Services.Interfaces.Bills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.BillControllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BillDetailsController : ControllerBase
{
    private readonly IBillDetailService _billDetailService;

    public BillDetailsController(
        IBillDetailService billDetailService)
    {
        _billDetailService = billDetailService;
    }

    [HttpGet("get-list-by-billId/{billId}")]
    public async Task<IActionResult> GetListByBillId(int billId)
    {
        var results = await _billDetailService.GetListByBillId(billId);
        return Ok(new BaseResponseCommonModel
        {
            Data = results,
        });
    }

    [HttpGet("bill-details-and-promotions/{billId}")]
    public async Task<IActionResult> GetListBillDetailAndPromotion(int billId)
    {
        var results = await _billDetailService.GetListBillDetailAndPromotion(billId);
        return Ok(new BaseResponseCommonModel
        {
            Data = results,
        });
    }

    [HttpGet("get-list-by-billId-for-warehouse/{billId}")]
    public async Task<IActionResult> GetListByBillIdForWareHouse([FromHeader] int yearFilter, int billId)
    {
        var results = await _billDetailService.GetListByBillIdForWareHouse(billId, yearFilter);
        return Ok(new BaseResponseModel
        {
            Data = results,
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromHeader] int yearFilter, [FromBody] List<BillDetailModel> model)
    {
        try
        {
            var result = await _billDetailService.Create(model, yearFilter);
            if (result != null)
                return Ok(new BaseResponseModel { Data = result });
            return BadRequest(new { msg = "Create bill detail fail" });
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("note")]
    public async Task<IActionResult> UpdateNote([FromBody] List<BillDetailNoteRequestModel> model)
    {
        try
        {
            var result = await _billDetailService.UpdateNote(model);
            if (result != null)
                return Ok(new BaseResponseModel { Data = result });
            return BadRequest(new { msg = "Update note detail fail" });
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _billDetailService.Delete(id);
        return Ok();
    }

    [HttpPut("refund-goods/{billId}")]
    public async Task<IActionResult> RefundGoods([FromHeader] int yearFilter, [FromBody] BillRefundModel billRefund, [FromRoute] int billId)
    {
        await _billDetailService.RefundGoodsAsync(billRefund, billId, yearFilter);
        return Ok();
    }
}