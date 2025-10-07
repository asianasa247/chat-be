using AutoMapper;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.InOut;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Timekeep;
using ManageEmployee.DataTransferObject.TimeKeeperModels;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Extends;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.InOuts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Security.Claims;
using Report = ManageEmployee.DataTransferObject.TimeKeeperModels.Report;

namespace ManageEmployee.Controllers.InOutControllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class InOutController : ControllerBase
{
    private readonly IInOutService _inOutService;
    private readonly ICompanyService _companyService;
    private readonly IMapper _mapper;
    private readonly IInOutImporter _inOutImporter;

    public InOutController(
        IInOutService inOutService,
        IMapper mapper, ICompanyService companyService, IInOutImporter inOutImporter)
    {
        _inOutService = inOutService;
        _mapper = mapper;
        _companyService = companyService;
        _inOutImporter = inOutImporter;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ManualCheckInUserRequest request)
    {
        var identityUser = HttpContext.GetIdentityUser();

        var result = await _inOutService.GetManualCheckInUsers(request, identityUser.Id, identityUser.Role);

        return Ok(result);
    }

    [HttpGet("countTargetId")]
    public async Task<IActionResult> CountTargetId()
    {
        var inOutHistories = await _inOutService.GetAll();

        var model = _mapper.Map<IList<InOutHistory>>(inOutHistories);
        return Ok(new BaseResponseModel
        {
            Data = model,
        });
    }

    /// <hung123>
    

    [HttpPost("findAll")]
    public async Task<IActionResult> FindAll([FromBody] InOutHistoryFilterDateParams param)
    {
        var identityUser = HttpContext.GetIdentityUser();

        var inOutHistories = await _inOutService.GetPaging(param, identityUser.Id, identityUser.Role);

        return Ok(inOutHistories);
    }

