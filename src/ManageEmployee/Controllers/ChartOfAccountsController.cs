using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.ChartOfAccountModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Services.Interfaces;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace ManageEmployee.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ChartOfAccountsController : ControllerBase
{
    private readonly IChartOfAccountService _chartOfAccountService;
    private readonly IChartOfAccountUpdater _chartOfAccountUpdater;
    private readonly IChartOfAccountDetailUpdater _chartOfAccountDetailUpdater;
    private readonly IChartOfAccountDeleter _chartOfAccountDeleter;
    private readonly IConnectionStringProvider _connectionStringProvider;
    public ChartOfAccountsController(IChartOfAccountService chartOfAccountService,
        IChartOfAccountUpdater chartOfAccountUpdater,
        IChartOfAccountDetailUpdater chartOfAccountDetailUpdater,
        IChartOfAccountDeleter chartOfAccountDeleter,
        IConnectionStringProvider connectionStringProvider) 
    {
        _chartOfAccountService = chartOfAccountService;
        _chartOfAccountUpdater = chartOfAccountUpdater;
        _chartOfAccountDetailUpdater = chartOfAccountDetailUpdater;
        _chartOfAccountDeleter = chartOfAccountDeleter;
        _connectionStringProvider = connectionStringProvider;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllPaging([FromQuery] PagingRequestModel param, [FromHeader] int yearFilter)
    {
        var data = await _chartOfAccountService.GetAllAccounts(param, yearFilter);
        return Ok(new BaseResponseModel()
        {
            Data = data,
            TotalItems = data.Count,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] ChartOfAccount model, [FromHeader] int yearFilter)
    {
        var errorMessage = await _chartOfAccountService.Create(model, yearFilter);
        if (string.IsNullOrEmpty(errorMessage))
            return Ok(new ObjectReturn
            {
                message = ResultErrorConstants.SUCCESS,
                status = 200,
            });
        return Ok(new ObjectReturn
        {
            message = errorMessage,
            status = 400,
        });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ChartOfAccount model, [FromHeader] int yearFilter)
    {
        var errorMessage = await _chartOfAccountUpdater.Update(model, yearFilter);
        if (string.IsNullOrEmpty(errorMessage))
            return Ok(new ObjectReturn
            {
                message = ResultErrorConstants.SUCCESS,
                status = 200,
            });
        return Ok(new ObjectReturn
        {
            message = errorMessage,
            status = 400,
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromHeader] int yearFilter, long id)
    {
        await _chartOfAccountDeleter.Delete(id, yearFilter);
        return Ok(new ObjectReturn
        {
            message = ResultErrorConstants.SUCCESS,
            status = 200,
        });
    }

    [HttpGet("details/{parentCode}")]
    public async Task<IActionResult> GetDetails([FromQuery] ChartOfAccountDetailRequest param, [FromHeader] int yearFilter, string parentCode,
        string? warehouseCode = "", int id = 0)
    {
        var data = await _chartOfAccountService.GetAllDetails(param.Page, param.PageSize, parentCode,
            warehouseCode, param.SearchText, yearFilter, id, param.IsInternal);
        var totalItems = await _chartOfAccountService.Count(yearFilter, x => x.Type >= 5 && x.ParentRef == parentCode);
        return Ok(new BaseResponseModel()
        {
            Data = data,
            TotalItems = totalItems,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpPost("details")]
    public async Task<IActionResult> CreateAccountDetail([FromBody] ChartOfAccount model, [FromHeader] int yearFilter)
    {
        var errorMessage = await _chartOfAccountService.CreateDetail(model, yearFilter);
        if (string.IsNullOrEmpty(errorMessage))
            return Ok(new ObjectReturn
            {
                message = ResultErrorConstants.SUCCESS,
                status = 200,
            });
        return Ok(new ObjectReturn
        {
            message = errorMessage,
            status = 400,
        });
    }

    [HttpPut("details")]
    public async Task<IActionResult> UpdateAccountDetail([FromBody] ChartOfAccountModel model, [FromHeader] int yearFilter)
    {
        var errorMessage = await _chartOfAccountDetailUpdater.UpdateAccountDetail(model, yearFilter);
        if (string.IsNullOrEmpty(errorMessage))
            return Ok(new ObjectReturn
            {
                message = ResultErrorConstants.SUCCESS,
                status = 200,
            });
        return Ok(new ObjectReturn
        {
            message = errorMessage,
            status = 400,
        });
    }

    [HttpDelete("details/{id}")]
    public async Task<IActionResult> DeleteAccountDetail([FromHeader] int yearFilter, long id)
    {
        var errorMessage = await _chartOfAccountService.DeleteDetail(id, yearFilter);
        if (string.IsNullOrEmpty(errorMessage))
            return Ok(new ObjectReturn
            {
                message = ResultErrorConstants.SUCCESS,
                status = 200,
            });
        return Ok(new ObjectReturn
        {
            message = errorMessage,
            status = 400,
        });
    }

    [HttpPost("details/check-account-detail")]
    public async Task<IActionResult> CheckAccountDetail([FromBody] ChartOfAccount model, [FromHeader] int yearFilter)
    {
        var errorMessage = await _chartOfAccountService.CheckAccountDetail(model, yearFilter);
        return Ok(errorMessage);
    }

    [HttpGet("getAllByDisplayInsert")]
    public async Task<IActionResult> GetAllByDisplayInsert([FromHeader] int yearFilter, string? searchText = "")
    {
        PagingRequestModel param = new PagingRequestModel()
        {
            SearchText = searchText
        };
        var response = await _chartOfAccountService.GetAllByDisplayInsert(param, yearFilter);
        return Ok(response);
    }

    [HttpGet("ExportGetAllArisingAccounts")]
    public async Task<IActionResult> ExportGetAllArisingAccounts([FromHeader] int yearFilter)
    {
        var datas = await _chartOfAccountService.ExportGetAllArisingAccounts(yearFilter);

        string _fileMapServer = $"DanhSachTaiKhoan_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx",
                folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\EXCEL"),
                _pathSave = Path.Combine(folder, _fileMapServer);

        string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/danhsachtaikhoanArising_template.xlsx");
        MemoryStream stream = new MemoryStream();
        using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
        {
            using (ExcelPackage package = new(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                int nRowBegin = 4;
                int rowIdx = nRowBegin;
                var listTinhChat = await _chartOfAccountService.GetLookupValues("COA_ACC_GROUP");
                var listLoai = await _chartOfAccountService.GetLookupValues("COA_CLASSIFICATION");
                if (datas.Count > 0)
                {
                    foreach (ChartOfAccountModel item in datas)
                    {
                        sheet.Cells.Style.WrapText = true;
                        sheet.Cells[rowIdx, 1].Value = item.Code;
                        sheet.Cells[rowIdx, 2].Value = item.Name;
                        sheet.Cells[rowIdx, 3].Value = item.ParentRef;
                        sheet.Cells[rowIdx, 4].Value = item.WarehouseCode;
                        if (item.OpeningCredit > 0)
                        {
                            sheet.Cells[rowIdx, 5].Value = 0;
                            sheet.Cells[rowIdx, 6].Value = item.OpeningCredit;
                        }
                        else
                        {
                            sheet.Cells[rowIdx, 5].Value = item.OpeningCredit;
                            sheet.Cells[rowIdx, 6].Value = 0;
                        }
                        if (item.OpeningForeignCredit > 0)
                        {
                            sheet.Cells[rowIdx, 7].Value = 0;
                            sheet.Cells[rowIdx, 8].Value = item.OpeningForeignCredit;
                        }
                        else
                        {
                            sheet.Cells[rowIdx, 7].Value = item.OpeningForeignCredit;
                            sheet.Cells[rowIdx, 8].Value = 0;
                        }
                        sheet.Cells[rowIdx, 10].Value = "Hạch toán";
                        var accGroup = listTinhChat.Find(x => x.Code == item.AccGroup.ToString());
                        if (accGroup != null)
                            sheet.Cells[rowIdx, 9].Value = accGroup.Value;

                        var classifica = listLoai.Find(x => x.Code == item.Classification.ToString());
                        if (classifica != null)
                            sheet.Cells[rowIdx, 11].Value = classifica.Value;
                        rowIdx++;
                    }

                    rowIdx--;

                    var tinhChat = sheet.Cells[nRowBegin, 9, rowIdx, 9].DataValidation.AddListDataValidation();
                    foreach (var type in listTinhChat)
                    {
                        tinhChat.Formula.Values.Add(type.Value);
                    }
                    
                    var loai = sheet.Cells[nRowBegin, 11, rowIdx, 11].DataValidation.AddListDataValidation();
                    foreach (var type in listLoai)
                    {
                        loai.Formula.Values.Add(type.Value);
                    }
                    var loaiTaiKhoan = sheet.Cells[nRowBegin, 10, rowIdx, 10].DataValidation.AddListDataValidation();
                    loaiTaiKhoan.Formula.Values.Add("Hạch toán");
                    loaiTaiKhoan.Formula.Values.Add("Nội bộ");

                    if (rowIdx >= nRowBegin)
                    {
                        sheet.Cells[nRowBegin, 5, rowIdx, 8].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                        sheet.Cells[nRowBegin, 1, rowIdx, 11].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 1, rowIdx, 11].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 1, rowIdx, 11].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        sheet.Cells[nRowBegin, 1, rowIdx, 11].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                        sheet.Cells[nRowBegin, 1, rowIdx, 11].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 1, rowIdx, 11].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 1, rowIdx, 11].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        sheet.Cells[nRowBegin, 1, rowIdx, 11].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }
                    package.SaveAs(stream);

                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    using (FileStream fs = new FileStream(_pathSave, FileMode.Create))
                    {
                        package.SaveAs(fs);
                    }
                }
            }
        }
        return Ok(new BaseResponseModel
        {
            Data = _fileMapServer
        });
    }

    [HttpPost("ImportFromExcelTaiKhoanArising")]
    public async Task<IActionResult> ImportFromExcelTaiKhoanArising([FromBody] List<ChartOfAccount> data, [FromHeader] int yearFilter)
    {
        await _chartOfAccountService.ImportFromExcelTaiKhoanArising(data, yearFilter);
        return Ok(new ObjectReturn
        {
            message = ResultErrorConstants.SUCCESS,
            status = 200,
        });
    }

    [HttpGet("ExportTaiKhoanNoiBo")]
    public async Task<IActionResult> Export([FromHeader] int yearFilter, int Loai)// 0:HachToan; 1:NoiBo
    {
        if (Loai == 3)
            Loai = 1;
        else if (Loai == 2)
            Loai = 0;

        string _fileMapServer = $"DanhSachTaiKhoan_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx",
                folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\EXCEL"),
                _pathSave = Path.Combine(folder, _fileMapServer);

        var datas = _chartOfAccountService.ExportGetAllAccounts(yearFilter);
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Excel\\danhsachtaikhoan_template.xlsx");
        using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                int nRowBegin = 4;
                int rowIdx = nRowBegin;
                var listTinhChat = await _chartOfAccountService.GetLookupValues("COA_ACC_GROUP");
                var listLoai = await _chartOfAccountService.GetLookupValues("COA_CLASSIFICATION");
                if (datas.Count > 0)
                {
                    foreach (ChartOfAccountModel item in datas)
                    {
                        sheet.Cells.Style.WrapText = true;
                        sheet.Cells[rowIdx, 1].Value = item.Code;
                        sheet.Cells[rowIdx, 2].Value = item.Name;
                        if (Loai == 0)
                        {
                            sheet.Cells[rowIdx, 3].Value = item.OpeningDebit;
                            sheet.Cells[rowIdx, 4].Value = item.OpeningCredit;
                            sheet.Cells[rowIdx, 5].Value = item.OpeningForeignDebit;
                            sheet.Cells[rowIdx, 6].Value = item.OpeningForeignCredit;
                            sheet.Cells[rowIdx, 9].Value = "Hạch toán";
                        }
                        else
                        {
                            sheet.Cells[rowIdx, 3].Value = item.OpeningDebitNB;
                            sheet.Cells[rowIdx, 4].Value = item.OpeningCreditNB;
                            sheet.Cells[rowIdx, 5].Value = item.OpeningForeignDebitNB;
                            sheet.Cells[rowIdx, 6].Value = item.OpeningForeignCreditNB;
                            sheet.Cells[rowIdx, 9].Value = "Nội bộ";
                        }

                        var accGroup = listTinhChat.Find(x => x.Code == item.AccGroup.ToString());
                        if (accGroup != null)
                            sheet.Cells[rowIdx, 7].Value = accGroup.Value;
                        var tinhChat = sheet.Cells[rowIdx, 7].DataValidation.AddListDataValidation();
                        foreach (var type in listTinhChat)
                        {
                            tinhChat.Formula.Values.Add(type.Value);
                        }
                        var classifica = listLoai.Find(x => x.Code == item.Classification.ToString());
                        if (classifica != null)
                            sheet.Cells[rowIdx, 8].Value = classifica.Value;
                        var loai = sheet.Cells[rowIdx, 8].DataValidation.AddListDataValidation();
                        foreach (var type in listLoai)
                        {
                            loai.Formula.Values.Add(type.Value);
                        }
                        var loaiTaiKhoan = sheet.Cells[rowIdx, 9].DataValidation.AddListDataValidation();
                        loaiTaiKhoan.Formula.Values.Add("Hạch toán");
                        loaiTaiKhoan.Formula.Values.Add("Nội bộ");
                        rowIdx++;
                    }
                }
                else
                {
                    if (listTinhChat.Count > 0)
                    {
                        sheet.Cells[rowIdx, 7].Value = listTinhChat[0].Value;
                        var tinhChat = sheet.Cells[rowIdx, 7].DataValidation.AddListDataValidation();
                        foreach (var type in listTinhChat)
                        {
                            tinhChat.Formula.Values.Add(type.Value);
                        }
                    }
                    if (listLoai.Count > 0)
                    {
                        sheet.Cells[rowIdx, 8].Value = listLoai[0].Value;
                        var loai = sheet.Cells[rowIdx, 8].DataValidation.AddListDataValidation();
                        foreach (var type in listLoai)
                        {
                            loai.Formula.Values.Add(type.Value);
                        }
                    }

                    sheet.Cells[rowIdx, 9].Value = "Hạch toán";
                    var loaiTaiKhoan = sheet.Cells[rowIdx, 9].DataValidation.AddListDataValidation();
                    loaiTaiKhoan.Formula.Values.Add("Hạch toán");
                    loaiTaiKhoan.Formula.Values.Add("Nội bộ");
                    rowIdx++;
                }
                rowIdx--;
                if (rowIdx >= nRowBegin)
                {
                    sheet.Cells[nRowBegin, 4, rowIdx, 6].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                    sheet.Cells[nRowBegin, 1, rowIdx, 9].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 9].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 9].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 9].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                    sheet.Cells[nRowBegin, 1, rowIdx, 9].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 9].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 9].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 9].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
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

    [HttpGet("ExportTaiKhoanNoiBoChiTiet1")]
    public async Task<IActionResult> ExportChiTiet1([FromHeader] int yearFilter, 
        string code, int Loai, bool isExportAll,
        string warehouseCode)//0:hachToan. 1:NB
    {
        if (Loai == 3)
            Loai = 1;
        else if (Loai == 2)
            Loai = 0;

        string _fileMapServer = $"DanhSachTaiKhoan_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx",
               folder = Path.Combine(Directory.GetCurrentDirectory(), @"ExportHistory\\EXCEL"),
               _pathSave = Path.Combine(folder, _fileMapServer);

        var datas = await _chartOfAccountService.ExportAccountChiTiet1(code, isExportAll, warehouseCode, yearFilter);
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads\\Excel\\danhsachchitiet_template.xlsx");
        using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                int nRowBegin = 4;
                int rowIdx = nRowBegin;
                var listTinhChat = await _chartOfAccountService.GetLookupValues("COA_ACC_GROUP");
                var listLoai = await _chartOfAccountService.GetLookupValues("COA_CLASSIFICATION");
                var listWarehouse = _chartOfAccountService.GetListWarehouse();
                sheet.Cells.Style.WrapText = true;

                if (datas.Count > 0)
                {
                    var accountParent = datas.Where(x => x.ParentRef == code).ToList();
                    foreach (ChartOfAccountModel item in accountParent)
                    {
                        var warehouse = listWarehouse.Find(x => x.Code == item.WarehouseCode);
                        if (warehouse != null)
                            sheet.Cells[rowIdx, 1].Value = warehouse.Code;

                        sheet.Cells[rowIdx, 3].Value = item.Code;
                        sheet.Cells[rowIdx, 4].Value = item.Name;
                        sheet.Cells[rowIdx, 5].Value = item.StockUnit;
                        if (Loai == 0)
                        {
                            sheet.Cells[rowIdx, 6].Value = item.OpeningStockQuantity;
                            sheet.Cells[rowIdx, 7].Value = item.StockUnitPrice;
                            sheet.Cells[rowIdx, 8].Value = item.OpeningDebit;
                            sheet.Cells[rowIdx, 9].Value = item.OpeningCredit;
                            sheet.Cells[rowIdx, 10].Value = item.OpeningForeignDebit;
                            sheet.Cells[rowIdx, 11].Value = item.OpeningForeignCredit;
                            sheet.Cells[rowIdx, 14].Value = "Hạch toán";
                        }
                        else
                        {
                            sheet.Cells[rowIdx, 6].Value = item.OpeningStockQuantityNB;
                            sheet.Cells[rowIdx, 7].Value = item.StockUnitPriceNB;
                            sheet.Cells[rowIdx, 8].Value = item.OpeningDebitNB;
                            sheet.Cells[rowIdx, 9].Value = item.OpeningCreditNB;
                            sheet.Cells[rowIdx, 10].Value = item.OpeningForeignDebitNB;
                            sheet.Cells[rowIdx, 11].Value = item.OpeningForeignCreditNB;
                            sheet.Cells[rowIdx, 14].Value = "Nội bộ";
                        }
                        var isInternal = "Cả hai";
                        if(item.IsInternal == 2)
                            isInternal = "HT";
                        else if (item.IsInternal == 2)
                            isInternal = "NB";
                        sheet.Cells[rowIdx, 15].Value = isInternal;

                        var accGroup = listTinhChat.Find(x => x.Code == item.AccGroup.ToString());
                        if (accGroup != null)
                            sheet.Cells[rowIdx, 12].Value = accGroup.Value;

                        var classifica = listLoai.Find(x => x.Code == item.Classification.ToString());
                        if (classifica != null)
                            sheet.Cells[rowIdx, 13].Value = classifica.Value;

                        rowIdx++;
                        var accountChild = datas.Where(x => x.ParentRef == (code+":"+ item.Code) && (string.IsNullOrEmpty(item.WarehouseCode) || x.WarehouseCode == item.WarehouseCode)).ToList();
                        foreach (ChartOfAccountModel child in accountChild)
                        {
                            var warehousechild = listWarehouse.Find(x => x.Code == child.WarehouseCode);
                            if (warehousechild != null)
                                sheet.Cells[rowIdx, 1].Value = warehousechild.Code;

                            if (isExportAll)
                            {
                                sheet.Cells[rowIdx, 2].Value = child.ParentRef.Split(":")[1];
                            }

                            sheet.Cells[rowIdx, 3].Value = child.Code;
                            sheet.Cells[rowIdx, 4].Value = child.Name;
                            sheet.Cells[rowIdx, 5].Value = child.StockUnit;
                            if (Loai == 0)
                            {
                                sheet.Cells[rowIdx, 6].Value = child.OpeningStockQuantity;
                                sheet.Cells[rowIdx, 7].Value = child.StockUnitPrice;
                                sheet.Cells[rowIdx, 8].Value = child.OpeningDebit;
                                sheet.Cells[rowIdx, 9].Value = child.OpeningCredit;
                                sheet.Cells[rowIdx, 10].Value = child.OpeningForeignDebit;
                                sheet.Cells[rowIdx, 11].Value = child.OpeningForeignCredit;
                                sheet.Cells[rowIdx, 14].Value = "Hạch toán";
                            }
                            else
                            {
                                sheet.Cells[rowIdx, 6].Value = child.OpeningStockQuantityNB;
                                sheet.Cells[rowIdx, 7].Value = child.StockUnitPriceNB;
                                sheet.Cells[rowIdx, 8].Value = child.OpeningDebitNB;
                                sheet.Cells[rowIdx, 9].Value = child.OpeningCreditNB;
                                sheet.Cells[rowIdx, 10].Value = child.OpeningForeignDebitNB;
                                sheet.Cells[rowIdx, 11].Value = child.OpeningForeignCreditNB;
                                sheet.Cells[rowIdx, 14].Value = "Nội bộ";
                            }
                            sheet.Cells[rowIdx, 15].Value = "Cả hai";

                            var accGroupchild = listTinhChat.Find(x => x.Code == child.AccGroup.ToString());
                            if (accGroupchild != null)
                                sheet.Cells[rowIdx, 12].Value = accGroupchild.Value;

                            var classificachild = listLoai.Find(x => x.Code == child.Classification.ToString());
                            if (classificachild != null)
                                sheet.Cells[rowIdx, 13].Value = classificachild.Value;

                            rowIdx++;
                        }
                           
                    }
                    if (listWarehouse.Any())
                    {
                        var warehouseE = sheet.Cells[nRowBegin, 1, rowIdx, 1].DataValidation.AddListDataValidation();
                        foreach (var itemFor in listWarehouse)
                        {
                            warehouseE.Formula.Values.Add(itemFor.Code);
                        }
                    }
                    if (listTinhChat.Any())
                    {
                        var tinhChat = sheet.Cells[nRowBegin, 12, rowIdx, 12].DataValidation.AddListDataValidation();
                        foreach (var itemFor in listTinhChat)
                        {
                            tinhChat.Formula.Values.Add(itemFor.Value);
                        }
                    }
                    if (listLoai.Any())
                    {
                        var loai = sheet.Cells[nRowBegin, 13, rowIdx, 13].DataValidation.AddListDataValidation();
                        foreach (var itemFor in listLoai)
                        {
                            loai.Formula.Values.Add(itemFor.Value);
                        }
                    }
                    var loaiTaiKhoan = sheet.Cells[nRowBegin, 14, rowIdx, 14].DataValidation.AddListDataValidation();
                    loaiTaiKhoan.Formula.Values.Add("Hạch toán");
                    loaiTaiKhoan.Formula.Values.Add("Nội bộ");
                }
                else
                {
                    if (listTinhChat.Any())
                    {
                        sheet.Cells[rowIdx, 12].Value = listTinhChat[0].Value;
                        var tinhChat = sheet.Cells[rowIdx, 12].DataValidation.AddListDataValidation();
                        foreach (var itemFor in listTinhChat)
                        {
                            tinhChat.Formula.Values.Add(itemFor.Value);
                        }
                    }
                    if (listLoai.Any())
                    {
                        sheet.Cells[rowIdx, 13].Value = listLoai[0].Value;
                        var loai = sheet.Cells[rowIdx, 13].DataValidation.AddListDataValidation();
                        foreach (var itemFor in listLoai)
                        {
                            loai.Formula.Values.Add(itemFor.Value);
                        }
                    }
                    sheet.Cells[rowIdx, 14].Value = "Hạch toán";
                    var loaiTaiKhoan = sheet.Cells[rowIdx, 14].DataValidation.AddListDataValidation();
                    loaiTaiKhoan.Formula.Values.Add("Hạch toán");
                    loaiTaiKhoan.Formula.Values.Add("Nội bộ");
                    rowIdx++;
                }

                var isInternals = sheet.Cells[nRowBegin, 15, rowIdx, 15].DataValidation.AddListDataValidation();
                isInternals.Formula.Values.Add("Cả hai");
                isInternals.Formula.Values.Add("HT");
                isInternals.Formula.Values.Add("NB");

                rowIdx--;
                if (rowIdx >= nRowBegin)
                {
                    sheet.Cells[nRowBegin, 6, rowIdx, 11].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                    sheet.Cells[nRowBegin, 1, rowIdx, 15].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 15].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 15].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 15].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                    sheet.Cells[nRowBegin, 1, rowIdx, 15].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 15].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 15].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 15].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
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

    [HttpPost("importTaiKhoan")]
    public async Task<IActionResult> Import([FromBody] List<ChartOfAccount> data, [FromHeader] int yearFilter)
    {
        try
        {
            string errorMessage = await _chartOfAccountService.ImportFromExcel(data, yearFilter);
            if (string.IsNullOrEmpty(errorMessage))
                return Ok(new ObjectReturn
                {
                    message = ResultErrorConstants.SUCCESS,
                    status = 200,
                });
            return Ok(new ObjectReturn
            {
                message = errorMessage,
                status = 400,
            });
        }
        catch (Exception ex)
        {
            return Ok(new ObjectReturn
            {
                message = ex.Message.ToString(),
                status = 400,
            });
        }
    }

    [HttpPost("importTaiKhoanCT1/{parentCode}")]
    public async Task<IActionResult> importTaiKhoanCT1([FromBody] List<ChartOfAccountImportModel> data, [FromHeader] int yearFilter, string parentCode)
    {
        string errorMessage = await _chartOfAccountService.ImportFromExcelCT1(data, parentCode, yearFilter);

        if (string.IsNullOrEmpty(errorMessage))
            return Ok(new ObjectReturn
            {
                message = ResultErrorConstants.SUCCESS,
                status = 200,
            });
        return Ok(new ObjectReturn
        {
            message = errorMessage,
            status = 400,
        });
    }

    [HttpGet("check-account-test/{code}")]
    public async Task<IActionResult> CheckAccountTest([FromHeader] int yearFilter, string code)
    {
        try
        {
            var result = await _chartOfAccountService.CheckAccountTest(code, yearFilter);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Ok(new ObjectReturn
            {
                message = ex.Message.ToString(),
                status = 400,
            });
        }
    }

    [HttpGet("selection-customer")]
    public async Task<IActionResult> GetAllAccountCustomer([FromHeader] int yearFilter)
    {
        var data = await _chartOfAccountService.GetAllAccountCustomer(yearFilter);
        return Ok(new ObjectReturn
        {
            data = data,
            status = 200,
        });
    }

    [HttpGet("get-chart-accounts")]
    public async Task<IActionResult> GetPage([FromHeader] int yearFilter, string? code = "")
    {
        PagingRequestModel param = new PagingRequestModel();
        param.SearchText = code;
        return Ok(await _chartOfAccountService.GetAllAccounts(param, yearFilter));
    }

    [HttpGet("get-chart-accounts-classification")]
    public async Task<IActionResult> GetPageByClassifications([FromQuery] List<int> classification, [FromHeader] int yearFilter)
    {
        if (classification == null || classification.Count == 0)
        {
            return BadRequest(new { msg = "Classification not found" });
        }
        return Ok(await _chartOfAccountService.GetAllAccountSelections(classification, yearFilter));
    }

    [HttpPost("groups")]
    public async Task<IActionResult> CreateAccountGroup([FromBody] ChartOfAccountGroup accountGroup, [FromHeader] int yearFilter)
    {
        var errorMessage = await _chartOfAccountService.CreateAccountGroup(accountGroup, yearFilter);
        if (string.IsNullOrEmpty(errorMessage))
            return Ok(accountGroup);
        return BadRequest(new { msg = errorMessage });
    }

    [HttpGet("groups")]
    public async Task<IActionResult> GetAllGroup([FromHeader] int yearFilter)
    {
        return Ok(await _chartOfAccountService.GetAllGroups(yearFilter));
    }

    [HttpPut("groups")]
    public async Task<IActionResult> UpdateGroupDetails([FromBody] ChartOfAccountGroupModel group, [FromHeader] int yearFilter)
    {
        var errorMessage = await _chartOfAccountService.UpdateGroupDetails(group, yearFilter);

        if (string.IsNullOrEmpty(errorMessage))
            return Ok();

        return Ok(new ObjectReturn
        {
            message = errorMessage,
            status = 400,
        });
    }

    [HttpDelete("groups/{codeChartOfAccountGroup}")]
    public async Task<IActionResult> DeleteGroup([FromHeader] int yearFilter, string codeChartOfAccountGroup)
    {
        var errorMessage = await _chartOfAccountService.DeleteGroup(codeChartOfAccountGroup, yearFilter);
        if (string.IsNullOrEmpty(errorMessage))
            return Ok();
        return BadRequest(new { msg = errorMessage });
    }

    [HttpGet("UpdateArisingAccount")]
    public async Task<IActionResult> UpdateArisingAccount([FromHeader] int yearFilter)
    {
        var dbName = _connectionStringProvider.GetDbName();
        var data = await _chartOfAccountService.UpdateArisingAccount(yearFilter, dbName);
        return Ok(new BaseResponseModel()
        {
            Data = data,
        });
    }

    [HttpGet("code-auto")]
    public async Task<IActionResult> GetCodeAuto([FromHeader] int yearFilter, string parentRef, int isInternal = 1)
    {
        var data = await _chartOfAccountService.GetCodeAuto(parentRef, isInternal, yearFilter);
        return Ok(new BaseResponseModel()
        {
            Data = data,
        });
    }

    [HttpGet("transfer-account")]
    public async Task<IActionResult> TransferAccount([FromHeader] int yearFilter, int year)
    {
        if (year < 1)
        {
            year = yearFilter + 1;
        }
        await _chartOfAccountService.TransferAccount(yearFilter, year);
        return Ok();
    }
}