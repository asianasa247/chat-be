using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Text;
using DinkToPdf.Contracts;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Reports;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.LedgerEntities;

namespace ManageEmployee.Services.Reports;
public class LedgerDangKiChungTuGhiSoService : ILedgerDangKiChungTuGhiSoService
{
    private readonly ApplicationDbContext _context;
    private readonly ICompanyService _companyService;
    private readonly IConverter _converterPDF;

    public LedgerDangKiChungTuGhiSoService(ApplicationDbContext context, ICompanyService companyService, IConverter converterPDF)
    {
        _context = context;
        _companyService = companyService;
        _converterPDF = converterPDF;
    }
    public async Task<string> GetDataReport_DangKyChungTuGhiSo(LedgerReportParam _param, int year, bool isNoiBo = false)
    {
        try
        {
            if (_param.FromMonth == null) _param.FromMonth = 0;
            if (_param.ToMonth == null) _param.ToMonth = 0;
            if (_param.FromDate == null) _param.FromDate = new DateTime();
            if (_param.ToDate == null) _param.ToDate = new DateTime();
            if (string.IsNullOrEmpty(_param.AccountCode)) _param.AccountCode = string.Empty;
            _param.AccountCode = _param.AccountCode.Trim();

            var _accountFind = await _context.GetChartOfAccount(year).Where(x => x.Code == _param.AccountCode).FirstOrDefaultAsync();
            var _company = await _companyService.GetCompany();
            List<Ledger> _lstDKCT = await _context.GetLedger(year, isNoiBo ? 3 : 2).ToListAsync();


            List<DangKyChungTuGhiSo> _lstDKCTGS = _lstDKCT
                   .GroupBy(x => new { x.Type, x.VoucherNumber })
                   .Select(x => new DangKyChungTuGhiSo
                   {
                       Type = x.Select(k => k.Type).FirstOrDefault(),
                       VoucherNumber = x.Select(k => k.VoucherNumber).FirstOrDefault(),
                       Amount = x.Sum(k => k.Amount),
                       OrginalBookDate = x.Max(k => k.OrginalBookDate ?? DateTime.Now),
                       MonthOfYear = (x.Max(k => k.OrginalBookDate ?? DateTime.Now)).ToString("MM/yyyy"),
                       OrginalDescription = "Kèm theo " + x.Count(k => k.Id > 0).ToString() + " chứng từ"
                   }).OrderBy(k => k.OrginalBookDate).ToList();

            List<string> lstDT = _lstDKCTGS.Select(x => x.MonthOfYear).Distinct().ToList();
            IDictionary<string, List<DangKyChungTuGhiSo>> v_dic = new Dictionary<string, List<DangKyChungTuGhiSo>>();
            foreach (string dt in lstDT)
            {
                if (!v_dic.ContainsKey(dt))
                    v_dic[dt] = _lstDKCTGS.Where(x => x.MonthOfYear.Equals(dt)).ToList();
            }

            TotalAmountDKCTGS p = new()
            {
                Title_CT = "Cộng tháng",
                Sum_CT = _lstDKCTGS.Sum(x => x.Amount),
                Title_LKDN = "Lũy kế từ đầu năm",
                Sum_LKDN = _lstDKCTGS.Sum(x => x.Amount),
                Company = _company.Name,
                Address = _company.Address,
                TaxId = _company.MST,
                AccountCode = _accountFind?.Code,
                AccountName = _accountFind?.Name,
                CEOName = _company.NameOfCEO,
                ChiefAccountName = _company.NameOfChiefAccountant,
                CEONote = _company.NoteOfCEO,
                ChiefAccountNote = _company.NoteOfChiefAccountant
            };

            return ExportData_DKCTGS_List(v_dic, _param, p, year);
        }
        catch
        {
            return string.Empty;
        }
    }


