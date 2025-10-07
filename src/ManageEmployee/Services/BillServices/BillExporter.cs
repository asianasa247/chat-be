using ManageEmployee.Helpers;
using OfficeOpenXml;
using DinkToPdf.Contracts;
using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Bills;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.DataTransferObject.BillModels;

namespace ManageEmployee.Services.BillServices;

public class BillExporter: IBillExporter
{
    private readonly ApplicationDbContext _context;
    private readonly IBillService _billService;
    private readonly IConverter _converterPDF;
    private readonly ICompanyService _companyService;
    private readonly IMapper _mapper;

    public BillExporter(
        ApplicationDbContext context,
        IBillService billService,
        IConverter converterPDF,
        ICompanyService companyService,
        IMapper mapper
    )
    {
        _context = context;
        _billService = billService;
        _converterPDF = converterPDF;
        _companyService = companyService;
        _mapper = mapper;
    }

    public async Task<string> ExportExcelSale(BillReportRequestModel param)
    {
        var data = await _billService.GetGoodForCustomer(param);
        string file = "";
        switch (param.Type)
        {
            case 0:
                file = ExportExcelSaleForCustomer(data, param);
                break;
            case 1:
                file = ExportExcelSaleForGood(data, param);
                break;
            case 2:
                file = ExportExcelSaleForUser(data, param);
                break;

        }
        return file;
    }

