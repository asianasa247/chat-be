using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Helpers;

namespace ManageEmployee.Services;

using AutoMapper;
using Common.Constants;
using Common.Helpers;
using Hangfire;
using ManageEmployee.DataTransferObject.AriseModels;
using ManageEmployee.DataTransferObject.LedgerWarehouseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.DocumentEntities;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.OrderEntities;
using ManageEmployee.Queues;
using ManageEmployee.Services.Interfaces.Documents;
using ManageEmployee.Services.Interfaces.Excels;
using ManageEmployee.Services.Interfaces.Ledgers;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

/// <summary>
/// Defines the <see cref="ManagementAriesExcelService" />.
/// </summary>
public class ManagementAriesExcelService : IManagementAriesExcelService
{
    /// <summary>
    /// Defines the _context.
    /// </summary>
    private readonly ApplicationDbContext _context;

    private readonly IMapper _mapper;

    /// <summary>
    /// Defines the _companyService.
    /// </summary>

    private readonly ILedgerService _ledgerService;
    private readonly IDocumentService _documentService;
    private readonly ILedgerImportErrorQueue _ledgerImportErrorQueue;
    private readonly ILedgerUpdateChartOfAccountNameService _ledgerUpdateChartOfAccountNameService;

    public ManagementAriesExcelService(            
        IMapper mapper,
        ApplicationDbContext context,
        ILedgerService ledgerService,
        IDocumentService documentService,
         ILedgerImportErrorQueue ledgerImportErrorQueue,
        ILedgerUpdateChartOfAccountNameService ledgerUpdateChartOfAccountNameService
    )
    {
        _context = context;
        _ledgerService = ledgerService;
        _documentService = documentService;
        _mapper = mapper;
        _ledgerImportErrorQueue = ledgerImportErrorQueue;
        _ledgerUpdateChartOfAccountNameService = ledgerUpdateChartOfAccountNameService;
    }

    /// <summary>
    /// chuyển đổi
    /// </summary>
    /// <param name="request"></param>
    public async Task TransferInfoLedger(TransferModelRequest request, int year)
    {
        List<Ledger> ledgers = await _context.GetLedger(year, request.FromTypeData).Where(x => !x.IsDelete && request.LedgerIds.Contains(x.Id)).ToListAsync();
        if (!ledgers.Any())
        {
            return;
        }

        if (request.Month > 0)
        {
            await TranferData(request, year);

            if (request.TypeData == LedgerInternalConst.LedgerTotal)
            {
                request.TypeData = 3;
                await TranferData(request, year);
            }
        }
        if (request.isDeleteData)
        {
            _context.Ledgers.RemoveRange(ledgers);
        }
        await _context.SaveChangesAsync();

    }