    private string ExportData_DKCTGS_List(IDictionary<string, List<DangKyChungTuGhiSo>> ledgers, LedgerReportParam param, TotalAmountDKCTGS total, int year)
    {
        try
        {
            string _path = string.Empty;
            switch (param.FileType)
            {
                case "html":
                    _path = ConvertToHTML_Ledger_DKCTGS(ledgers, total, param.FromMonth, param.ToMonth, param.FromDate, param.ToDate, param.LedgerReportMaker, param.isCheckName, year);
                    break;

                case "excel":
                    _path = ExportExcel_Report_Ledger_DKCTGS(ledgers, total, param.LedgerReportMaker, param.isCheckName);
                    break;

                case "pdf":
                    _path = ConvertToPDFFile_Ledger_DKCTGS(ledgers, total, param.FromMonth, param.ToMonth, param.FromDate, param.ToDate, param.LedgerReportMaker, param.isCheckName, year);
                    break;
            }
            return _path;
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ConvertToHTML_Ledger_DKCTGS(IDictionary<string, List<DangKyChungTuGhiSo>> p, TotalAmountDKCTGS total, int? fromMonth, int? toMonth, DateTime? fromDate, DateTime? toDate, string preparedBy, bool isFillName, int year)
    {
        try
        {
            if (p == null) return string.Empty;
            if (fromMonth == null) fromMonth = 0;
            if (toMonth == null) toMonth = 0;
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;

            StringBuilder resultHTMLBuilder = new StringBuilder();
            string _template = "SoDangKyChungTuGhiSoTemplate.html",
                _folderPath = @"Uploads\Html",
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = System.IO.File.ReadAllText(path), resultHTML = string.Empty,
                _month = ((fromMonth > 0 && toMonth > 0) ? ((int)fromMonth).ToString("D2") + "/" + DateTime.Now.Year.ToString() : ((DateTime)fromDate).ToString("MM/yyyy"));
            IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TenCongTy", total.Company },
                { "DiaChi",total.Address },
                { "MST", total.TaxId },
                { "NgayChungTu", string.Empty },
                { "TuThang", ((fromMonth > 0 && toMonth > 0) ? ((int)fromMonth) : ((DateTime)fromDate).Month ).ToString("D2")   },
                { "DenThang", ( (fromMonth > 0 && toMonth > 0) ? ((int)toMonth) : ((DateTime)toDate).Month ).ToString("D2") },
                { "Nam", ((fromMonth > 0 && toMonth > 0) ? year : ((DateTime)fromDate).Year ).ToString("D4") },
                { "NguoiLap", !string.IsNullOrEmpty(preparedBy) ? preparedBy : string.Empty },
                { "Ngay", " ..... " },
                { "Thang", _month },
                { "NamSign", " ..... " },
                { "KeToanTruong", isFillName ? total.ChiefAccountName : string.Empty },
                { "GiamDoc", isFillName ? total.CEOName : string.Empty  },
                { "KeToanTruong_CV", total.ChiefAccountNote},
                { "GiamDoc_CV", total.CEONote},

                //last row sum
                { "CONG_THANG", total.Title_CT},
                { "TONG_TIEN_CONG_THANG", String.Format("{0:N0}", total.Sum_CT) },
                { "LUY_KE_TU_DAU_NAM", total.Title_LKDN },
                { "TONG_TIEN_LKTDN", String.Format("{0:N0}", total.Sum_LKDN) }
            };

            v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));

            string html_MonthTotal = @"<tr><td></td><td colspan='5'>{{{MONTH_TOTAL}}}</td></tr>", each_MT = string.Empty,
                each_htmlR = string.Empty, html_EachRow = @"<tr>
                                        <td>{{{STT}}}</td>
                                        <td>{{{LOAI_CHUNG_TU}}}</td>
                                        <td>{{{SO_CTGS}}}</td>
                                        <td>{{{NGAY_CT}}}</td>
                                        <td class='txt-right'>{{{AMOUNT}}}</td>
                                        <td>{{{NOTE}}}</td>
                                    </tr>";
            int _stt = 0;
            foreach (string key in p.Keys)
            {
                each_MT = html_MonthTotal.Replace("{{{MONTH_TOTAL}}}", "Tháng " + key);
                resultHTMLBuilder.Append(each_MT);
                p[key].ForEach(x =>
                {
                    _stt++;
                    each_htmlR = html_EachRow.Replace("{{{STT}}}", _stt.ToString())
                                    .Replace("{{{LOAI_CHUNG_TU}}}", x.Type)
                                    .Replace("{{{SO_CTGS}}}", x.VoucherNumber)
                                    .Replace("{{{NGAY_CT}}}", x.OrginalBookDate.ToString("dd/MM/yyyy"))
                                    .Replace("{{{AMOUNT}}}", String.Format("{0:N0}", x.Amount))
                                    .Replace("{{{NOTE}}}", x.OrginalDescription);
                    resultHTMLBuilder.Append(each_htmlR);
                });
            }

            resultHTML = resultHTMLBuilder.ToString();
            _allText = _allText.Replace("##REPLACE_PLACE##", resultHTML);

            return _allText;
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ConvertToPDFFile_Ledger_DKCTGS(IDictionary<string, List<DangKyChungTuGhiSo>> p, TotalAmountDKCTGS d, int? fromMonth, int? toMonth, DateTime? fromDate, DateTime? toDate, string preparedBy, bool isFillName, int year)
    {
        try
        {
            string _allText = ConvertToHTML_Ledger_DKCTGS(p, d, fromMonth, toMonth, fromDate, toDate, preparedBy, isFillName, year);
            return ExcelHelpers.ConvertUseDink(_allText, _converterPDF, Directory.GetCurrentDirectory(), "SoDangKyChungTuGhiSo");
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ExportExcel_Report_Ledger_DKCTGS(IDictionary<string, List<DangKyChungTuGhiSo>> p, TotalAmountDKCTGS d, string preparedBy, bool isFillName)
    {
        try
        {
            if (p == null) return string.Empty;

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ExcelPackage package = new ExcelPackage();
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheetName");

            //A => J
            worksheet.Cells["A1:F1"].Value = d.Company;
            worksheet.Cells["A2:F2"].Value = d.Address;
            worksheet.Cells["A3:F3"].Value = d.TaxId;
            worksheet.Cells["A1:F1"].Merge = true;
            worksheet.Cells["A2:F2"].Merge = true;
            worksheet.Cells["A3:F3"].Merge = true;

            worksheet.Cells["A5:F5"].Merge = true;

            worksheet.Cells["A5:F5"].Merge = true;
            worksheet.Cells["A5:F5"].Value = "SỔ ĐĂNG KÝ CHỨNG TỪ GHI SỔ";

            worksheet.Cells["A6:F6"].Merge = true;
            worksheet.Cells["A6:F6"].Value = "Từ tháng ... đến tháng ... năm ... ";

            worksheet.Cells["A7:C7"].Merge = true;
            worksheet.Cells["A7:C7"].Value = "Lũy kế đầu năm:";

            worksheet.Cells["D7:F7"].Merge = true;
            worksheet.Cells["D7:F7"].Value = "Đơn vị tính: Đồng";

            //table
            worksheet.Cells["A9"].Value = "STT"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["A9"]);
            worksheet.Cells["B9"].Value = "LOẠI CHỨNG TỪ"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["B9"]);
            worksheet.Cells["C9"].Value = "SỐ CTGS"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["C9"]);
            worksheet.Cells["D9"].Value = "NGÀY"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["D9"]);
            worksheet.Cells["E9"].Value = "SỐ TIỀN"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["E9"]);
            worksheet.Cells["F9"].Value = "GHI CHÚ"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["F9"]);

            int currentRowNo = 9, flagRowNo = 0;

            foreach (string key in p.Keys)
            {
                currentRowNo++;
                worksheet.Cells[currentRowNo, 2, currentRowNo, 6].Merge = true;
                worksheet.Cells[currentRowNo, 2, currentRowNo, 6].Value = "Tháng " + key;
                worksheet.Cells[currentRowNo, 2, currentRowNo, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                worksheet.Cells[currentRowNo, 2, currentRowNo, 6].Style.Font.Bold = true;

                p[key].ForEach(x =>
                {
                    flagRowNo++;
                    currentRowNo++;
                    worksheet.Cells[currentRowNo, 1].Value = flagRowNo.ToString();
                    worksheet.Cells[currentRowNo, 2].Value = x.Type;
                    worksheet.Cells[currentRowNo, 3].Value = x.VoucherNumber;
                    worksheet.Cells[currentRowNo, 4].Value = x.OrginalBookDate.ToString("dd/MM/yyyy");
                    worksheet.Cells[currentRowNo, 5].Value = String.Format("{0:N0}", x.Amount);
                    worksheet.Cells[currentRowNo, 6].Value = x.OrginalDescription;
                });
            }

            currentRowNo++;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 4].Merge = true;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 4].Value = d.Title_CT;
            worksheet.Cells[currentRowNo, 5].Value = String.Format("{0:N0}", d.Sum_CT);
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells[currentRowNo, 1, currentRowNo, 4]);
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells[currentRowNo, 5]);

            currentRowNo++;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 4].Merge = true;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 4].Value = d.Title_LKDN;
            worksheet.Cells[currentRowNo, 5].Value = String.Format("{0:N0}", d.Sum_LKDN);
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells[currentRowNo, 1, currentRowNo, 4]);
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells[currentRowNo, 5]);

            worksheet.SelectedRange["A10:F" + currentRowNo].Style.Font.Bold = false;

            worksheet.SelectedRange[10, 1, currentRowNo, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.SelectedRange[10, 5, currentRowNo, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            worksheet.SelectedRange[10, 6, currentRowNo, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

            currentRowNo += 2;
            worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Merge = true;
            worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Value = "Ngày ... tháng ... năm ...";
            worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            currentRowNo++;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 2].Merge = true;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 2].Value = "Người ghi sổ";

            worksheet.Cells[currentRowNo, 3, currentRowNo, 4].Merge = true;
            worksheet.Cells[currentRowNo, 3, currentRowNo, 4].Value = d.ChiefAccountNote;

            worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Merge = true;
            worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Value = d.CEONote;

            worksheet.Cells[currentRowNo - 2, 1, currentRowNo, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells[currentRowNo - 2, 1, currentRowNo, 6].Style.Font.Bold = true;

            currentRowNo += 2;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 2].Merge = true;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 2].Value = !string.IsNullOrEmpty(preparedBy) ? preparedBy : string.Empty;

            worksheet.Cells[currentRowNo, 3, currentRowNo, 4].Merge = true;
            worksheet.Cells[currentRowNo, 3, currentRowNo, 4].Value = isFillName ? d.ChiefAccountName : string.Empty;

            worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Merge = true;
            worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Value = isFillName ? d.CEOName : string.Empty;

            worksheet.Column(1).AutoFit(25);
            worksheet.Column(2).AutoFit(30);
            worksheet.Column(3).AutoFit(15);
            worksheet.Column(4).AutoFit(15);
            worksheet.Column(5).AutoFit(15);
            worksheet.Column(6).AutoFit(15);

            worksheet.SelectedRange["A10:F" + currentRowNo].Style.Font.Size = 12;

            worksheet.SelectedRange["A1:F3"].Style.Font.Size = 12;
            worksheet.SelectedRange["A6:F6"].Style.Font.Size = 12;

            worksheet.SelectedRange["A5:F5"].Style.Font.Size = 16;
            worksheet.SelectedRange["A5:F5"].Style.Font.Bold = true;
            worksheet.SelectedRange["A7:F7"].Style.Font.Bold = true;
            worksheet.Row(5).Height = 30;

            worksheet.SelectedRange["A9:F9"].Style.Font.Bold = true;
            worksheet.SelectedRange["A9:F9"].Style.Font.Size = 14;

            worksheet.Cells["A9:F9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.SelectedRange["A9:F9"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(252, 213, 180));
            worksheet.SelectedRange["A9:F9"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            worksheet.SelectedRange["A1:F" + currentRowNo].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            worksheet.SelectedRange["A4:F" + currentRowNo].Style.WrapText = true;

            worksheet.SelectedRange["A5:F8"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "SoDangKyChungTuGhiSo");
        }
        catch
        {
            return string.Empty;
        }
    }

}