    private string ExportExcelSaleForGood(GoodForCustomeViewModel data, BillReportRequestModel param)
    {
        try
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/SaleForGood.xlsx");
            MemoryStream stream = new MemoryStream();
            using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                    int nRowBegin = 6;
                    int nCol = 9;
                    int iRow = nRowBegin;
                    worksheet.Cells[iRow, 1].Value = "TỔNG CỘNG";
                    worksheet.Cells[iRow, 1, iRow, 3].Merge = true;
                    worksheet.Cells[iRow, 4].Value = data.Quantity;
                    worksheet.Cells[iRow, 5].Value = data.Amount;
                    worksheet.Cells[iRow, 6].Value = data.QuantityBack;
                    worksheet.Cells[iRow, 7].Value = data.AmountBack;
                    worksheet.Cells[iRow, 8].Value = data.AmountProfit;
                    worksheet.Cells[iRow, 9].Value = "";
                    iRow++;

                    string fromDatestr = "";
                    string toDatestr = "";
                    if (param.StartDate != null)
                        fromDatestr = param.StartDate.Value.ToString("dd/MM/yyyy");
                    if (param.EndDate != null)
                        toDatestr = param.EndDate.Value.ToString("dd/MM/yyyy");

                    worksheet.Cells[3, 1].Value = string.Format($"Từ ngày {fromDatestr} đến ngày {toDatestr}");

                    if (data.Items != null)
                    {
                        foreach (var item in data.Items)
                        {

                            worksheet.Cells[iRow, 1].Value = item.Detail1 + "(" + item.Detail1Name + ")";
                            worksheet.Cells[iRow, 1, iRow, 3].Merge = true;
                            worksheet.Cells[iRow, 4].Value = item.Quantity;
                            worksheet.Cells[iRow, 5].Value = item.Amount;
                            worksheet.Cells[iRow, 6].Value = item.QuantityBack;
                            worksheet.Cells[iRow, 7].Value = item.AmountBack;
                            worksheet.Cells[iRow, 8].Value = item.AmountProfit;
                            worksheet.Cells[iRow, 9].Value = item.Detail1;
                            iRow++;
                            foreach (var itemC in item.Items)
                            {

                                worksheet.Cells[iRow, 1].Value = itemC.GoodCode;
                                worksheet.Cells[iRow, 2].Value = itemC.GoodName;
                                worksheet.Cells[iRow, 3].Value = itemC.StockUnit;
                                worksheet.Cells[iRow, 4].Value = itemC.Quantity;
                                worksheet.Cells[iRow, 5].Value = itemC.Amount;
                                worksheet.Cells[iRow, 6].Value = itemC.QuantityBack;
                                worksheet.Cells[iRow, 7].Value = itemC.AmountBack;
                                worksheet.Cells[iRow, 8].Value = itemC.AmountProfit;
                                worksheet.Cells[iRow, 9].Value = itemC.Detail1;
                                iRow++;
                            }
                        }
                        iRow--;

                        if (iRow >= nRowBegin)
                        {
                            worksheet.Cells[nRowBegin, 4, iRow, 8].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }
                    }
                    package.SaveAs(stream);
                    return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "SaleForGood");
                }
            }
        }
        catch
        {
            return string.Empty;
        }
    }
    private string ExportExcelSaleForUser(GoodForCustomeViewModel data, BillReportRequestModel param)
    {
        try
        {
            var user = _context.Users.Find(param.UserId);

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/SaleForUser.xlsx");
            MemoryStream stream = new MemoryStream();
            using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                    int nRowBegin = 6;
                    int nCol = 11;
                    int iRow = nRowBegin;

                    string fromDatestr = "";
                    string toDatestr = "";
                    if (param.StartDate != null)
                        fromDatestr = param.StartDate.Value.ToString("dd/MM/yyyy");
                    if (param.EndDate != null)
                        toDatestr = param.EndDate.Value.ToString("dd/MM/yyyy");

                    worksheet.Cells[3, 1].Value = string.Format($"Từ ngày {fromDatestr} đến ngày {toDatestr}");

                    worksheet.Cells[iRow, 1].Value = "TỔNG CỘNG";
                    worksheet.Cells[iRow, 1, iRow, 5].Merge = true;
                    worksheet.Cells[iRow, 6].Value = data.Quantity;
                    worksheet.Cells[iRow, 7].Value = data.Amount;
                    worksheet.Cells[iRow, 8].Value = data.QuantityBack;
                    worksheet.Cells[iRow, 9].Value = data.AmountBack;
                    worksheet.Cells[iRow, 10].Value = data.AmountProfit;
                    worksheet.Cells[iRow, 11].Value = "";
                    iRow++;
                    if (data.Items != null)
                    {

                        foreach (var item in data.Items)
                        {

                            worksheet.Cells[iRow, 1].Value = item.Detail1 + "(" + item.Detail1Name + ")";
                            worksheet.Cells[iRow, 1, iRow, 5].Merge = true;
                            worksheet.Cells[iRow, 6].Value = item.Quantity;
                            worksheet.Cells[iRow, 7].Value = item.Amount;
                            worksheet.Cells[iRow, 8].Value = item.QuantityBack;
                            worksheet.Cells[iRow, 9].Value = item.AmountBack;
                            worksheet.Cells[iRow, 10].Value = item.AmountProfit;
                            worksheet.Cells[iRow, 11].Value = item.Detail1;
                            iRow++;
                            foreach (var itemC in item.Items)
                            {

                                worksheet.Cells[iRow, 1].Value = itemC.GoodCode;
                                worksheet.Cells[iRow, 2].Value = "";
                                worksheet.Cells[iRow, 3].Value = "";
                                worksheet.Cells[iRow, 4].Value = itemC.GoodName;
                                worksheet.Cells[iRow, 5].Value = itemC.StockUnit;
                                worksheet.Cells[iRow, 6].Value = itemC.Quantity;
                                worksheet.Cells[iRow, 7].Value = itemC.Amount;
                                worksheet.Cells[iRow, 8].Value = itemC.QuantityBack;
                                worksheet.Cells[iRow, 9].Value = itemC.AmountBack;
                                worksheet.Cells[iRow, 10].Value = itemC.AmountProfit;
                                worksheet.Cells[iRow, 11].Value = itemC.Detail1;
                                iRow++;
                            }
                        }
                        iRow--;

                        if (iRow >= nRowBegin)
                        {
                            worksheet.Cells[nRowBegin, 6, iRow, 10].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }
                    }
                    package.SaveAs(stream);
                    return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "SaleForUser");
                }
            }
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ExportExcelSaleForCustomer(GoodForCustomeViewModel data, BillReportRequestModel param)
    {
        try
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/SaleForCustomer.xlsx");
            MemoryStream stream = new MemoryStream();
            using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets["Sheet1"];
                    int nRowBegin = 6;
                    int nCol = 10;
                    int iRow = nRowBegin;
                    string fromDatestr = "";
                    string toDatestr = "";
                    if (param.StartDate != null)
                        fromDatestr = param.StartDate.Value.ToString("dd/MM/yyyy");
                    if (param.EndDate != null)
                        toDatestr = param.EndDate.Value.ToString("dd/MM/yyyy");

                    worksheet.Cells[3, 1].Value = string.Format($"Từ ngày {fromDatestr} đến ngày {toDatestr}");

                    worksheet.Cells[iRow, 1].Value = "TỔNG CỘNG";
                    worksheet.Cells[iRow, 2, iRow, 4].Merge = true;
                    worksheet.Cells[iRow, 5].Value = data.Quantity;
                    worksheet.Cells[iRow, 6].Value = data.Amount;
                    worksheet.Cells[iRow, 7].Value = data.QuantityBack;
                    worksheet.Cells[iRow, 8].Value = data.AmountBack;
                    worksheet.Cells[iRow, 9].Value = data.AmountProfit;
                    worksheet.Cells[iRow, 10].Value = "";
                    iRow++;
                    if (data.Items != null)
                    {
                        foreach (var item in data.Items)
                        {

                            worksheet.Cells[iRow, 1].Value = item.Detail1 + "(" + item.Detail1Name + ")";
                            worksheet.Cells[iRow, 2, iRow, 4].Merge = true;
                            worksheet.Cells[iRow, 5].Value = item.Quantity;
                            worksheet.Cells[iRow, 6].Value = item.Amount;
                            worksheet.Cells[iRow, 7].Value = item.QuantityBack;
                            worksheet.Cells[iRow, 8].Value = item.AmountBack;
                            worksheet.Cells[iRow, 9].Value = item.AmountProfit;
                            worksheet.Cells[iRow, 10].Value = item.Detail1;
                            iRow++;
                            foreach (var itemC in item.Items)
                            {

                                worksheet.Cells[iRow, 1].Value = "";
                                worksheet.Cells[iRow, 2].Value = itemC.GoodCode;
                                worksheet.Cells[iRow, 3].Value = itemC.GoodName;
                                worksheet.Cells[iRow, 4].Value = itemC.StockUnit;
                                worksheet.Cells[iRow, 5].Value = itemC.Quantity;
                                worksheet.Cells[iRow, 6].Value = itemC.Amount;
                                worksheet.Cells[iRow, 7].Value = itemC.QuantityBack;
                                worksheet.Cells[iRow, 8].Value = itemC.AmountBack;
                                worksheet.Cells[iRow, 9].Value = itemC.AmountProfit;
                                worksheet.Cells[iRow, 10].Value = itemC.Detail1;
                                iRow++;
                            }
                        }
                        iRow--;

                        if (iRow >= nRowBegin)
                        {
                            worksheet.Cells[nRowBegin, 5, iRow, 9].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, iRow, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }
                    }
                    package.SaveAs(stream);
                    return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "SaleForCustomer");
                }
            }
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<string> ExportPdfGoodForSale(BillReportRequestModel param)
    {
        var data = await _billService.GetGoodForCustomer(param);
        string _allText = "";
        var company = await _companyService.GetCompany();

        switch (param.Type)
        {
            case 0:
                _allText = await ExportPdfSaleForCustomer(data, param, company);
                break;
            case 1:
                _allText = ExportPdfSaleForGood(data, param, company);
                break;
            case 2:
                _allText = await ExportPdfSaleForUser(data, param, company);
                break;

        }
        return ExcelHelpers.ConvertUseDinkLandscape(_allText, _converterPDF, Directory.GetCurrentDirectory(), "BaoCaoBanHang");
    }

    private string ExportPdfSaleForGood(
        GoodForCustomeViewModel data,
        BillReportRequestModel param,
        Company company
    )
    {
        try
        {
            if (data == null) return string.Empty;

            string _template = "Bill_Goods.html",
                _folderPath = @"Uploads\Html",
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = File.ReadAllText(path), resultHTML = string.Empty;
            IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TenCongTy", company.Name },
                { "DiaChi", company.Address },
                { "MST", company.MST },
                { "NgayChungTu", string.Empty },
                { "TuNgay", param.StartDate.Value.ToString("dd/MM/yyyy")},
                { "DenNgay", param.EndDate.Value.Month.ToString("dd/MM/yyyy") },
            };
            string soThapPhan = "N" + company.MethodCalcExportPrice;
            v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));

            if (data.Items.Count > 0)
            {

                string _txt = @"<tr>
                                            <td colspan = '3'>TỔNG CỘNG</td>
                                            <td class='txt-right'>{{{QUANTITY}}}</td>
                                            <td class='txt-right'>{{{AMOUNT}}}</td>
                                            <td class='txt-right'>{{{QUANTITY_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_PROFIT}}}</td>
                                            <td>{{{GOODS_PARENT_NAME}}}</td>
                                        </tr>";

                _txt = _txt
                                    .Replace("{{{QUANTITY}}}", data.Quantity > 0 ? string.Format("{0:" + soThapPhan + "}", data.Quantity) : string.Empty)
                                    .Replace("{{{AMOUNT}}}", data.Amount > 0 ? string.Format("{0:" + soThapPhan + "}", data.Amount) : string.Empty)
                                    .Replace("{{{QUANTITY_REFUND}}}", data.QuantityBack > 0 ? string.Format("{0:" + soThapPhan + "}", data.QuantityBack) : string.Empty)
                                    .Replace("{{{AMOUNT_REFUND}}}", data.AmountBack > 0 ? string.Format("{0:" + soThapPhan + "}", data.AmountBack) : string.Empty)
                                    .Replace("{{{AMOUNT_PROFIT}}}", data.AmountProfit > 0 ? string.Format("{0:" + soThapPhan + "}", data.AmountProfit) : string.Empty)
                                    .Replace("{{{GOODS_PARENT_NAME}}}", string.Empty);
                resultHTML += _txt;

                data.Items.ForEach(x =>
                {
                    _txt = @"<tr>
                                            <td colspan = '3'>{{{GOODS_PARENT}}}</td>
                                            <td class='txt-right'>{{{QUANTITY}}}</td>
                                            <td class='txt-right'>{{{AMOUNT}}}</td>
                                            <td class='txt-right'>{{{QUANTITY_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_PROFIT}}}</td>
                                            <td>{{{GOODS_PARENT_NAME}}}</td>
                                        </tr>";

                    _txt = _txt.Replace("{{{GOODS_PARENT}}}", string.Format($"{x.Detail1} ({x.Detail1Name})"))
                                        .Replace("{{{QUANTITY}}}", x.Quantity > 0 ? string.Format("{0:" + soThapPhan + "}", x.Quantity) : string.Empty)
                                        .Replace("{{{AMOUNT}}}", x.Amount > 0 ? string.Format("{0:" + soThapPhan + "}", x.Amount) : string.Empty)
                                        .Replace("{{{QUANTITY_REFUND}}}", x.QuantityBack > 0 ? string.Format("{0:" + soThapPhan + "}", x.QuantityBack) : string.Empty)
                                        .Replace("{{{AMOUNT_REFUND}}}", x.AmountBack > 0 ? string.Format("{0:" + soThapPhan + "}", x.AmountBack) : string.Empty)
                                        .Replace("{{{AMOUNT_PROFIT}}}", x.AmountProfit > 0 ? string.Format("{0:" + soThapPhan + "}", x.AmountProfit) : string.Empty)
                                        .Replace("{{{GOODS_PARENT_NAME}}}", x.Detail1);
                    resultHTML += _txt;
                    x.Items.ForEach(y =>
                    {
                        string _txt = @"<tr>
                                            <td>{{{GOODS_CODE}}}</td>
                                            <td>{{{GOODS_NAME}}}</td>
                                            <td class='txt-right'>{{{UNIT_PRICE}}}</td>
                                            <td class='txt-right'>{{{QUANTITY}}}</td>
                                            <td class='txt-right'>{{{AMOUNT}}}</td>
                                            <td class='txt-right'>{{{QUANTITY_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_PROFIT}}}</td>
                                            <td>{{{GOODS_PARENT_NAME}}}</td>
                                        </tr>";

                        _txt = _txt.Replace("{{{GOODS_CODE}}}", y.GoodCode)
                                            .Replace("{{{GOODS_NAME}}}", y.GoodName)
                                            .Replace("{{{UNIT_PRICE}}}", y.StockUnit)
                                            .Replace("{{{QUANTITY}}}", y.Quantity > 0 ? string.Format("{0:" + soThapPhan + "}", y.Quantity) : string.Empty)
                                            .Replace("{{{AMOUNT}}}", y.Amount > 0 ? string.Format("{0:" + soThapPhan + "}", y.Amount) : string.Empty)
                                            .Replace("{{{QUANTITY_REFUND}}}", y.QuantityBack > 0 ? string.Format("{0:" + soThapPhan + "}", y.QuantityBack) : string.Empty)
                                            .Replace("{{{AMOUNT_REFUND}}}", y.AmountBack > 0 ? string.Format("{0:" + soThapPhan + "}", y.AmountBack) : string.Empty)
                                            .Replace("{{{AMOUNT_PROFIT}}}", y.AmountProfit > 0 ? string.Format("{0:" + soThapPhan + "}", y.AmountProfit) : string.Empty)
                                            .Replace("{{{GOODS_PARENT_NAME}}}", y.Detail1);
                        resultHTML += _txt;

                    });
                });
            }

            _allText = _allText.Replace("##TOTAL_REPLACE_PLACE##", resultHTML);

            return _allText;
        }
        catch
        {
            return string.Empty;
        }
    }

    private async Task<string> ExportPdfSaleForCustomer(
        GoodForCustomeViewModel data,
        BillReportRequestModel param,
        Company company
    )
    {
        try
        {
            if (data == null) return string.Empty;
            var customer = await _context.Customers.FindAsync(param.CustomerId);
            string _template = "Bill_Goods_And_Customer.html",
                _folderPath = @"Uploads\Html",
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = File.ReadAllText(path), resultHTML = string.Empty;
            IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TenCongTy", company.Name },
                { "TEN_KHACH_HANG", customer?.Name },
                { "DiaChi", company.Address },
                { "MST", company.MST },
                { "NgayChungTu", string.Empty },
                { "TuNgay", param.StartDate.Value.ToString("dd/MM/yyyy")},
                { "DenNgay", param.EndDate.Value.Month.ToString("dd/MM/yyyy") },
            };
            string soThapPhan = "N" + company.MethodCalcExportPrice;
            v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));

            if (data.Items.Count > 0)
            {
                string _txt = @"<tr>
                                            <td colspan = '4'>TỔNG CỘNG</td>
                                            <td class='txt-right'>{{{QUANTITY}}}</td>
                                            <td class='txt-right'>{{{AMOUNT}}}</td>
                                            <td class='txt-right'>{{{QUANTITY_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_PROFIT}}}</td>
                                            <td>{{{GOODS_PARENT_NAME}}}</td>
                                        </tr>";

                _txt = _txt
                                    .Replace("{{{QUANTITY}}}", data.Quantity > 0 ? string.Format("{0:" + soThapPhan + "}", data.Quantity) : string.Empty)
                                    .Replace("{{{AMOUNT}}}", data.Amount > 0 ? string.Format("{0:" + soThapPhan + "}", data.Amount) : string.Empty)
                                    .Replace("{{{QUANTITY_REFUND}}}", data.QuantityBack > 0 ? string.Format("{0:" + soThapPhan + "}", data.QuantityBack) : string.Empty)
                                    .Replace("{{{AMOUNT_REFUND}}}", data.AmountBack > 0 ? string.Format("{0:" + soThapPhan + "}", data.AmountBack) : string.Empty)
                                    .Replace("{{{AMOUNT_PROFIT}}}", data.AmountProfit > 0 ? string.Format("{0:" + soThapPhan + "}", data.AmountProfit) : string.Empty)
                                    .Replace("{{{GOODS_PARENT_NAME}}}", string.Empty);
                resultHTML += _txt;

                data.Items.ForEach(x =>
                {
                    _txt = @"<tr>
                                            <td colspan = '4'>{{{GOODS_PARENT}}}</td>
                                            <td class='txt-right'>{{{QUANTITY}}}</td>
                                            <td class='txt-right'>{{{AMOUNT}}}</td>
                                            <td class='txt-right'>{{{QUANTITY_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_PROFIT}}}</td>
                                            <td>{{{GOODS_PARENT_NAME}}}</td>
                                        </tr>";

                    _txt = _txt.Replace("{{{GOODS_PARENT}}}", string.Format($"{x.Detail1} ({x.Detail1Name})"))
                                        .Replace("{{{QUANTITY}}}", x.Quantity > 0 ? string.Format("{0:" + soThapPhan + "}", x.Quantity) : string.Empty)
                                        .Replace("{{{AMOUNT}}}", x.Amount > 0 ? string.Format("{0:" + soThapPhan + "}", x.Amount) : string.Empty)
                                        .Replace("{{{QUANTITY_REFUND}}}", x.QuantityBack > 0 ? string.Format("{0:" + soThapPhan + "}", x.QuantityBack) : string.Empty)
                                        .Replace("{{{AMOUNT_REFUND}}}", x.AmountBack > 0 ? string.Format("{0:" + soThapPhan + "}", x.AmountBack) : string.Empty)
                                        .Replace("{{{AMOUNT_PROFIT}}}", x.AmountProfit > 0 ? string.Format("{0:" + soThapPhan + "}", x.AmountProfit) : string.Empty)
                                        .Replace("{{{GOODS_PARENT_NAME}}}", x.Detail1);
                    resultHTML += _txt;
                    x.Items.ForEach(y =>
                    {
                        string _txt = @"<tr>
                                            <td>{{{CUSTOMER_CODE}}}</td>
                                            <td>{{{GOODS_CODE}}}</td>
                                            <td>{{{GOODS_NAME}}}</td>
                                            <td class='txt-right'>{{{UNIT_PRICE}}}</td>
                                            <td class='txt-right'>{{{QUANTITY}}}</td>
                                            <td class='txt-right'>{{{AMOUNT}}}</td>
                                            <td class='txt-right'>{{{QUANTITY_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_PROFIT}}}</td>
                                            <td>{{{GOODS_PARENT_NAME}}}</td>
                                        </tr>";

                        _txt = _txt.Replace("{{{CUSTOMER_CODE}}}", string.Empty)
                                            .Replace("{{{GOODS_CODE}}}", y.GoodCode)
                                            .Replace("{{{GOODS_NAME}}}", y.GoodName)
                                            .Replace("{{{UNIT_PRICE}}}", y.StockUnit)
                                            .Replace("{{{QUANTITY}}}", y.Quantity > 0 ? string.Format("{0:" + soThapPhan + "}", y.Quantity) : string.Empty)
                                            .Replace("{{{AMOUNT}}}", y.Amount > 0 ? string.Format("{0:" + soThapPhan + "}", y.Amount) : string.Empty)
                                            .Replace("{{{QUANTITY_REFUND}}}", y.QuantityBack > 0 ? string.Format("{0:" + soThapPhan + "}", y.QuantityBack) : string.Empty)
                                            .Replace("{{{AMOUNT_REFUND}}}", y.AmountBack > 0 ? string.Format("{0:" + soThapPhan + "}", y.AmountBack) : string.Empty)
                                            .Replace("{{{AMOUNT_PROFIT}}}", y.AmountProfit > 0 ? string.Format("{0:" + soThapPhan + "}", y.AmountProfit) : string.Empty)
                                            .Replace("{{{GOODS_PARENT_NAME}}}", y.Detail1);
                        resultHTML += _txt;

                    });
                });
            }

            _allText = _allText.Replace("##TOTAL_REPLACE_PLACE##", resultHTML);

            return _allText;
        }
        catch
        {
            return string.Empty;
        }
    }

    private async Task<string> ExportPdfSaleForUser(
        GoodForCustomeViewModel data,
        BillReportRequestModel param,
        Company company
    )
    {
        try
        {
            if (data == null) return string.Empty;

            var user = await _context.Users.FindAsync(param.UserId);

            string _template = "Bill_Goods_And_User.html",
                _folderPath = @"Uploads\Html",
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = File.ReadAllText(path), resultHTML = string.Empty;
            IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TEN_NHAN_VIEN", user?.FullName },
                { "TenCongTy", company.Name },
                { "DiaChi", company.Address },
                { "MST", company.MST },
                { "NgayChungTu", string.Empty },
                { "TuNgay", param.StartDate.Value.ToString("dd/MM/yyyy")},
                { "DenNgay", param.EndDate.Value.Month.ToString("dd/MM/yyyy") },
            };
            string soThapPhan = "N" + company.MethodCalcExportPrice;
            v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));

            if (data.Items.Count > 0)
            {
                string _txt = @"<tr>
                                            <td colspan = '5'>TỔNG CỘNG</td>
                                            <td class='txt-right'>{{{QUANTITY}}}</td>
                                            <td class='txt-right'>{{{AMOUNT}}}</td>
                                            <td class='txt-right'>{{{QUANTITY_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_PROFIT}}}</td>
                                            <td>{{{GOODS_PARENT_NAME}}}</td>
                                        </tr>";

                _txt = _txt
                                    .Replace("{{{QUANTITY}}}", data.Quantity > 0 ? string.Format("{0:" + soThapPhan + "}", data.Quantity) : string.Empty)
                                    .Replace("{{{AMOUNT}}}", data.Amount > 0 ? string.Format("{0:" + soThapPhan + "}", data.Amount) : string.Empty)
                                    .Replace("{{{QUANTITY_REFUND}}}", data.QuantityBack > 0 ? string.Format("{0:" + soThapPhan + "}", data.QuantityBack) : string.Empty)
                                    .Replace("{{{AMOUNT_REFUND}}}", data.AmountBack > 0 ? string.Format("{0:" + soThapPhan + "}", data.AmountBack) : string.Empty)
                                    .Replace("{{{AMOUNT_PROFIT}}}", data.AmountProfit > 0 ? string.Format("{0:" + soThapPhan + "}", data.AmountProfit) : string.Empty)
                                    .Replace("{{{GOODS_PARENT_NAME}}}", string.Empty);
                resultHTML += _txt;

                data.Items.ForEach(x =>
                {
                    _txt = @"<tr>
                                            <td colspan = '5'>{{{GOODS_PARENT}}}</td>
                                            <td class='txt-right'>{{{QUANTITY}}}</td>
                                            <td class='txt-right'>{{{AMOUNT}}}</td>
                                            <td class='txt-right'>{{{QUANTITY_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_PROFIT}}}</td>
                                            <td>{{{GOODS_PARENT_NAME}}}</td>
                                        </tr>";

                    _txt = _txt.Replace("{{{GOODS_PARENT}}}", string.Format($"{x.Detail1} ({x.Detail1Name})"))
                                        .Replace("{{{QUANTITY}}}", x.Quantity > 0 ? string.Format("{0:" + soThapPhan + "}", x.Quantity) : string.Empty)
                                        .Replace("{{{AMOUNT}}}", x.Amount > 0 ? string.Format("{0:" + soThapPhan + "}", x.Amount) : string.Empty)
                                        .Replace("{{{QUANTITY_REFUND}}}", x.QuantityBack > 0 ? string.Format("{0:" + soThapPhan + "}", x.QuantityBack) : string.Empty)
                                        .Replace("{{{AMOUNT_REFUND}}}", x.AmountBack > 0 ? string.Format("{0:" + soThapPhan + "}", x.AmountBack) : string.Empty)
                                        .Replace("{{{AMOUNT_PROFIT}}}", x.AmountProfit > 0 ? string.Format("{0:" + soThapPhan + "}", x.AmountProfit) : string.Empty)
                                        .Replace("{{{GOODS_PARENT_NAME}}}", x.Detail1);
                    resultHTML += _txt;
                    x.Items.ForEach(y =>
                    {
                        string _txt = @"<tr>
                                            <td>{{{GOODS_CODE}}}</td>
                                            <td>{{{USER_CODE}}}</td>
                                            <td>{{{USER_NAME}}}</td>
                                            <td>{{{GOODS_NAME}}}</td>
                                            <td class='txt-right'>{{{UNIT_PRICE}}}</td>
                                            <td class='txt-right'>{{{QUANTITY}}}</td>
                                            <td class='txt-right'>{{{AMOUNT}}}</td>
                                            <td class='txt-right'>{{{QUANTITY_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_REFUND}}}</td>
                                            <td class='txt-right'>{{{AMOUNT_PROFIT}}}</td>
                                            <td>{{{GOODS_PARENT_NAME}}}</td>
                                        </tr>";

                        _txt = _txt.Replace("{{{USER_CODE}}}", string.Empty)
                                            .Replace("{{{USER_NAME}}}", string.Empty)
                                            .Replace("{{{GOODS_CODE}}}", y.GoodCode)
                                            .Replace("{{{GOODS_NAME}}}", y.GoodName)
                                            .Replace("{{{UNIT_PRICE}}}", y.StockUnit)
                                            .Replace("{{{QUANTITY}}}", y.Quantity > 0 ? string.Format("{0:" + soThapPhan + "}", y.Quantity) : string.Empty)
                                            .Replace("{{{AMOUNT}}}", y.Amount > 0 ? string.Format("{0:" + soThapPhan + "}", y.Amount) : string.Empty)
                                            .Replace("{{{QUANTITY_REFUND}}}", y.QuantityBack > 0 ? string.Format("{0:" + soThapPhan + "}", y.QuantityBack) : string.Empty)
                                            .Replace("{{{AMOUNT_REFUND}}}", y.AmountBack > 0 ? string.Format("{0:" + soThapPhan + "}", y.AmountBack) : string.Empty)
                                            .Replace("{{{AMOUNT_PROFIT}}}", y.AmountProfit > 0 ? string.Format("{0:" + soThapPhan + "}", y.AmountProfit) : string.Empty)
                                            .Replace("{{{GOODS_PARENT_NAME}}}", y.Detail1);
                        resultHTML += _txt;

                    });
                });
            }

            _allText = _allText.Replace("##TOTAL_REPLACE_PLACE##", resultHTML);

            return _allText;
        }
        catch
        {
            return string.Empty;
        }
    }

}