    [HttpPut("{id}")]
    [HttpPost]
    public async Task<IActionResult> Save([FromBody] InOutHistoryViewModel model, int id = 0)
    {
        // map model to entity and set id
        var inOutHistory = _mapper.Map<InOutHistory>(model);
        var date = inOutHistory.TimeIn.Value.AddHours(7);
        inOutHistory.TimeOut = new DateTime(date.Year, date.Month, date.Day);
        inOutHistory.TimeIn = inOutHistory.TimeOut;
        inOutHistory.Id = id;
        try
        {
            var check2 = _inOutService.HasPreviousCheckInWithAnotherSymbolId(model.UserId ?? 0, inOutHistory.TimeIn, inOutHistory.SymbolId);

            if (inOutHistory.Id != 0 && check2)
            {
                await _inOutService.Update(inOutHistory);
            }
            else
            {
                inOutHistory.Id = 0;
                var resulCommit = await _inOutService.Create(inOutHistory);
                inOutHistory.Id = resulCommit.Id;
            }
            // update user
            return Ok(_mapper.Map<InOutHistoryViewModel>(inOutHistory));
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPost("update/{id}")]
    [HttpPost("update")]
    public async Task<IActionResult> Update(int id, [FromBody] InOutHistoryViewModel model)
    {
        var inOutHistory = _mapper.Map<InOutHistory>(model);
        await _inOutService.Update(inOutHistory);

        return Ok(_mapper.Map<InOutHistoryViewModel>(inOutHistory));
    }

    [HttpPost("updatechecked/{id}")]
    [HttpPost("updatechecked")]
    public async Task<IActionResult> UpdateChecked(int id)
    {
        // map model to entity and set id
        await _inOutService.UpdateCheckedAsync(id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDetail(int id)
    {
        await _inOutService.Delete(id);
        return Ok();
    }

    [HttpPost("historybyuser")]
    public async Task<IActionResult> GetHistoryByUser([FromBody] InOutHistoryFilterDateParams param)
    {
        var identityUser = HttpContext.GetIdentityUser();

        var fromdate = param.FromDate;
        var todate = param.ToDate;
        var timekeeps = new PagingResult<InOutHistoryViewModel>();
        if (param.CheckCurrentUser)
        {
            timekeeps = await _inOutService.GetAllByUserId(param.SearchText, param.DepartmentId, param.TargetId ?? 0, fromdate, todate, param.Page, param.PageSize, identityUser.Id);
        }
        else
        {
            timekeeps = await _inOutService.GetPaging(param, identityUser.Id, identityUser.Role);
        }
        if (timekeeps.Data.Any())
        {
            return Ok(timekeeps);
        }
        return Ok(new BaseResponseModel
        {
            TotalItems = 0,
            Data = null,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    ///// <summary>
    ///// Báo cáo
    ///// </summary>
    ///// <param name="param"></param>
    ///// <returns></returns>
    [HttpPost("report")]
    public async Task<IActionResult> GetReport([FromBody] TimeKeepViewModel param)
    {
        var identityUser = HttpContext.GetIdentityUser();
        var timekeeps = await _inOutService.GetReport(param, identityUser.Id, identityUser.Role);
        if (timekeeps.Any())
        {
            return Ok(new BaseResponseModel
            {
                TotalItems = timekeeps.Count(),
                Data = timekeeps.Skip(param.PageSize * (param.Page > 0 ? param.Page - 1 : param.Page))
        .Take(param.PageSize),
                PageSize = param.PageSize,
                CurrentPage = param.Page
            });
        }
        return Ok(new BaseResponseModel
        {
            TotalItems = 0,
            Data = null,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpPost("report-v2")]
    public async Task<IActionResult> GetReportV2([FromBody] TimeKeepViewModel param)
    {
        var identityUser = HttpContext.GetIdentityUser();
        var result = await _inOutService.TimeKeepingReportV2(param, identityUser.Id, identityUser.Role);
        return Ok(result);
    }

    ///// <summary>
    ///// Xuất file excel
    ///// </summary>
    ///// <param name="param"></param>
    ///// <returns></returns>
    [HttpPost("exportreport")]
    public async Task<IActionResult> Export([FromBody] TimeKeepViewModel param)
    {
        var identityUser = HttpContext.GetIdentityUser();

        var result = await _inOutService.TimeKeepingReportV2(
            param, 
            identityUser.Id, 
            identityUser.Role
        );

        var date = result.Date;

        if (result.TotalItems > 0)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/chamcong_template.xlsx");
            MemoryStream stream = new MemoryStream();
            using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                    sheet.DefaultColWidth = 10.0;
                    int rowIdx = 9;
                    var listDay = new List<ExportExcelModel>();

                    // Tên công ty, tiêu đề
                    var company = await _companyService.GetCompany();

                    sheet.Cells["A1:AN1"].Value = company.Name;
                    sheet.Cells["A2:AN2"].Value = company.Address;
                    sheet.Cells["A3:AN3"].Value = company.MST;
                    sheet.Cells["A4:AN4"].Value = string.Empty;

                    sheet.Cells["A6"].Value = "Từ " + date.StrFromDate + " đến " + date.StrToDate;
                    sheet.Cells["A7:AN7"].Value = string.Empty;

                    foreach (var item in date.Dates)
                    {
                        listDay.Add(new ExportExcelModel()
                        {
                            DateKeep = item,
                            DateKeepStr = item.ToString("dd/MM"),
                        });
                    }

                    if (listDay != null && listDay.Any())
                    {
                        listDay.Add(new ExportExcelModel()
                        {
                            DateKeep = null,
                            DateKeepStr = "Ngày công",
                        });
                        listDay.Add(new ExportExcelModel()
                        {
                            DateKeep = null,
                            DateKeepStr = "Tổng giờ",
                        });
                        listDay.Add(new ExportExcelModel()
                        {
                            DateKeep = null,
                            DateKeepStr = "Tăng ca",
                        });

                        listDay.Add(new ExportExcelModel()
                        {
                            DateKeep = null,
                            DateKeepStr = "Không Phép",
                        });
                        listDay.Add(new ExportExcelModel()
                        {
                            DateKeep = null,
                            DateKeepStr = "Phép không lương",
                        });
                        listDay.Add(new ExportExcelModel()
                        {
                            DateKeep = null,
                            DateKeepStr = "Phép có lương",
                        });
                        listDay.Add(new ExportExcelModel()
                        {
                            DateKeep = null,
                            DateKeepStr = "Tổng ngày tính công",
                        });
                        int index = 4;
                        sheet.Cells["A8"].Value = "STT";
                        sheet.Cells["B8"].Value = "Mã NV";
                        sheet.Cells["C8"].Value = "Tên nhân viên";
                        sheet.SelectedRange["A8:C8"].Style.Fill.SetBackground(System.Drawing.Color.Orange);
                        sheet.SelectedRange["A8:C8"].Style.Font.Bold = true;
                        foreach (var item in listDay)
                        {
                            sheet.Cells[8, index].Value = item.DateKeepStr;
                            sheet.Cells[8, index].Style.Fill.SetBackground(System.Drawing.Color.Orange);
                            sheet.Cells[8, index].Style.Font.Bold = true;
                            index++;
                        }
                    }

                    var indexOfItem = 1;
                    foreach (var lo in result.Data)
                    {
                        sheet.Cells.Style.WrapText = true;
                        sheet.Cells[rowIdx, 1].Value = indexOfItem;
                        sheet.Cells[rowIdx, 2].Value = lo.UserId;
                        sheet.Cells[rowIdx, 3].Value = lo.FullName;
                        int index = 4;

                        foreach (var item in lo.Histories)
                        {
                            sheet.Cells[rowIdx, index].Value = item.SymbolCode;
                            index++;
                        }

                        sheet.Cells[rowIdx, index++].Value = lo.TotalWorkingDay;

                        sheet.Cells[rowIdx, index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Cells[rowIdx, index++].Value = lo.TotalWorkingHours;

                        sheet.Cells[rowIdx, index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Cells[rowIdx, index++].Value = lo.TotalOverTimeWorkingHours;

                        sheet.Cells[rowIdx, index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Cells[rowIdx, index++].Value = lo.TotalUnLicensed; 

                        sheet.Cells[rowIdx, index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Cells[rowIdx, index++].Value = lo.TotalUnPaidLeave; 

                        sheet.Cells[rowIdx, index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Cells[rowIdx, index++].Value = lo.TotalPaidLeave; 

                        sheet.Cells[rowIdx, index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        sheet.Cells[rowIdx, index++].Value = lo.TotalPaid;

                        sheet.Cells[rowIdx, index].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        rowIdx++;
                        indexOfItem++;
                    }
                    package.SaveAs(stream);
                }
            }
            stream.Seek(0L, SeekOrigin.Begin);
            string fileName = string.Format("ChamCong_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"));
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        else
        {
            return BadRequest(new { msg = "Không có dữ liệu xuất file" });
        }
    }

    ///// <summary>
    ///// Xuất file excel
    ///// </summary>
    ///// <param name="param"></param>
    ///// <returns></returns>
    [HttpPost("exportreportexcel")]
    public async Task<IActionResult> ExportExcel([FromBody] TimeKeepViewModel param)
    {
        DateTime date = DateTime.Now;
        var Fromdate = param.FromDate.HasValue ? param.FromDate.Value : new DateTime(date.Year, date.Month, 1).Date;
        var Todate = param.ToDate.HasValue ? param.ToDate.Value : new DateTime().Date;

        var datas = _inOutService.ExportReport(Fromdate, Todate, param.DepartmentId ?? 0, param.TargetId ?? 0, param.SearchText).ToList();

        if (datas.Any())
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/exportHours.xlsx");
            MemoryStream stream = new MemoryStream();
            using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                    sheet.DefaultColWidth = 10.0;
                    int rowIdx = 9;
                    var listDay = new List<ExportExcelModel>();
                    Fromdate = Fromdate.AddHours(7);
                    Todate = Todate.AddHours(7);

                    // Tên công ty, tiêu đề
                    var company = await _companyService.GetCompany();

                    sheet.Cells["A1:AN1"].Value = company.Name;
                    sheet.Cells["A2:AN2"].Value = company.Address;
                    sheet.Cells["A3:AN3"].Value = company.MST;
                    sheet.Cells["A4:AN4"].Value = string.Empty;

                    sheet.Cells["A6"].Value = "Từ " + Fromdate.ToString("dd/MM/yyyy") + "đến " + Todate.ToString("dd/MM/yyyy");
                    sheet.Cells["A7:AN7"].Value = string.Empty;
                    while (Fromdate <= Todate)
                    {
                        listDay.Add(new ExportExcelModel()
                        {
                            DateKeep = Fromdate,
                            DateKeepStr = Fromdate.ToString("dd/MM"),
                        });
                        Fromdate = Fromdate.AddDays(1);
                    }
                    if (listDay != null && listDay.Any())
                    {
                        listDay.Add(new ExportExcelModel()
                        {
                            DateKeep = null,
                            DateKeepStr = "Ngày công",
                        });
                        listDay.Add(new ExportExcelModel()
                        {
                            DateKeep = null,
                            DateKeepStr = "Tổng giờ",
                        });
                        listDay.Add(new ExportExcelModel()
                        {
                            DateKeep = null,
                            DateKeepStr = "Tăng ca",
                        });
                        listDay.Add(new ExportExcelModel()
                        {
                            DateKeep = null,
                            DateKeepStr = "Nghỉ phép",
                        });
                        listDay.Add(new ExportExcelModel()
                        {
                            DateKeep = null,
                            DateKeepStr = "Nghỉ không phép",
                        });
                        listDay.Add(new ExportExcelModel()
                        {
                            DateKeep = null,
                            DateKeepStr = "Tổng ngày tính công",
                        });
                        int index = 4;
                        sheet.Cells["A8"].Value = "STT";
                        sheet.Cells["B8"].Value = "Mã NV";
                        sheet.Cells["C8"].Value = "Tên nhân viên";
                        sheet.SelectedRange["A8:C8"].Style.Fill.SetBackground(System.Drawing.Color.Orange);
                        sheet.SelectedRange["A8:C8"].Style.Font.Bold = true;
                        foreach (var item in listDay)
                        {
                            sheet.Cells[8, index].Value = item.DateKeepStr;
                            sheet.Cells[8, index].Style.Fill.SetBackground(System.Drawing.Color.Orange);
                            sheet.Cells[8, index].Style.Font.Bold = true;
                            sheet.Cells[8, index].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                            sheet.Cells[8, index].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            index++;
                        }
                    }
                    var indexOfItem = 1;
                    foreach (Report lo in datas)
                    {
                        sheet.Cells[rowIdx, 1].Value = indexOfItem;
                        sheet.Cells[rowIdx, 2].Value = lo.Code;
                        sheet.Cells[rowIdx, 3].Value = lo.FullName;
                        int index = 4;
                        double total1 = 0;
                        double total2 = 0;
                        double total3 = 0;
                        double total4 = 0;
                        double total5 = 0;
                        double total6 = 0;
                        foreach (var item in listDay)
                        {
                            if (lo.Histories != null && lo.Histories.Any() && item.DateKeep.HasValue)
                            {
                                var valueRow = lo.Histories.Where(x => x.TimeIn.Value.Date == item.DateKeep.Value.Date);
                                if (valueRow.Any() && valueRow.FirstOrDefault()?.TimeKeepSymbolId != 0)
                                {
                                    total3 += 1;
                                }

                                var listTime = valueRow.Where(x => x.CheckInMethod == 1).Select(x => x.TimeKeepSymbolTimeTotal);
                                var listTime2 = valueRow.Where(x => x.CheckInMethod == 2).Select(x => x.TimeKeepSymbolTimeTotal);
                                foreach (var time in listTime)
                                {
                                    total1 += time;
                                }
                                foreach (var time in listTime2)
                                {
                                    total2 += time;
                                }
                            }
                        }
                        var listTime4 = lo.Histories.Where(x => x.IsOverTime == 3).ToList();
                        var listTime5 = lo.Histories.Where(x => x.IsOverTime == 4).ToList();
                        foreach (var time in listTime4)
                        {
                            total4 += 1;
                        }
                        foreach (var time in listTime5)
                        {
                            total5 += 1;
                        }
                        total6 = total3 + total4;
                        index = index + 1;
                        sheet.Cells[rowIdx, index++].Value = total3;
                        sheet.Cells[rowIdx, index].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        sheet.Cells[rowIdx, index++].Value = total1;
                        sheet.Cells[rowIdx, index].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        sheet.Cells[rowIdx, index++].Value = total2;
                        sheet.Cells[rowIdx, index].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        sheet.Cells[rowIdx, index++].Value = total4;
                        sheet.Cells[rowIdx, index].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        sheet.Cells[rowIdx, index++].Value = total5;
                        sheet.Cells[rowIdx, index].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        sheet.Cells[rowIdx, index++].Value = total6;
                        sheet.Cells[rowIdx, index].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        rowIdx++;
                        indexOfItem++;
                    }
                    sheet.Cells.AutoFitColumns();
                    package.SaveAs(stream);
                }
            }
            stream.Seek(0L, SeekOrigin.Begin);
            string fileName = string.Format("ChamCong_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"));
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        else
        {
            return BadRequest(new { msg = "Không có dữ liệu xuất file" });
        }
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import([FromForm] IFormFile file)
    {
        await _inOutImporter.ImportAsync(file);
        return Ok();
    }

}

public class ExportExcelModel
{
public DateTime? DateKeep { get; set; }
public string? DateKeepStr { get; set; }
}