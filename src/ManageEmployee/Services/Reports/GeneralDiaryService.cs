using DinkToPdf.Contracts;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Entities.CompanyEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.Reports;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ManageEmployee.Services.Reports;
public class GeneralDiaryService : IGeneralDiaryService
{
    private readonly ApplicationDbContext _context;
    private readonly IConverter _converterPDF;
    private readonly ICompanyService _companyService;

    public GeneralDiaryService(ApplicationDbContext context, IConverter converPDF, ICompanyService companyService)
    {
        _context = context;
        _converterPDF = converPDF;
        _companyService = companyService;
    }
    public async Task<string> GenerateGeneralDiaryReport(GeneralDiaryReportParam param, int year)
    {
        DateTime fromdt = param.FromDate;
        DateTime todt = param.ToDate;
        if(param.FromMonth > 0)
            fromdt = new DateTime(DateTime.Now.Year, param.FromMonth, 1);
        if (param.ToMonth > 0)
            todt = new DateTime(DateTime.Now.Year, param.ToMonth, 1).AddMonths(1);
        var reportItems = _context.GetLedger(year, 2).Where(x => x.OrginalBookDate.Value >= fromdt && x.OrginalBookDate.Value < todt)
                        .OrderBy(x => x.OrginalBookDate).Select(x => new GeneralDiaryReportItem()
                        {
                            VoucherNumber = x.VoucherNumber,
                            OrginalBookDate = x.OrginalBookDate ?? DateTime.Now,
                            BookDate = x.BookDate ?? DateTime.Now,
                            OrginalDescription = x.OrginalDescription,
                            DebitCode = x.DebitCode,
                            Amount = x.Amount,
                            CreditCode = x.CreditCode,
                        }).ToList()
                        .GroupBy(x => x.BookDate.Value.Month)
                        .Select(x => new GenaralDiaryViewModel
                        {
                            Month = x.Key,
                            Items = x.GroupBy(x => x.VoucherNumber).Select(c => new GenaralDiaryVoucherViewModel
                            {
                                VoucherNumber = c.Key,
                                Items = c.Select(k => new GeneralDiaryReportItem
                                {

                                    VoucherNumber = k.VoucherNumber,
                                    OrginalBookDate = k.OrginalBookDate ?? DateTime.Now,
                                    BookDate = k.BookDate ?? DateTime.Now,
                                    OrginalDescription = k.OrginalDescription,
                                    DebitCode = k.DebitCode,
                                    Amount = k.Amount,
                                    CreditCode = k.CreditCode,
                                }).ToList()
                            }).ToList(),
                            Amount = x.Sum(x => (decimal?)x.Amount) ?? 0

                        }).ToList();
        var _company = await _companyService.GetCompany();

        return ExportDataReport(reportItems, _company, param);
    }
    private string ExportDataReport(List<GenaralDiaryViewModel> reportItems, Company company, GeneralDiaryReportParam param)
    {

        string _path = string.Empty;
        switch (param.FileType)
        {
            case "html":
                _path = ConvertToHTML_Report(reportItems, company, param);
                break;
            case "excel":
                _path = ExportExcel_Report(reportItems, company, param);
                break;
            case "pdf":
                _path = ConvertToPDFFile_Report(reportItems, company, param);
                break;
        }
        return _path;
    }
    private string ConvertToHTML_Report(List<GenaralDiaryViewModel> reportItems, Company company, GeneralDiaryReportParam param)
    {
        try
        {
            if (reportItems == null) return string.Empty;

            string _template = "NhatKyChung.html",
                _folderPath = @"Uploads\Html", resultStr = string.Empty,
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = System.IO.File.ReadAllText(path), resultHTML = string.Empty;

            IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TenCongTy", company.Name ?? ""},
                { "DiaChi", company.Address ?? ""},
                { "MST", company.MST },
                { "NgayChungTu", string.Empty },
                { "TuThang", ((param.FromMonth > 0 && param.ToMonth > 0) ? (param.FromMonth) : param.FromDate.Month ).ToString("D2")   },
                { "DenThang", ( (param.FromMonth > 0 && param.ToMonth > 0) ? (param.ToMonth) : param.ToDate.Month ).ToString("D2") },
                { "Nam", ((param.FromMonth > 0 && param.ToMonth > 0) ? DateTime.Now.Year : param.FromDate.Year ).ToString("D4") },
                { "NguoiLap", !string.IsNullOrEmpty(param.LedgerReportMaker) ? param.LedgerReportMaker : string.Empty },
                { "Ngay", " .... " },
                { "Thang",  " .... " },
                { "NamSign",  " .... " },
                { "KeToanTruong", param.isCheckName ? company.NameOfChiefAccountant : string.Empty },
                { "GiamDoc", param.isCheckName ? company.NameOfCEO : string.Empty },
                { "LUYvouchersE_DAU_NAM", string.Empty},
                { "KeToanTruong_CV", company.NoteOfChiefAccountant},
                { "GiamDoc_CV", company.NoteOfCEO},
                { "LUY_KE_DAU_NAM", company.NoteOfCEO},

            };
            string soThapPhan = "N" + company.MethodCalcExportPrice;
            v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));
            if (reportItems.Count > 0)
            {
                reportItems.ForEach(x =>
                {
                    foreach (var vouchers in x.Items)
                    {
                        string _txt = @"<tr>
                                                <td>{{{NGAY_GHI_SO}}}</td>
                                                <td>{{{CHUNG_TU_SO}}}</td>
                                                <td>{{{CHUNG_TU_NGAY}}}</td>
                                                <td class='tbl-td-diengiai'>{{{DIEN_GIAI}}}</td>
                                                <td>{{{TAIvouchersHOAN}}}</td>
                                                <td class='txt-right'>{{{SO_TIEN_NO}}}</td>
                                                <td class='txt-right'>{{{SO_TIEN_CO}}}</td>
                                            </tr>";

                        string descriptions = "";
                        string codes = "";
                        string soTienNos = "";
                        string soTienCos = "";
                        var debitCodes = vouchers.Items.Select(x => x.DebitCode).Distinct();
                        foreach (var debitCode in debitCodes)
                        {
                            descriptions += vouchers.Items.FirstOrDefault(x => x.DebitCode == debitCode).OrginalDescription + "<br/>";
                            codes += debitCode + "<br/>";
                            soTienNos += (vouchers.Items.Where(x => x.DebitCode == debitCode).Sum(x => x.Amount) > 0 ? String.Format("{0:" + soThapPhan + "}", x.Amount) : string.Empty) + "<br/>";
                            soTienCos += "<br/>";
                        }
                        var creditCodes = vouchers.Items.Select(x => x.CreditCode).Distinct();
                        foreach (var creditCode in creditCodes)
                        {
                            descriptions += vouchers.Items.FirstOrDefault(x => x.CreditCode == creditCode).OrginalDescription + "<br/>";
                            codes += "<div class='txt-right'>" +creditCode + "</div><br/>";
                            soTienCos += (vouchers.Items.Where(x => x.CreditCode == creditCode).Sum(x => x.Amount) > 0 ? String.Format("{0:" + soThapPhan + "}", x.Amount) : string.Empty) + "<br/>";
                            soTienNos += "<br/>";
                        }

                        _txt = _txt.Replace("{{{NGAY_GHI_SO}}}", vouchers.Items[0].BookDate.HasValue ? vouchers.Items[0].BookDate.Value.ToString("dd/MM") : string.Empty)
                                                .Replace("{{{CHUNG_TU_SO}}}", vouchers.Items[0].VoucherNumber)
                                                .Replace("{{{CHUNG_TU_NGAY}}}", vouchers.Items[0].OrginalBookDate.HasValue ? vouchers.Items[0].OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty)
                                                .Replace("{{{DIEN_GIAI}}}", descriptions)
                                                .Replace("{{{TAIvouchersHOAN}}}", codes)
                                                .Replace("{{{SO_TIEN_NO}}}", soTienNos)
                                                .Replace("{{{SO_TIEN_CO}}}", soTienCos)
                                                ;
                        resultHTML += _txt;

                    }

                    string _sumRowMonthHTML = @"<tr class='font-b'>
                                                            <td colspan='4'>Cộng phát sinh tháng {{THANG_ROW}}</td>
                                                            <td></td>
                                                            <td class='txt-right'>{{SR_TONG_SO_TIEN_PS_NO_CONG}}</td>
                                                            <td class='txt-right'>{{SR_TONG_SO_TIEN_PS_CO_CONG}}</td>
                                                        </tr>    
                                               <tr class='font-b'>
                                                            <td colspan='4'>Lũy kế phát sinh từ đầu năm</td>
                                                            <td></td>
                                                            <td class='txt-right'>{{SR_TONG_SO_TIEN_PS_NO_LK}}</td>
                                                            <td class='txt-right'>{{SR_TONG_SO_TIEN_PS_CO_LK}}</td>
                                                        </tr>    ";

                    _sumRowMonthHTML = _sumRowMonthHTML.Replace("{{THANG_ROW}}", x.Month.ToString("00"))

                        .Replace("{{SR_TONG_SO_TIEN_PS_NO_CONG}}", String.Format("{0:" + soThapPhan + "}", x.Amount))
                        .Replace("{{SR_TONG_SO_TIEN_PS_CO_CONG}}", String.Format("{0:" + soThapPhan + "}", x.Amount))

                        .Replace("{{SR_TONG_SO_TIEN_PS_NO_LK}}", String.Format("{0:" + soThapPhan + "}", x.Amount))
                        .Replace("{{SR_TONG_SO_TIEN_PS_CO_LK}}", String.Format("{0:" + soThapPhan + "}", x.Amount));
                    resultHTML += _sumRowMonthHTML;
                });
            }

            _allText = _allText.Replace("##REPLACE_PLACE##", resultHTML);
            return _allText;
        }
        catch
        {
            return string.Empty;
        }
    }
    private string ConvertToPDFFile_Report(List<GenaralDiaryViewModel> reportItems, Company company, GeneralDiaryReportParam param)
    {
        try
        {
            string _allText = ConvertToHTML_Report(reportItems, company, param);
            return ExcelHelpers.ConvertUseDinkLandscape(_allText, _converterPDF, Directory.GetCurrentDirectory(), "SoNhatKyChung");
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ExportExcel_Report(List<GenaralDiaryViewModel> reportItems, Company company, GeneralDiaryReportParam param)
    {
        try
        {
            string sTenFile = "SoNhatKyChung.xlsx";
            int nCol = 7;

            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/" + sTenFile);
            MemoryStream stream = new MemoryStream();
            using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    worksheet.Cells["A5"].Value = "SỔ NHẬT KÝ CHUNG";
                    worksheet.Cells["A6"].Value = "Từ tháng ... đến tháng ... năm ... ";

                    worksheet.Cells["A8"].Value = "Đơn vị tính: Đồng";

                    int currentRowNo = 10, flagRowNo = 0, nRowBegin = 10;

                    foreach (var item in reportItems)
                    {

                        foreach (var vouchers in item.Items)
                        {
                            currentRowNo++;

                            worksheet.Cells[currentRowNo, 1].Value = vouchers.Items[0].BookDate.HasValue ? vouchers.Items[0].BookDate.Value.ToString("dd/MM") : string.Empty;
                            worksheet.Cells[currentRowNo, 2].Value = vouchers.VoucherNumber;
                            worksheet.Cells[currentRowNo, 3].Value = vouchers.Items[0].OrginalBookDate.HasValue ? vouchers.Items[0].OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                            int nRow = currentRowNo;
                            var debitCodes = vouchers.Items.Select(x => x.DebitCode).Distinct();
                            foreach (var debitCode in debitCodes)
                            {
                                worksheet.Cells[currentRowNo, 4].Value = vouchers.Items.FirstOrDefault(x => x.DebitCode == debitCode).OrginalDescription;
                                worksheet.Cells[currentRowNo, 5].Value = debitCode;
                                worksheet.Cells[currentRowNo, 6].Value = vouchers.Items.Where(x => x.DebitCode == debitCode).Sum(x => x.Amount);
                                currentRowNo++;
                            }
                            var creditCodes = vouchers.Items.Select(x => x.CreditCode).Distinct();
                            foreach (var creditCode in creditCodes)
                            {
                                worksheet.Cells[currentRowNo, 4].Value = vouchers.Items.FirstOrDefault(x => x.CreditCode == creditCode).OrginalDescription;
                                worksheet.Cells[currentRowNo, 5].Value = creditCode;
                                worksheet.Cells[currentRowNo, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                worksheet.Cells[currentRowNo, 7].Value = vouchers.Items.Where(x => x.CreditCode == creditCode).Sum(x => x.Amount);
                                currentRowNo++;
                            }
                            currentRowNo--;

                            worksheet.Cells[nRow, 1, currentRowNo, 1].Merge = true;
                            worksheet.Cells[nRow, 2, currentRowNo, 2].Merge = true;
                            worksheet.Cells[nRow, 3, currentRowNo, 3].Merge = true;
                        }
                        {

                            currentRowNo++;
                            worksheet.Cells[currentRowNo, 1, currentRowNo, 5].Merge = true;
                            worksheet.Cells[currentRowNo, 1, currentRowNo, 5].Value = "Cộng phát sinh tháng " + item.Month;
                            worksheet.Cells[currentRowNo, 6].Value = item.Amount;
                            worksheet.Cells[currentRowNo, 7].Value = item.Amount;

                            currentRowNo++;
                            worksheet.Cells[currentRowNo, 1, currentRowNo, 5].Merge = true;
                            worksheet.Cells[currentRowNo, 1, currentRowNo, 5].Value = "Lũy kế phát sinh từ đầu năm";
                            worksheet.Cells[currentRowNo, 6].Value = item.Amount;
                            worksheet.Cells[currentRowNo, 7].Value = item.Amount;
                        }
                    }

                    flagRowNo = currentRowNo;

                    string _month = string.Empty;
                    IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
                    {
                        { "TenCongTy", company.Name ?? ""},
                        { "DiaChi", company.Address ?? ""},
                        { "MST", company.MST },
                        { "NgayChungTu", string.Empty },
                        { "TuThang", ((param.FromMonth > 0 && param.ToMonth > 0) ? (param.FromMonth) : param.FromDate.Month ).ToString("D2")   },
                        { "DenThang", ( (param.FromMonth > 0 && param.ToMonth > 0) ? (param.ToMonth) : param.ToDate.Month ).ToString("D2") },
                        { "Nam", ((param.FromMonth > 0 && param.ToMonth > 0) ? DateTime.Now.Year : param.FromDate.Year ).ToString("D4") },
                        { "NguoiLap", !string.IsNullOrEmpty(param.LedgerReportMaker) ? param.LedgerReportMaker : string.Empty },
                        { "Ngay", " .... " },
                        { "Thang",  " .... " },
                        { "NamSign",  " .... " },
                        { "KeToanTruong", param.isCheckName ? company.NameOfChiefAccountant : string.Empty },
                        { "GiamDoc", param.isCheckName ? company.NameOfCEO : string.Empty },
                        { "LUYvouchersE_DAU_NAM", string.Empty},
                        { "KeToanTruong_CV", company.NoteOfChiefAccountant},
                        { "GiamDoc_CV", company.NoteOfCEO},
                    };

                    currentRowNo++;
                    worksheet.Cells[currentRowNo, 6, currentRowNo, 7].Merge = true;
                    worksheet.Cells[currentRowNo, 6, currentRowNo, 7].Value = "Ngày ... tháng ... năm";

                    currentRowNo++;
                    worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Merge = true;
                    worksheet.Cells[currentRowNo, 4, currentRowNo, 5].Merge = true;
                    worksheet.Cells[currentRowNo, 6, currentRowNo, 7].Merge = true;
                    worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Value = "Người ghi sổ";
                    worksheet.Cells[currentRowNo, 4, currentRowNo, 5].Value = v_dicFixed["KeToanTruong_CV"];
                    worksheet.Cells[currentRowNo, 6, currentRowNo, 7].Value = v_dicFixed["GiamDoc_CV"];

                    currentRowNo += 4;
                    worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Merge = true;
                    worksheet.Cells[currentRowNo, 4, currentRowNo, 5].Merge = true;
                    worksheet.Cells[currentRowNo, 6, currentRowNo, 7].Merge = true;
                    worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Value = v_dicFixed["NguoiLap"];
                    worksheet.Cells[currentRowNo, 4, currentRowNo, 5].Value = v_dicFixed["KeToanTruong"];
                    worksheet.Cells[currentRowNo, 6, currentRowNo, 7].Value = v_dicFixed["GiamDoc"];

                    currentRowNo--;
                    if (currentRowNo > 10)
                    {
                        worksheet.Cells[nRowBegin, 6, flagRowNo, nCol].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                        worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                        worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }

                    return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "SoNhatKy");
                }
            }
        }
        catch
        {
            return string.Empty;
        }
    }
}
public class GenaralDiaryViewModel
{
public int Month { get; set; }
public decimal Amount { get; set; }
public List<GenaralDiaryVoucherViewModel>? Items { get; set; }
}
public class GenaralDiaryVoucherViewModel
{
public string? VoucherNumber { get; set; }
public List<GeneralDiaryReportItem>? Items { get; set; }
}