    private async Task TranferData(TransferModelRequest request, int year)
    {
        int orderMax = 0;
        List<Ledger> ledgers = await _context.GetLedger(year, request.FromTypeData).Where(x => !x.IsDelete && request.LedgerIds.Contains(x.Id)).ToListAsync();
        var ledgerOrder = await _context.GetLedger(year, request.TypeData).Where(x => x.Month == request.Month && x.Type == request.DocumentType).OrderByDescending(x => x.Order).FirstOrDefaultAsync();
        if (ledgerOrder != null)
        {
            orderMax = ledgerOrder.Order;
        }

        List<Ledger> ledgerTranferOuts = new List<Ledger>();
        string monthStr = (request.Month < 10 ? "0" + request.Month : request.Month.ToString());

        var orders = ledgers.GroupBy(x => x.Order).Select(x => new { Order = x.Key, Ledger = x.ToList() });
        foreach (var order in orders)
        {
            orderMax++;
            var orderString = orderMax < 10 ? $"00{orderMax}" :
            orderMax < 100 ? $"0{orderMax}" : orderMax.ToString();

            var ledgerTranfers = order.Ledger.ConvertAll(x =>
            {
                int days = DateTime.DaysInMonth(x.OrginalBookDate.Value.Year, request.Month);
                if (x.OrginalBookDate.Value.Day <= days)
                {
                    days = x.OrginalBookDate.Value.Day;
                }
                x.Id = 0;
                x.Type = request.DocumentType;
                x.Month = request.Month;
                x.VoucherNumber = monthStr + "/" + request.DocumentType;
                x.OrginalBookDate = new DateTime(x.OrginalBookDate.Value.Year, request.Month, days);
                x.BookDate = new DateTime(x.OrginalBookDate.Value.Year, request.Month, days);
                x.ReferenceBookDate = new DateTime(x.OrginalBookDate.Value.Year, request.Month, days);
                x.InvoiceDate = new DateTime(x.OrginalBookDate.Value.Year, request.Month, days);
                x.Year = year;
                x.Order = orderMax;
                x.OrginalVoucherNumber = request.DocumentType + monthStr + "-" + year.ToString().Substring(2, 2) + "-" + orderString;
                x.IsInternal = request.TypeData;
                return x;
            });
            ledgerTranferOuts.AddRange(ledgerTranfers);
        }

        _context.Ledgers.AddRange(ledgerTranferOuts);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Lấy Ledger cuối cùng của tháng.
    /// </summary>
    /// <param name="request">.</param>
    /// <returns>.</returns>
    public Ledger GetLastVoucherNumberInMonth(AriesExcelSearchModel request, int year)
    {
        var ledgers = _context.GetLedger(year, request.IsInternal).Where(x => x.Month == request.Month)
                                      .OrderBy(x => x.Order)
                                      .ToList();
        if (ledgers.Count > 0)
            return ledgers.Last();
        else
            return new Ledger();
    }


    public async Task UpdateOrginalVoucherNumber(AriseUpdateOrginalVoucherRequest request, int year)
    {
        List<Ledger> ledgers;
        if (request.StartDate != null && request.EndDate != null)
        {
            var startMonth = ((DateTime)request.StartDate).Month;
            var endMonth = ((DateTime)request.EndDate).Month;
            ledgers = await _context.GetLedger(year, request.IsInternal).Where(x => !x.IsDelete &&
                                             (x.Month >= startMonth && x.Month <= endMonth) &&
                                             (string.IsNullOrEmpty(request.Type) || x.Type == request.Type) &&
                                             (request.Month == null || request.Month == 0 || x.Month == request.Month))
                                      .OrderBy(x => x.OrginalBookDate).ToListAsync();
        }
        else
        {
            ledgers = await _context.GetLedger(year, request.IsInternal).Where(x => !x.IsDelete &&
                                             (string.IsNullOrEmpty(request.Type) || x.Type == request.Type) &&
                                             (request.Month == null || request.Month == 0 || x.Month == request.Month))
                                      .OrderBy(x => x.OrginalBookDate).ToListAsync();
        }

        var ledgerUpdates = ledgers.OrderBy(x => x.OrginalBookDate.Value.Day).ToList();
        var days = ledgerUpdates.Select(x => x.OrginalBookDate.Value.Day).Distinct().OrderBy(x => x).ToList();
        var index = 1;
        foreach (var day in days)
        {
            var orginalVoucherNumbers = ledgerUpdates.Where(x => x.OrginalBookDate.Value.Day == day).Select(x => x.OrginalVoucherNumber).Distinct().OrderBy(x => x).ToList();

            foreach (var orginalVoucherNumber in orginalVoucherNumbers)
            {

                var itemUpdates = ledgerUpdates.Where(x => x.OrginalBookDate.Value.Day == day && x.OrginalVoucherNumber == orginalVoucherNumber).ToList();
                foreach (var ledger in itemUpdates)
                {
                    ledger.Order = index;
                }
                index++;
            }
        }

        foreach (var ledger in ledgers)
        {
            var orderString = ledger.Order < 10 ? $"00{ledger.Order}" :
                            ledger.Order < 100 ? $"0{ledger.Order}" : ledger.Order.ToString();
            var monthStr = request.Month < 10 ? "0" + request.Month : request.Month.ToString();
            ledger.OrginalVoucherNumber = $"{ledger.Type}{monthStr}-{year.ToString().Substring(2, 2)}-{orderString}";

        }
        _context.Ledgers.UpdateRange(ledgers);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Xuất file excel.
    /// </summary>
    /// <param name="search">.</param>
    /// <returns>.</returns>
    public async Task<MemoryStream> ExportExcel(AriesExcelSearchModel search, int year)
    {
        var searchQuery = string.IsNullOrWhiteSpace(search.SearchText) ? "" : search.SearchText.Trim();
        var data = new List<LedgerExport>();

                data = await _context.GetLedger(year, search.IsInternal)
                               .Where(l => (string.IsNullOrEmpty(search.DocumentType) || (!string.IsNullOrEmpty(search.DocumentType) && l.Type == search.DocumentType)) &&
                                           ( search.FilterMonth == 0 || (search.FilterMonth > 0 && l.Month == search.FilterMonth)))
                               .Where(x => x.OrginalVoucherNumber.Contains(searchQuery))
                               .OrderByDescending(s => s.Order).ThenByDescending(s => s.OrginalVoucherNumber)
                               .Select(x => _mapper.Map<LedgerExport>(x))
                               .ToListAsync();

        string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/AriesExcel_Template.xlsx");
        MemoryStream stream = new MemoryStream();
        using (FileStream templateDocumentStream = File.OpenRead(path))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                sheet.DefaultColWidth = 10.0;
                int nRowBegin = 3, rowIdx = 3;
                List<Document> listDocument = _documentService.GetAll().ToList();
                List<TaxRate> taxRates = _context.TaxRates.ToList();
                for (int i = 0; i < data.Count; i++)
                {
                    sheet.Cells[rowIdx, 1].Value = data[i].Type;
                    sheet.Cells[rowIdx, 2].Value = data[i].Month;
                    sheet.Cells[rowIdx, 3].Value = data[i].BookDate.HasValue ? data[i].BookDate.Value.ToString("dd/MM/yyyy") : "";
                    sheet.Cells[rowIdx, 4].Value = data[i].OrginalVoucherNumber;
                    sheet.Cells[rowIdx, 5].Value = data[i].OrginalBookDate.HasValue ? data[i].OrginalBookDate.Value.ToString("dd/MM/yyyy") : "";
                    sheet.Cells[rowIdx, 6].Value = data[i].OrginalDescription;
                    sheet.Cells[rowIdx, 7].Value = data[i].OrginalCompanyName;
                    sheet.Cells[rowIdx, 8].Value = data[i].OrginalAddress;
                    sheet.Cells[rowIdx, 9].Value = data[i].AttachVoucher;
                    sheet.Cells[rowIdx, 10].Value = data[i].ReferenceVoucherNumber;
                    sheet.Cells[rowIdx, 11].Value = data[i].ReferenceBookDate.HasValue ? data[i].ReferenceBookDate.Value.ToString("dd/MM/yyyy") : "";
                    sheet.Cells[rowIdx, 12].Value = data[i].ReferenceFullName;
                    sheet.Cells[rowIdx, 13].Value = data[i].ReferenceAddress;

                    sheet.Cells[rowIdx, 14].Value = data[i].InvoiceCode;
                    sheet.Cells[rowIdx, 15].Value = data[i].InvoiceAdditionalDeclarationCode;

                    sheet.Cells[rowIdx, 16].Value = data[i].InvoiceNumber;
                    sheet.Cells[rowIdx, 17].Value = data[i].InvoiceTaxCode;
                    sheet.Cells[rowIdx, 18].Value = data[i].InvoiceAddress;
                    sheet.Cells[rowIdx, 19].Value = data[i].InvoiceSerial;
                    sheet.Cells[rowIdx, 20].Value = data[i].InvoiceDate.HasValue ? data[i].InvoiceDate.Value.ToString("dd/MM/yyyy") : "";
                    sheet.Cells[rowIdx, 21].Value = data[i].InvoiceName;
                    sheet.Cells[rowIdx, 22].Value = data[i].InvoiceProductItem;
                    sheet.Cells[rowIdx, 23].Value = data[i].DebitCode;
                    sheet.Cells[rowIdx, 24].Value = data[i].DebitCodeName;

                    sheet.Cells[rowIdx, 25].Value = data[i].DebitWarehouse;
                    sheet.Cells[rowIdx, 26].Value = data[i].DebitWarehouseName;

                    sheet.Cells[rowIdx, 27].Value = data[i].DebitDetailCodeFirst;
                    sheet.Cells[rowIdx, 28].Value = data[i].DebitDetailCodeFirstName;

                    sheet.Cells[rowIdx, 29].Value = data[i].DebitDetailCodeSecond;
                    sheet.Cells[rowIdx, 30].Value = data[i].DebitDetailCodeSecondName;

                    sheet.Cells[rowIdx, 31].Value = data[i].CreditCode;
                    sheet.Cells[rowIdx, 32].Value = data[i].CreditCodeName;

                    sheet.Cells[rowIdx, 33].Value = data[i].CreditWarehouse;
                    sheet.Cells[rowIdx, 34].Value = data[i].CreditWarehouseName;

                    sheet.Cells[rowIdx, 35].Value = data[i].CreditDetailCodeFirst;
                    sheet.Cells[rowIdx, 36].Value = data[i].CreditDetailCodeFirstName;

                    sheet.Cells[rowIdx, 37].Value = data[i].CreditDetailCodeSecond;
                    sheet.Cells[rowIdx, 38].Value = data[i].CreditDetailCodeSecondName;

                    sheet.Cells[rowIdx, 39].Value = data[i].ProjectCode;
                    sheet.Cells[rowIdx, 40].Value = data[i].DepreciaMonth;
                    sheet.Cells[rowIdx, 41].Value = data[i].Quantity;
                    sheet.Cells[rowIdx, 42].Value = data[i].UnitPrice;
                    sheet.Cells[rowIdx, 43].Value = data[i].OrginalCurrency;
                    sheet.Cells[rowIdx, 44].Value = data[i].ExchangeRate;
                    sheet.Cells[rowIdx, 45].Value = data[i].Amount;

                    sheet.Cells[rowIdx, 46].Value = search.IsInternal == 3 ? "3. Nội bộ" :
                                                    (search.IsInternal == 2 ? "2. Hạch toán" : "1. Cả hai");
                    sheet.Cells[rowIdx, 47].Value = data[i].Id;

                    rowIdx++;
                }
                rowIdx--;

                var listInternal = sheet.Cells[nRowBegin, 46, rowIdx, 46].DataValidation.AddListDataValidation();
                {
                    listInternal.Formula.Values.Add("1. Cả hai");
                    listInternal.Formula.Values.Add("2. Hạch toán");
                    listInternal.Formula.Values.Add("3. Nội bộ");
                }

                var listType = sheet.Cells[nRowBegin, 1, rowIdx, 1].DataValidation.AddListDataValidation();
                foreach (var type in listDocument)
                {
                    listType.Formula.Values.Add(type.Code);
                }
                var listThang = sheet.Cells[nRowBegin, 2, rowIdx, 2].DataValidation.AddListDataValidation();
                for (int index = 1; index <= 12; index++)
                {
                    listThang.Formula.Values.Add(index.ToString());
                }

                var listInvoiceCode = sheet.Cells[nRowBegin, 14, rowIdx, 14].DataValidation.AddListDataValidation();
                foreach (var invoiceCode in taxRates)
                {
                    listInvoiceCode.Formula.Values.Add(invoiceCode.Code);
                }
                var invoiceDeclaration = sheet.Cells[nRowBegin, 15, rowIdx, 15].DataValidation.AddListDataValidation();
                {
                    invoiceDeclaration.Formula.Values.Add("BT");
                    invoiceDeclaration.Formula.Values.Add("HB");
                }

                if (rowIdx >= nRowBegin)
                {
                    sheet.Cells[nRowBegin, 41, rowIdx, 45].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                    sheet.Cells[nRowBegin, 1, rowIdx, 47].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 47].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 47].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                    sheet.Cells[nRowBegin, 1, rowIdx, 47].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                    sheet.Cells[nRowBegin, 1, rowIdx, 47].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 47].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 47].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    sheet.Cells[nRowBegin, 1, rowIdx, 47].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                }
                package.SaveAs(stream);
            }
        }
        stream.Seek(0L, SeekOrigin.Begin);
        return stream;
    }

    /// <summary>
    /// Import duwx liệu từ file excell.
    /// </summary>
    /// <param name="request">.</param>
    public async Task<string> ImportExcel(List<LedgerExport> request, int year)
    {
        int i = 0;
        try
        {
            var data = request.GroupBy(grp => new { grp.OrginalBookDate.Value.Year, grp.OrginalBookDate.Value.Month }).Select(s =>
                new AriseExcelImportModel()
                {
                    Year = s.Key.Year,
                    Month = s.Key.Month,
                    Ledgers = s.Select(x => x).OrderBy(x => x.OrginalBookDate).ToList()
                }
            ).ToList();

            if(data.Any(x => x.Year != year))
            {
                throw new Exception("Năm của ngảy ghi sổ đang khác với năm bạn đang làm việc");
            }

            var dataImports = new List<Ledger>();
            var ledgerIds = request.Select(x => x.Id);
            var ledgers = await _context.Ledgers.Where(x => ledgerIds.Contains(x.Id)).ToListAsync();
            foreach (var item in data)
            {
                foreach (var ledger in item.Ledgers)
                {
                    i++;
                    int maxOriginalVoucher = int.Parse(ledger.OrginalVoucherNumber.Split("-").Last());

                    string months = (item.Month < 10 ? ("0" + item.Month) : item.Month.ToString());
                    Ledger newLedger = ledgers.Find(x => x.Id == ledger.Id);
                    
                    if (newLedger == null)
                    {
                        newLedger = _mapper.Map<Ledger>(ledger);
                        newLedger.Id = 0;
                        newLedger.Order = maxOriginalVoucher;
                        newLedger.VoucherNumber = months + "/" + ledger.Type;
                    }
                    else
                    {
                        newLedger.BookDate = ledger.BookDate;
                        newLedger.OrginalVoucherNumber = ledger.OrginalVoucherNumber;
                        newLedger.OrginalBookDate = ledger.OrginalBookDate;
                        newLedger.OrginalDescription = ledger.OrginalDescription;
                        newLedger.OrginalCompanyName = ledger.OrginalCompanyName;
                        newLedger.OrginalAddress = ledger.OrginalAddress;
                        newLedger.AttachVoucher = ledger.AttachVoucher;
                        newLedger.ReferenceVoucherNumber = ledger.ReferenceVoucherNumber;
                        newLedger.ReferenceBookDate = ledger.ReferenceBookDate;
                        newLedger.ReferenceFullName = ledger.ReferenceFullName;
                        newLedger.ReferenceAddress = ledger.ReferenceAddress;

                        newLedger.InvoiceCode = ledger.InvoiceCode;
                        newLedger.InvoiceAdditionalDeclarationCode = ledger.InvoiceAdditionalDeclarationCode;

                        newLedger.InvoiceNumber = ledger.InvoiceNumber;
                        newLedger.InvoiceTaxCode = ledger.InvoiceTaxCode;
                        newLedger.InvoiceAddress = ledger.InvoiceAddress;
                        newLedger.InvoiceSerial = ledger.InvoiceSerial;
                        newLedger.InvoiceDate = ledger.InvoiceDate;
                        newLedger.InvoiceName = ledger.InvoiceName;
                        newLedger.InvoiceProductItem = ledger.InvoiceProductItem;
                        newLedger.DebitCode = ledger.DebitCode;
                        newLedger.DebitCodeName = ledger.DebitCodeName;

                        newLedger.DebitWarehouse = ledger.DebitWarehouse;
                        newLedger.DebitWarehouseName = ledger.DebitWarehouseName;

                        newLedger.DebitDetailCodeFirst = ledger.DebitDetailCodeFirst;
                        newLedger.DebitDetailCodeFirstName = ledger.DebitDetailCodeFirstName;

                        newLedger.DebitDetailCodeSecond = ledger.DebitDetailCodeSecond;
                        newLedger.DebitDetailCodeSecondName = ledger.DebitDetailCodeSecondName;

                        newLedger.CreditCode = ledger.CreditCode;
                        newLedger.CreditCodeName = ledger.CreditCodeName;

                        newLedger.CreditWarehouse = ledger.CreditWarehouse;
                        newLedger.CreditWarehouseName = ledger.CreditWarehouseName;

                        newLedger.CreditDetailCodeFirst = ledger.CreditDetailCodeFirst;
                        newLedger.CreditDetailCodeFirstName = ledger.CreditDetailCodeFirstName;

                        newLedger.CreditDetailCodeSecond = ledger.CreditDetailCodeSecond;
                        newLedger.CreditDetailCodeSecondName = ledger.CreditDetailCodeSecondName;

                        newLedger.ProjectCode = ledger.ProjectCode;
                        newLedger.DepreciaMonth = ledger.DepreciaMonth;
                        newLedger.Quantity = ledger.Quantity;
                        newLedger.UnitPrice = ledger.UnitPrice;
                        newLedger.OrginalCurrency = ledger.OrginalCurrency;
                        newLedger.ExchangeRate = ledger.ExchangeRate;
                        newLedger.Amount = ledger.Amount;
                        newLedger.Year = year;

                    }
                    newLedger.UpdateAt = DateTime.Now;

                    var checkPayer = await _context.Payers.FirstOrDefaultAsync(x => x.Name == ledger.OrginalCompanyName);
                    if (checkPayer == null)
                    {
                        var payer = new Payer
                        {
                            Name = ledger.OrginalCompanyName,
                            Address = ledger.OrginalAddress,
                            TaxCode = ledger.InvoiceTaxCode
                        };
                        _context.Payers.Add(payer);
                    }

                        if (newLedger.Id > 0)
                            _context.Ledgers.Update(newLedger);
                        else
                            _context.Ledgers.Add(newLedger);
                    newLedger = await _ledgerUpdateChartOfAccountNameService.UpdateChartOfAccountName(newLedger, year);

                    dataImports.Add(newLedger);
                }
            }
            await _context.Ledgers.AddRangeAsync(dataImports);
            await _context.SaveChangesAsync();

            // check data error when import
            BackgroundJob.Enqueue(() => _ledgerImportErrorQueue.Perform(dataImports, year));
            return "";
        }
        catch (Exception ex)
        {
            throw new ErrorException("Error " + ex.Message.ToString()+ " line " + i);
        }
    }

    /// <summary>
    /// Lay danh sach tai khoan
    /// </summary>
    /// <returns></returns>
    public async Task<List<ChartOfAccount>> GetDebitAndCreditAccount(int year)
    {
        return await _context.GetChartOfAccount(year).Where(x => x.Type == 5 || x.Type == 6).ToListAsync();
    }

    public async Task TransferInfoLedgerLuong(TransferModelRequest request, int year)
    {
        var ledgers = await _context.GetLedger(year, 2).AsNoTracking().Where(x => !x.IsDelete && request.LedgerIds.Count > 0 && request.LedgerIds.Contains(x.Id)).ToListAsync();
        var maxOriginalVoucher = 0;
        var ledgerExist = await _context.GetLedger(year, 2).Where(x => !x.IsDelete && x.Type == "PC").OrderByDescending(x => x.Order).FirstOrDefaultAsync();
        if (ledgerExist != null)
        {
            maxOriginalVoucher = Int32.Parse(ledgerExist.OrginalVoucherNumber.Split('-').Last());
        }
        foreach (var ledger in ledgers)
        {
            ledger.Type = "PC";
            maxOriginalVoucher++;
            var orderString = maxOriginalVoucher < 10 ? $"00{maxOriginalVoucher}" :
                        maxOriginalVoucher < 100 ? $"0{maxOriginalVoucher}" : maxOriginalVoucher.ToString();
            string months = (ledger.Month < 10 ? ("0" + ledger.Month) : ledger.Month.ToString());
            ledger.OrginalVoucherNumber = $"{ledger.Type}{months}-{year.ToString().Substring(2, 2)}-{orderString}";
            ledger.Order = maxOriginalVoucher;
            ledger.VoucherNumber = months + "/" + ledger.Type;

            ledger.Id = 0;
            ledger.OrginalDescription = $"Bảo hiểm Xã hội tháng {ledger.Month}";
            await _ledgerService.Create(ledger, year);
        }
    }

    public async Task ImportExcelLocal(int year)
    {
        string path_exc = @"D:\Sổ kế toán_20230302_103004.xlsx";

        using (FileStream templateDocumentStream = System.IO.File.OpenRead(path_exc))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
            {
                ExcelWorksheet sheet = package.Workbook.Worksheets["Sheet1"];
                int i = 3;
                using var transaction = await _context.Database.BeginTransactionAsync();
                while (sheet.Cells[i, 1].Value != null)
                {
                    Ledger pro = new Ledger();
                    pro.Type = ConvertCell.ConvertCellToString(sheet.Cells[i, 1].Value);
                    pro.Month = ConvertCell.ConvertCellToInt(sheet.Cells[i, 2].Value);
                    pro.BookDate = ConvertCell.ConvertCellToDatetime(sheet.Cells[i, 3].Value);
                    pro.OrginalVoucherNumber = ConvertCell.ConvertCellToString(sheet.Cells[i, 4].Value);
                    pro.OrginalBookDate = ConvertCell.ConvertCellToDatetime(sheet.Cells[i, 5].Value);
                    pro.OrginalDescription = ConvertCell.ConvertCellToString(sheet.Cells[i, 6].Value);
                    pro.OrginalCompanyName = ConvertCell.ConvertCellToString(sheet.Cells[i, 7].Value);
                    pro.OrginalAddress = ConvertCell.ConvertCellToString(sheet.Cells[i, 8].Value);
                    pro.AttachVoucher = ConvertCell.ConvertCellToString(sheet.Cells[i, 9].Value);
                    pro.ReferenceVoucherNumber = ConvertCell.ConvertCellToString(sheet.Cells[i, 10].Value);
                    pro.ReferenceBookDate = ConvertCell.ConvertCellToDatetime(sheet.Cells[i, 11].Value);
                    pro.ReferenceFullName = ConvertCell.ConvertCellToString(sheet.Cells[i, 12].Value);

                    pro.ReferenceAddress = ConvertCell.ConvertCellToString(sheet.Cells[i, 13].Value);
                    pro.InvoiceCode = ConvertCell.ConvertCellToString(sheet.Cells[i, 14].Value);
                    pro.InvoiceAdditionalDeclarationCode = ConvertCell.ConvertCellToString(sheet.Cells[i, 15].Value);
                    pro.InvoiceNumber = ConvertCell.ConvertCellToString(sheet.Cells[i, 16].Value);
                    pro.InvoiceTaxCode = ConvertCell.ConvertCellToString(sheet.Cells[i, 17].Value);
                    pro.InvoiceAddress = ConvertCell.ConvertCellToString(sheet.Cells[i, 18].Value);
                    pro.InvoiceSerial = ConvertCell.ConvertCellToString(sheet.Cells[i, 19].Value);
                    pro.InvoiceDate = ConvertCell.ConvertCellToDatetime(sheet.Cells[i, 20].Value);
                    pro.InvoiceName = ConvertCell.ConvertCellToString(sheet.Cells[i, 21].Value);
                    pro.InvoiceProductItem = ConvertCell.ConvertCellToString(sheet.Cells[i, 22].Value);
                    pro.DebitCode = ConvertCell.ConvertCellToString(sheet.Cells[i, 23].Value);
                    pro.DebitCodeName = ConvertCell.ConvertCellToString(sheet.Cells[i, 24].Value);
                    pro.DebitWarehouse = ConvertCell.ConvertCellToString(sheet.Cells[i, 25].Value);
                    pro.DebitWarehouseName = ConvertCell.ConvertCellToString(sheet.Cells[i, 26].Value);
                    pro.DebitDetailCodeFirst = ConvertCell.ConvertCellToString(sheet.Cells[i, 27].Value);
                    pro.DebitDetailCodeFirstName = ConvertCell.ConvertCellToString(sheet.Cells[i, 28].Value);
                    pro.DebitDetailCodeSecond = ConvertCell.ConvertCellToString(sheet.Cells[i, 29].Value);
                    pro.DebitDetailCodeSecondName = ConvertCell.ConvertCellToString(sheet.Cells[i, 30].Value);
                    pro.CreditCode = ConvertCell.ConvertCellToString(sheet.Cells[i, 31].Value);
                    pro.CreditCodeName = ConvertCell.ConvertCellToString(sheet.Cells[i, 32].Value);
                    pro.CreditWarehouse = ConvertCell.ConvertCellToString(sheet.Cells[i, 33].Value);
                    pro.CreditWarehouseName = ConvertCell.ConvertCellToString(sheet.Cells[i, 34].Value);
                    pro.CreditDetailCodeFirst = ConvertCell.ConvertCellToString(sheet.Cells[i, 35].Value);
                    pro.CreditDetailCodeFirstName = ConvertCell.ConvertCellToString(sheet.Cells[i, 36].Value);
                    pro.CreditDetailCodeSecond = ConvertCell.ConvertCellToString(sheet.Cells[i, 37].Value);
                    pro.CreditDetailCodeSecondName = sheet.Cells[i, 38].Value.ToString();
                    pro.ProjectCode = ConvertCell.ConvertCellToString(sheet.Cells[i, 39].Value);
                    pro.DepreciaMonth = ConvertCell.ConvertCellToInt(sheet.Cells[i, 40].Value.ToString());
                    pro.Quantity = ConvertCell.ConvertCellToDecimal(sheet.Cells[i, 41].Value.ToString());
                    pro.UnitPrice = ConvertCell.ConvertCellToDecimal(sheet.Cells[i, 42].Value.ToString());
                    pro.OrginalCurrency = ConvertCell.ConvertCellToDecimal(sheet.Cells[i, 43].Value.ToString());
                    pro.ExchangeRate = ConvertCell.ConvertCellToDecimal(sheet.Cells[i, 44].Value.ToString());
                    pro.Amount = ConvertCell.ConvertCellToDecimal(sheet.Cells[i, 45].Value.ToString());
                    pro.IsInternal = 1;
                    pro.Year = year;

                    //Id = 0,

                    await _context.Ledgers.AddAsync(pro);
                    i++;
                }
                await _context.SaveChangesAsync();
                await _context.Database.CommitTransactionAsync();
            }
        }
    }

    public MemoryStream ExportExcelSample()
    {
        string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/PSExcelmau.xlsx");
        MemoryStream stream = new MemoryStream();
        using (FileStream templateDocumentStream = File.OpenRead(path))
        {
            using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                package.SaveAs(stream);
        }
        stream.Seek(0L, SeekOrigin.Begin);
        return stream;
    }

}