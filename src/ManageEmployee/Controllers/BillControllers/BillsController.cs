using Common.Constants;
using ManageEmployee.Filters;
using ManageEmployee.Helpers;
using ManageEmployee.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using System.Security.Claims;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Bills;
using ManageEmployee.Services.Interfaces.Customers;
using ManageEmployee.Services.Interfaces.DeskFloors;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Extends;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Entities.CustomerEntities;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.InvoiceModel.WinInvoice;

namespace ManageEmployee.Controllers.BillControllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BillsController : ControllerBase
{
    private readonly IBillService _billService;
    private readonly IBillTrackingService _billTrackingService;
    private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
    private readonly ApplicationDbContext _context;
    private readonly IDeskFloorService _deskFloorService;
    private readonly IFileService _fileService;
    private readonly ICustomerService _customerService;
    private readonly IBillExporter _billExporter;
    private readonly IBillForSaleReporter _billForSaleReporter;
    public BillsController(
        IBillService billService,
        IBillTrackingService billTrackingService,
        IHubContext<BroadcastHub, IHubClient> hubContext,
        ApplicationDbContext context,
        IDeskFloorService deskFloorService,
        IFileService fileService,
        ICustomerService customerService,
        IBillExporter billExporter,
        IBillForSaleReporter billForSaleReporter)
    {
        _billService = billService;
        _billTrackingService = billTrackingService;
        _hubContext = hubContext;
        _context = context;
        _deskFloorService = deskFloorService;
        _fileService = fileService;
        _customerService = customerService;
        _billExporter = billExporter;
        _billForSaleReporter = billForSaleReporter;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll([FromQuery] BillRequestModel param)
    {
        var result = await _billService.GetAll(param);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var bill = await _billService.GetById(id);
        return Ok(new BaseResponseModel { Data = bill });
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] BillModel model)
    {
        model.UserCreated = HttpContext.GetIdentityUser().Id;
        Bill result = await _billService.Create(model);

        _deskFloorService.UpdateDeskChoose(result.DeskId, !model.IsPayment);
        return Ok(new BaseResponseModel { Data = result });
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] BillModel model)
    {
        try
        {
            Bill result = await _billService.Update(model);
            if (result != null)
                return Ok(new BaseResponseModel { Data = result });
            return BadRequest(new { msg = "Update bill fail" });
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("complete/{billId}")]
    public async Task<IActionResult> Complete([FromHeader] int yearFilter, int billId, [FromBody] BillModel model)
    {
        var result = await _billTrackingService.onCompleteBill(billId, model, yearFilter);
        return Ok(new ObjectReturn { message = "Complete bill success", status = 200, data = result });
    }

    [HttpPut("sendToChef/{billId}")]
    public async Task<IActionResult> sendToChef(int billId)
    {
        try
        {
            var result = await _billTrackingService.onSendToChefBill(billId);
            if (result != null)
                return Ok(new BaseResponseModel { Data = "Send bill to chef success" });
            return BadRequest(new { msg = "Send bill to chef fail" });
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("reveivedBill/{billId}")]
    public async Task<IActionResult> receivedBill([FromRoute] int billId, [FromQuery] int userId)
    {
        var result = await _billTrackingService.receivedBill(billId, userId);
        if (result != null)
            return Ok(new BaseResponseModel { Data = "Received bill success" });
        return BadRequest(new { msg = "Received bill fail" });
    }

    [HttpDelete("delete/{id}")]
    public IActionResult Delete(int id)
    {
        _billService.Delete(id);
        return Ok();
    }

    [HttpGet("ExportBill")]
    public async Task<IActionResult> ExportBill([FromQuery] BillRequestModel param)
    {
        param.PageSize = 0;
        param.Page = 0;
        var result = await _billService.GetAll(param);

        string _fileMapServer = $"danhsach_Bill_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx",
               folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\EXCEL"),
               _pathSave = Path.Combine(folder, _fileMapServer);

        List<BillPaging> datas = result.Data;
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Excel\\danhsach_Bill.xlsx");
        using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                int nRowBegin = 4;
                int rowIdx = nRowBegin;
                if (datas.Count > 0)
                {
                    sheet.Cells.Style.WrapText = true;
                    sheet.Cells[rowIdx, 1].Value = "TỔNG CỘNG";
                    sheet.Cells[rowIdx, 1, rowIdx, 3].Merge = true;

                    sheet.Cells[rowIdx, 4].Value = datas[0].QuantityCustomer;
                    sheet.Cells[rowIdx, 6].Value = datas[0].TotalAmount;

                    rowIdx++;
                    int i = 0;
                    foreach (BillPaging item in datas)
                    {
                        if (i == 0)
                        {
                            i++;
                            continue;
                        }
                        sheet.Cells.Style.WrapText = true;
                        sheet.Cells[rowIdx, 1].Value = item.Id;
                        sheet.Cells[rowIdx, 2].Value = item.UserCode;
                        sheet.Cells[rowIdx, 3].Value = item.CustomerName;
                        sheet.Cells[rowIdx, 4].Value = item.QuantityCustomer;
                        if (item.DiscountType == "percent")
                            sheet.Cells[rowIdx, 5].Value = "Giảm " + item.DiscountPrice + "VND/SP";
                        else if (item.DiscountType == "money")
                            sheet.Cells[rowIdx, 5].Value = "Giảm " + item.DiscountPrice + "/SP";

                        sheet.Cells[rowIdx, 6].Value = item.TotalAmount;
                        sheet.Cells[rowIdx, 7].Value = item.AmountReceivedByCus;
                        sheet.Cells[rowIdx, 8].Value = item.AmountSendToCus;
                        sheet.Cells[rowIdx, 9].Value = item.CreatedDate.ToString("dd/MM/yyyy");
                        sheet.Cells[rowIdx, 10].Value = item.Note;
                        rowIdx++;
                    }
                }
                rowIdx--;
                if (rowIdx >= nRowBegin)
                {
                    sheet.Cells[nRowBegin, 6, rowIdx, 8].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                    sheet.Cells[nRowBegin, 1, rowIdx, 10].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 10].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 10].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 10].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                    sheet.Cells[nRowBegin, 1, rowIdx, 10].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 10].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 10].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 10].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                using (FileStream fs = new FileStream(_pathSave, FileMode.Create))
                {
                    package.SaveAs(fs);
                }
            }
        }
        return Ok(new BaseResponseModel
        {
            Data = _fileMapServer
        });
    }

    [HttpPost("createBillEmployee")]
    public async Task<IActionResult> createBillEmployee([FromBody] BillModel model)
    {
        try
        {
            //_deskFloorService.UpdateDeskChoose(model.DeskId, true);

            Bill result = await _billService.CreateBillEmployee(model);
            if (result != null)
            {
                BillTracking billTracking = new BillTracking()
                {
                    BillId = result.Id,
                    CustomerName = result.CustomerName,
                    UserCode = result.UserCode,
                    TranType = model.IsPayment ? TranTypeConst.Paid : TranTypeConst.SendToCashier,
                    Status = "Success",
                    DisplayOrder = result.DisplayOrder ?? 1
                };
                if (model.IsPayment)
                {
                    billTracking.TranType = TranTypeConst.Paid;
                }
                else
                {
                    billTracking.TranType = model.UserType == "cashier" ? "SendToChef" : "SendToCashier";
                }
                await _context.BillTrackings.AddAsync(billTracking);
                try
                {
                    await _context.SaveChangesAsync();
                    //await _hubContext.Clients.All.BroadcastMessage();
                }
                catch (Exception ex)
                {
                    // return error message if there was an exception
                    return BadRequest(new { msg = ex.Message });
                }
            }
            return Ok(new BaseResponseModel { Data = result });
        }
        catch (Exception ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpGet("GetListBaoCaoLoiNhuan")]
    public async Task<IActionResult> GetListBaoCaoLoiNhuan([FromQuery] SearchViewModel param)
    {
        var result =await _billForSaleReporter.BaoCaoLoiNhuanTruThue(param.Page, param.PageSize, param.StartDate ?? DateTime.Today, param.EndDate ?? DateTime.Today, param.Type ?? 1);
        return Ok(result);
    }

    [HttpGet("GetListBaoCaoDoanhThuTheoNgay")]
    public async Task<IActionResult> GetListBaoCaoDoanhThuTheoNgay([FromQuery] SearchViewModel param)
    {
        var result = await _billForSaleReporter.BaoCaoDoanhThuTheoNgay(param.Page, param.PageSize, param.StartDate ?? DateTime.Today, param.EndDate ?? DateTime.Today);
        return Ok(result);
    }

    [HttpGet("BaoCaoDoanhThuTheoNgay")]
    public async Task<IActionResult> BaoCaoDoanhThuTheoNgay([FromQuery] SearchViewModel param)
    {
        var _fileMapServer = await _billForSaleReporter.BaoCaoDoanhThuTheoNgayExcel(param);
        return Ok(new BaseResponseModel
        {
            Data = _fileMapServer
        });
    }

    [HttpGet("BaoCaoLoiNhuanTruThue")]
    public async Task<IActionResult> BaoCaoLoiNhuanTruThue([FromQuery] SearchViewModel param)
    {
        string _fileMapServer = await _billForSaleReporter.BaoCaoLoiNhuanTruThueExcel(param);
        return Ok(new BaseResponseModel
        {
            Data = _fileMapServer
        });
    }

    [HttpGet("GetBillIdNewest/{billPrefix}")]
    [TypeFilter(typeof(ResponseWrapperFilterAttribute))]
    public async Task<IActionResult> GetBillIdNewest(string? billPrefix = "")
    {
        if (string.IsNullOrEmpty(billPrefix))
        {
            var bilLOrder = _billService.GetBillTrackingOrder();
            return Ok(bilLOrder);
        }

        var (billOrder, billNumber) = await _billService.GetBillTrackingOrder(billPrefix);
        return Ok(new { billOrder, billNumber });
    }

    [HttpGet("get-next-bill-number")]
    [TypeFilter(typeof(ResponseWrapperFilterAttribute))]
    public async Task<IActionResult> GetBillIdNewest([FromQuery] string? billPrefix, [FromQuery] List<int> excludeIds)
    {
        var (billOrder, billNumber) = await _billService.GetBillTrackingOrder(billPrefix, excludeIds);
        return Ok(new { billOrder, billNumber });
    }

    [HttpGet("ExportBill-detail")]
    public async Task<IActionResult> ExportBillDetail([FromQuery] BillRequestModel param)
    {
        param.Page = 0;
        param.PageSize = 0;
        var result = await _billService.GetAll(param, true);

        string _fileMapServer = $"danhsach_Bill_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx",
               folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\EXCEL"),
               _pathSave = Path.Combine(folder, _fileMapServer);

        List<BillPaging> datas = result.Data;
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Excel\\danhsach_Bill_detail.xlsx");
        using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                int nRowBegin = 4;
                int rowIdx = nRowBegin;
                if (datas.Count > 0)
                {
                    foreach (BillPaging item in datas)
                    {
                        sheet.Cells[rowIdx, 1].Value = item.Id;
                        sheet.Cells[rowIdx, 2].Value = item.UserCode;
                        sheet.Cells[rowIdx, 3].Value = item.CustomerName;
                        sheet.Cells[rowIdx, 4].Value = item.QuantityCustomer;
                        if (item.DiscountType == "percent")
                            sheet.Cells[rowIdx, 5].Value = "Giảm " + item.DiscountPrice + "VND/SP";
                        else if (item.DiscountType == "money")
                            sheet.Cells[rowIdx, 5].Value = "Giảm " + item.DiscountPrice + "/SP";

                        sheet.Cells[rowIdx, 6].Value = item.TotalAmount;
                        sheet.Cells[rowIdx, 7].Value = item.AmountReceivedByCus;
                        sheet.Cells[rowIdx, 8].Value = item.AmountSendToCus;
                        sheet.Cells[rowIdx, 9].Value = item.CreatedDate.ToString("dd/MM/yyyy");
                        sheet.Cells[rowIdx, 10].Value = item.Note;
                        rowIdx++;

                        sheet.Cells[rowIdx, 1].Value = "Tên sản phẩm";
                        sheet.Cells[rowIdx, 1, rowIdx, 3].Merge = true;
                        sheet.Cells[rowIdx, 4].Value = "Đơn giá";
                        sheet.Cells[rowIdx, 5].Value = "Số lượng";
                        sheet.Cells[rowIdx, 6].Value = "Giảm giá";
                        sheet.Cells[rowIdx, 7].Value = "Thuế VAT";
                        sheet.Cells[rowIdx, 8].Value = "Tổng tiền";
                        sheet.Cells[rowIdx, 9].Value = "";
                        sheet.Cells[rowIdx, 10].Value = "";
                        sheet.Cells[rowIdx, 1, rowIdx, 10].Style.Font.Bold = true;
                        rowIdx++;

                        List<BillDetail> billDetails = _context.BillDetails.Where(x => x.BillId == item.Id).ToList();
                        foreach (BillDetail billDetail in billDetails)
                        {
                            var good = _context.Goods.Find(billDetail.GoodsId);
                            if (good == null)
                                continue;
                            sheet.Cells[rowIdx, 1].Value = !string.IsNullOrEmpty(good.Detail2) ? good.DetailName2 :
                                !string.IsNullOrEmpty(good.Detail1) ? good.DetailName1 : good.AccountName;
                            sheet.Cells[rowIdx, 1, rowIdx, 3].Merge = true;
                            sheet.Cells[rowIdx, 4].Value = billDetail.UnitPrice;
                            sheet.Cells[rowIdx, 5].Value = billDetail.Quantity;
                            if (billDetail.DiscountType == "percent")
                                sheet.Cells[rowIdx, 6].Value = "Giảm " + billDetail.DiscountPrice + "VND/SP";
                            else if (billDetail.DiscountType == "money")
                                sheet.Cells[rowIdx, 6].Value = "Giảm " + billDetail.DiscountPrice + "/SP";
                            sheet.Cells[rowIdx, 7].Value = billDetail.TaxVAT;
                            sheet.Cells[rowIdx, 8].Value = billDetail.UnitPrice * billDetail.Quantity - billDetail.DiscountPrice - billDetail.TaxVAT;
                            rowIdx++;
                        }
                    }
                }
                rowIdx--;
                if (rowIdx >= nRowBegin)
                {
                    sheet.Cells[nRowBegin, 6, rowIdx, 8].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                    sheet.Cells[nRowBegin, 1, rowIdx, 10].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 10].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 10].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 10].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                    sheet.Cells[nRowBegin, 1, rowIdx, 10].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 10].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 10].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 10].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                using (FileStream fs = new FileStream(_pathSave, FileMode.Create))
                {
                    package.SaveAs(fs);
                }
            }
        }
        return Ok(new BaseResponseModel
        {
            Data = _fileMapServer
        });
    }

    [HttpGet("good-report-sale")]
    public async Task<IActionResult> GetGoodForCustomer([FromQuery] BillReportRequestModel param)
    {
        var result = await _billService.GetGoodForCustomer(param);
        return Ok(result);
    }

    [HttpGet("export-good-report-sale")]
    public async Task<IActionResult> ExportGoodForCustomer([FromQuery] BillReportRequestModel param)
    {
        var result = await _billExporter.ExportExcelSale(param);
        return Ok(new BaseResponseModel
        {
            Data = result
        });
    }

    [HttpGet("bill-customer-invoice/{id}")]
    public IActionResult GetBillForCustomerInvoice(int id)
    {
        var result = _billService.GetBillForCustomerInvoice(id);
        return Ok(new BaseResponseModel
        {
            Data = result
        });
    }

    [HttpPut("bill-customer-invoice/{id}")]
    public async Task<IActionResult> UpdateCustomerInvoice([FromHeader] int yearFilter, CustomerTaxInformation customer, int statusPrintInvoice, int id)
    {
        await _billService.UpdateCustomerInvoice(customer, statusPrintInvoice, id, yearFilter);
        return Ok(new BaseResponseModel
        {
            Data = ""
        });
    }

    [HttpGet("bill-invoice/{id}")]
    public async Task<IActionResult> GetInvoiceForBill([FromHeader] int yearFilter, int id)
    {
        var result = await _billService.GetInvoiceForBill(id, yearFilter);
        return Ok(new BaseResponseModel
        {
            Data = result
        });
    }

    [HttpGet("bill-copy/{id}")]
    public async Task<IActionResult> CopyBill(int id)
    {
        int userId = 0;
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
        }
        await _billService.CopyBill(id, userId);
        return Ok();
    }

    [HttpGet("update-surcharge/{id}")]
    public async Task<IActionResult> UpdateSurCharge(int id, double surcharge)
    {
        await _billService.UpdateSurCharge(id, surcharge);
        return Ok();
    }

    [HttpPut("cancel-bill/{billId}")]
    public async Task<IActionResult> CancelBill([FromRoute] int billId, [FromQuery] int userId)
    {
        try
        {
            var result = await _billTrackingService.CancelBill(billId, userId);
            if (result != null)
                return Ok(new BaseResponseModel { Data = "Received bill success" });
            return BadRequest(new { msg = "Received bill fail" });
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpGet("customer-for-report-bill")]
    public async Task<IActionResult> GetCustomerForReportBill([FromQuery] BillReportRequestModel param)
    {
        var customers = await _billService.GetCustomerForReportBill(param);

        return Ok(new ObjectReturn
        {
            data = customers,
            status = 200,
        });
    }

    [HttpGet("user-for-report-bill")]
    public async Task<IActionResult> GetUserForReportBill([FromQuery] BillReportRequestModel param)
    {
        var customers = await _billService.GetUserForReportBill(param);

        return Ok(new ObjectReturn
        {
            data = customers,
            status = 200,
        });
    }

    [HttpGet("account-for-report-bill")]
    public async Task<IActionResult> GetChartOfAccountForReportBill([FromQuery] BillReportRequestModel param)
    {
        var customers = await _billService.GetChartOfAccountForReportBill(param);

        return Ok(new ObjectReturn
        {
            data = customers,
            status = 200,
        });
    }

    [HttpPost("import-bill")]
    [AllowAnonymous]
    public async Task<IActionResult> ImportBill([FromForm] IFormFile file, [FromQuery] int type)
    {
        var fileName = _fileService.Upload(file, "Bills", file.FileName);

        var res = await _billService.ImportBill(fileName, type);
        return Ok(new ObjectReturn
        {
            data = res,
            status = 200,
        });
    }

    [HttpGet("export-pdf-good-report-sale")]
    public async Task<IActionResult> ExportPdfGoodForCustomer([FromQuery] BillReportRequestModel param)
    {
        var result = await _billExporter.ExportPdfGoodForSale(param);
        return Ok(new BaseResponseModel
        {
            Data = result
        });
    }

    [HttpGet("ledger-from-bill")]
    public async Task<IActionResult> GetLedgerFromBillId(int billId)
    {
        var result = await _billService.GetLedgerFromBillId(billId);
        return Ok(new BaseResponseModel
        {
            Data = result
        });
    }

    [HttpGet("get-bill-pdf/{id}")]
    [TypeFilter(typeof(ResponseWrapperFilterAttribute))]
    public async Task<IActionResult> GetBillById([FromHeader] int yearFilter, [FromRoute] int id)
    {
        var (bill, goods) = await _billService.GetBillPdfByIdAsync(id);
        var customer = await _customerService.GetById(bill?.CustomerId ?? 0, yearFilter, true);
        return Ok(new { bill, customer, goods });
    }

    [HttpPut("user-sale/{id}")]
    public async Task<IActionResult> UpdateUserSale(int id, int userId)
    {
        await _billService.UpdateUserSale(id, userId);
        return Ok();
    }

    [HttpPost("create-invoice")]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceRequest request)
    {
        var result = await _billService.CreateInvoice(request.Id);
        int statusCode = result.success ? 200 : 400;

        return new ObjectResult(new
        {
            Success = result.success,
            Message = result.msg,
        })
        {
            StatusCode = statusCode
        };
    }

    [HttpGet("get-invoice-link/{id}")]
    public async Task<IActionResult> GetInvoiceLink([FromRoute] int id)
    {
        var result = await _billService.GetLinkInvoice(id);
        int statusCode = result.success ? 200 : 400;

        return new ObjectResult(new
        {
            Success = result.success,
            Message = result.msg,
            Link = result.link
        })
        {
            StatusCode = statusCode
        };
    }

    [HttpPost("digitally-sign-invoice")]
    public async Task<IActionResult> DigitallySignInvoice([FromQuery] int Id)
    {
        var res = await _billService.DigitallySignInvoice(Id);
        int statusCode = res.success ? 200 : 400;

        return new ObjectResult(new
        {
            Success = res.success,
            Message = res.msg,
        })
        {
            StatusCode = statusCode
        };
    }

    [HttpPost("delete-invoice")]
    public async Task<IActionResult> DeleteInvoice([FromQuery] int Id)
    {
        var res = await _billService.DeleteSignInvoice(Id);
        int statusCode = res.success ? 200 : 400;

        return new ObjectResult(new
        {
            Success = res.success,
            Message = res.msg,
        })
        {
            StatusCode = statusCode
        };
    }

}