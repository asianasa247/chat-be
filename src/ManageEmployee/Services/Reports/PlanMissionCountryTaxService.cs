using DinkToPdf.Contracts;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Text;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Reports;
using ManageEmployee.Services.Interfaces.Accounts;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.Services.Reports;

public class PlanMissionCountryTaxService : IPlanMissionCountryTaxService
{
    private readonly IConverter converterPDF;
    private readonly ICompanyService _companyService;
    private readonly IAccountBalanceSheetService _accountBalanceSheetService;

    public PlanMissionCountryTaxService(
        IConverter _converPDF, ICompanyService companyService, IAccountBalanceSheetService accountBalanceSheetService)
    {
        converterPDF = _converPDF;
        _companyService = companyService;
        _accountBalanceSheetService = accountBalanceSheetService;
    }

    public async Task<string> ExportDataReport(PlanMissionCountryTaxParam request, bool isNoiBo, int year)
    {
        try
        {
            DateTime dtFrom = new DateTime(), dtTo = new DateTime();
            if (request == null) return string.Empty;
            if (request.FromMonth > 0) dtFrom = new DateTime(DateTime.Now.Year, (int)request.FromMonth, 1);
            if (request.ToMonth > 0) dtFrom = new DateTime(DateTime.Now.Year, (int)request.ToMonth, 1);
            if (request.FromDate != null) dtFrom = (DateTime)request.FromDate;
            if (request.ToDate != null) dtTo = (DateTime)request.ToDate;

            var company = await _companyService.GetCompany();

            var modelCompanyInfo = new VoucherReportViewModel()
            {
                Company = company.Name,
                Address = company.Address,
                TaxId = company.MST,
                Type = request.VoucherType,
                TypeName = request.VoucherType,
                ChiefAccountantName = company.NameOfChiefAccountant,
                NoteChiefAccountantName = company.NoteOfChiefAccountant,
                CEOOfName = company.NameOfCEO,
                CEOOfNote = company.NoteOfCEO,
                VoteMaker = request.VoteMaker
            };

            List<PlanMissionCountryTaxModelBase> _lstModelBaseReport = GetModelOnjectReport();

            List<AccountBalanceItemModel> _lstAccount = await _accountBalanceSheetService
                       .GenerateReport(dtFrom, dtTo, year, isNoiBo: isNoiBo);

            foreach (PlanMissionCountryTaxModelBase p in _lstModelBaseReport)
            {
                if (!string.IsNullOrEmpty(p.accountcode))
                {
                    AccountBalanceItemModel _obj = _lstAccount.Find(x => x.AccountCode == p.accountcode);
                    if (_obj != null)
                    {
                        if (!isNoiBo)
                            p.soconphainopdauky = _obj.OpeningCredit - _obj.OpeningDebit;
                        else
                            p.soconphainopdauky = _obj.OpeningCreditNB - _obj.OpeningDebitNB;
                        p.sopstk_phainop = _obj.ArisingCredit - _obj.ArisingDebit;
                        p.sopstk_danop = 0;
                        p.luykedn_phainop = _obj.CumulativeCredit - _obj.CumulativeDebit;
                        p.luykedn_danop = 0;
                        p.soconphainopcuoiky = _obj.ClosingCredit - _obj.ClosingDebit;
                    }
                }
            }

            _lstModelBaseReport.Where(x => x.rowtype.HasValue && string.IsNullOrEmpty(x.accountcode) ? x.rowtype.Value == 0 : false)
                .ToList()
                .ForEach(x =>
                {
                    var _tmp = _lstModelBaseReport.Where(_k => int.Parse(_k.code) > 10 && int.Parse(_k.code) < 21).ToList();

                    x.soconphainopdauky = _tmp.Sum(y => y.soconphainopdauky);
                    x.sopstk_phainop = _tmp.Sum(y => y.sopstk_phainop);
                    x.sopstk_danop = _tmp.Sum(y => y.sopstk_danop);
                    x.luykedn_phainop = _tmp.Sum(y => y.luykedn_phainop);
                    x.luykedn_danop = _tmp.Sum(y => y.luykedn_danop);
                    x.soconphainopcuoiky = _tmp.Sum(y => y.soconphainopcuoiky);
                });

            string _path = string.Empty;
            switch (request.FileType)
            {
                case "html":
                    _path = ConvertToHTML(_lstModelBaseReport, modelCompanyInfo, request.FromMonth, request.ToMonth, request.FromDate, request.ToDate, request.VoucherType, request.VoteMaker, request.isCheckName);
                    break;

                case "excel":
                    _path = ExportExcel_Report(_lstModelBaseReport, modelCompanyInfo, request.FromMonth, request.ToMonth, request.VoteMaker, request.isCheckName);
                    break;

                case "pdf":
                    _path = ConvertToPDFFile(_lstModelBaseReport, modelCompanyInfo, request.FromMonth, request.ToMonth, request.FromDate, request.ToDate, request.VoucherType, request.VoteMaker, request.isCheckName);
                    break;
            }
            return _path;
        }
        catch
        {
            return string.Empty;
        }
    }

    #region CONVERT DATA TO FILE

    private string ConvertToHTML(List<PlanMissionCountryTaxModelBase> _modelRequest, VoucherReportViewModel p, int? fromMonth, int? toMonth, DateTime? fromDate, DateTime? toDate, string voucherType, string preparedBy, bool isFillName)
    {
        try
        {
            if (p == null) return string.Empty;
            if (fromMonth == null) fromMonth = 0;
            if (toMonth == null) toMonth = 0;
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;

            string _template = "TinhHinhThucHienNghiaVuVoiNhaNuocTemplate.html",
                _folderPath = @"Uploads\Html", resultStr = string.Empty,
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = System.IO.File.ReadAllText(path);
            IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TenCongTy", p.Company },
                { "DiaChi", p.Address },
                { "MST", p.TaxId },
                { "LoaiChungTu", p.Type+" - "+ p.TypeName },
                { "SoCT", string.Empty },
                { "MaxNgay", string.Empty },
                { "TuThang", ((fromMonth > 0 && toMonth > 0) ? ((int)fromMonth) : ((DateTime)fromDate).Month ).ToString("D2")   },
                { "DenThang", ( (fromMonth > 0 && toMonth > 0) ? ((int)toMonth) : ((DateTime)toDate).Month ).ToString("D2") },
                { "Nam", ((fromMonth > 0 && toMonth > 0) ? DateTime.Now.Year : ((DateTime)fromDate).Year ).ToString("D4") },
                { "NguoiLap", !string.IsNullOrEmpty(preparedBy) ? preparedBy : string.Empty },
                { "KeToanTruong", isFillName ? p.ChiefAccountantName : string.Empty },
                { "GiamDoc", isFillName ? p.CEOOfName : string.Empty },
                { "Ngay", "......" },
                { "Thang", "......" },
                { "NamSign", "......" },
                { "KeToanTruong_CV", p.NoteChiefAccountantName},
                { "GiamDoc_CV", p.CEOOfNote},
            };

            v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));

            if (_modelRequest.Count > 0)
            {
                int _count = 0;
                _modelRequest.ForEach(x =>
                {
                    _count++;

                    string _debitHtml = @"<tr>
                                                    <td colspan='2' class='txt-left'>{{{CHI_TIEU}}}</td>
                                                    <td class='txt-left'>{{{MA_SO}}}</td>
                                                    <td class='txt-left'>{{{TAI_KHOAN}}}</td>
                                                    <td class='txt-right'>{{{SO_CON_DAU_KY_PHAI_NOP}}}</td>
                                                    <td class='txt-right'>{{{SO_PSTK_PHAI_NOP}}}</td>
                                                    <td class='txt-right'>{{{SO_PSTK_DA_NOP}}}</td>
                                                    <td class='txt-right'>{{{SO_LK_PHAI_NOP}}}</td>
                                                    <td class='txt-right'>{{{SO_LK_DA_NOP}}}</td>
                                                    <td class='txt-right'>{{{SO_CON_CUOI_KY_PHAI_NOP}}}</td>
                                                </tr>";
                    _debitHtml = _debitHtml
                        .Replace("{{{CHI_TIEU}}}", x.title)
                        .Replace("{{{MA_SO}}}", x.code)
                        .Replace("{{{TAI_KHOAN}}}", x.accountcode)
                        .Replace("{{{SO_CON_DAU_KY_PHAI_NOP}}}", x.soconphainopdauky > 0 ? String.Format("{0:N0}", x.soconphainopdauky) : string.Empty)
                        .Replace("{{{SO_PSTK_PHAI_NOP}}}", x.sopstk_phainop > 0 ? String.Format("{0:N0}", x.sopstk_phainop) : string.Empty)
                        .Replace("{{{SO_PSTK_DA_NOP}}}", x.sopstk_danop > 0 ? String.Format("{0:N0}", x.sopstk_danop) : string.Empty)
                        .Replace("{{{SO_LK_PHAI_NOP}}}", x.luykedn_phainop > 0 ? String.Format("{0:N0}", x.luykedn_phainop) : string.Empty)
                        .Replace("{{{SO_LK_DA_NOP}}}", x.luykedn_danop > 0 ? String.Format("{0:N0}", x.luykedn_danop) : string.Empty)
                        .Replace("{{{SO_CON_CUOI_KY_PHAI_NOP}}}", x.soconphainopcuoiky > 0 ? String.Format("{0:N0}", x.soconphainopcuoiky) : string.Empty);

                    resultStr += _debitHtml;
                });
            }

            _allText = _allText
                .Replace("##REPLACE_PLACE##", resultStr)
                .Replace("#ROW_SPAN#", "0");

            return _allText;
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ConvertToPDFFile(List<PlanMissionCountryTaxModelBase> _modelRequest, VoucherReportViewModel p, int? fromMonth, int? toMonth, DateTime? fromDate, DateTime? toDate, string voucherType, string preparedBy, bool isFillName)
    {
        try
        {
            string _allText = ConvertToHTML(_modelRequest, p, fromMonth, toMonth, fromDate, toDate, voucherType, preparedBy, isFillName);
            return ExcelHelpers.ConvertUseDink(_allText, converterPDF, Directory.GetCurrentDirectory(), "TinhHinhThucHienNghiaVuThue");
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ExportExcel_Report(List<PlanMissionCountryTaxModelBase> _modelRequest, VoucherReportViewModel p, int? fromMonth, int? toMonth, string preparedBy, bool isFillName)
    {
        try
        {
            if (p == null) return string.Empty;
            if (fromMonth == null) fromMonth = 0;
            if (toMonth == null) toMonth = 0;

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            ExcelPackage package = new ExcelPackage();
            ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheetName");

            //A => J

            worksheet.Cells["A1:I1"].Merge = true;
            worksheet.Cells["A2:I2"].Merge = true;
            worksheet.Cells["A3:I3"].Merge = true;
            worksheet.Cells["A4:I4"].Merge = true;

            worksheet.Cells["A1:I1"].Value = p.Company;
            worksheet.Cells["A2:I2"].Value = p.Address;
            worksheet.Cells["A3:I3"].Value = p.TaxId;

            worksheet.Cells["A5:I5"].Merge = true;
            worksheet.Cells["A5:I5"].Value = "TÌNH HÌNH THỰC HIỆN NGHĨA VỤ VỚI NGÂN SÁCH NHÀ NƯỚC";

            worksheet.Cells["A6:I6"].Merge = true;
            worksheet.Cells["A6:I6"].Value = "";

            worksheet.Cells["A7:I7"].Merge = true;
            worksheet.Cells["A7:I7"].Value = $"Từ tháng {fromMonth} đến tháng {toMonth}";

            //table

            worksheet.Cells["A8:A9"].Merge = true; worksheet.Cells["A8:A9"].Value = "CHỈ TIÊU";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["A8:A9"]);

            worksheet.Cells["B8:B9"].Merge = true; worksheet.Cells["B8:B9"].Value = "MÃ SỐ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["B8:B9"]);

            worksheet.Cells["C8:C9"].Merge = true; worksheet.Cells["C8:C9"].Value = "TÀI KHOẢN";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["C8:C9"]);

            worksheet.Cells["D8:D9"].Merge = true; worksheet.Cells["D8:D9"].Value = "SỐ CÒN PHẢI NỘP ĐẦU KỲ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["D8:D9"]);

            worksheet.Cells["E8:F8"].Merge = true; worksheet.Cells["E8:F8"].Value = "SỐ PHÁT SINH TRONG KỲ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["E8:F8"]);

            worksheet.Cells["E9"].Value = "SỐ PHẢI NỘP";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["E9"]);

            worksheet.Cells["F9"].Value = "SỐ ĐÃ NỘP";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["F9"]);

            worksheet.Cells["G8:H8"].Merge = true; worksheet.Cells["G8:H8"].Value = "LŨY KẾ TỪ ĐẦU NĂM";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["G8:H8"]);

            worksheet.Cells["G9"].Value = "SỐ PHẢI NỘP";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["G9"]);

            worksheet.Cells["H9"].Value = "SỐ ĐÃ NỘP";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["H9"]);

            worksheet.Cells["I8:I9"].Merge = true; worksheet.Cells["I8:I9"].Value = "SỐ CÒN PHẢI NỘP CUỐI KỲ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["I8:I9"]);

            int currentRowNo = 9;

            for (int i = 0; i < _modelRequest.Count; i++)
            {
                currentRowNo++;

                PlanMissionCountryTaxModelBase _mBase = _modelRequest[i];
                worksheet.Cells[currentRowNo, 1].Value = _mBase.title;
                worksheet.Cells[currentRowNo, 2].Value = _mBase.code;
                worksheet.Cells[currentRowNo, 3].Value = _mBase.accountcode;
                worksheet.Cells[currentRowNo, 4].Value = _mBase.soconphainopdauky > 0 ? String.Format("{0:N0}", _mBase.soconphainopdauky) : string.Empty;
                worksheet.Cells[currentRowNo, 5].Value = _mBase.sopstk_phainop > 0 ? String.Format("{0:N0}", _mBase.sopstk_phainop) : string.Empty;
                worksheet.Cells[currentRowNo, 6].Value = _mBase.sopstk_danop > 0 ? String.Format("{0:N0}", _mBase.sopstk_danop) : string.Empty;
                worksheet.Cells[currentRowNo, 7].Value = _mBase.luykedn_phainop > 0 ? String.Format("{0:N0}", _mBase.luykedn_phainop) : string.Empty;
                worksheet.Cells[currentRowNo, 8].Value = _mBase.luykedn_danop > 0 ? String.Format("{0:N0}", _mBase.luykedn_danop) : string.Empty;
                worksheet.Cells[currentRowNo, 9].Value = _mBase.soconphainopcuoiky > 0 ? String.Format("{0:N0}", _mBase.soconphainopcuoiky) : string.Empty;
            }

            currentRowNo += 2;
            worksheet.Cells[currentRowNo, 6, currentRowNo, 9].Merge = true;
            worksheet.Cells[currentRowNo, 6, currentRowNo, 9].Value = "Ngày ..... tháng ..... năm ..... ";
            worksheet.Cells[currentRowNo, 6, currentRowNo, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            currentRowNo++;

            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Merge = true;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Value = "Người lập phiếu";
            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[currentRowNo, 4, currentRowNo, 6].Merge = true;
            worksheet.Cells[currentRowNo, 4, currentRowNo, 6].Value = p.NoteChiefAccountantName;
            worksheet.Cells[currentRowNo, 4, currentRowNo, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[currentRowNo, 7, currentRowNo, 9].Merge = true;
            worksheet.Cells[currentRowNo, 7, currentRowNo, 9].Value = p.CEOOfNote;
            worksheet.Cells[currentRowNo, 7, currentRowNo, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            currentRowNo += 4;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Merge = true;
            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Value = (!string.IsNullOrEmpty(preparedBy) ? preparedBy : string.Empty);
            worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells[currentRowNo, 4, currentRowNo, 6].Merge = true;
            worksheet.Cells[currentRowNo, 4, currentRowNo, 6].Value = isFillName ? p.ChiefAccountantName : "";
            worksheet.Cells[currentRowNo, 4, currentRowNo, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[currentRowNo, 7, currentRowNo, 9].Merge = true;
            worksheet.Cells[currentRowNo, 7, currentRowNo, 9].Value = isFillName ? p.CEOOfName : "";
            worksheet.Cells[currentRowNo, 7, currentRowNo, 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Column(1).AutoFit(25);
            worksheet.Column(2).AutoFit(30);
            worksheet.Column(3).AutoFit(15);
            worksheet.Column(4).AutoFit(15);
            worksheet.Column(5).AutoFit(15);
            worksheet.Column(6).AutoFit(15);
            worksheet.Column(7).AutoFit(15);
            worksheet.Column(8).AutoFit(15);
            worksheet.Column(9).AutoFit(15);

            worksheet.SelectedRange["A1:I3"].Style.Font.Size = 12;
            worksheet.SelectedRange["A6:I6"].Style.Font.Size = 12;
            worksheet.SelectedRange["H5"].Style.Font.Size = 12;

            worksheet.SelectedRange["A5:I5"].Style.Font.Size = 16;
            worksheet.SelectedRange["A5:I5"].Style.Font.Bold = true;
            worksheet.Row(5).Height = 30;

            worksheet.SelectedRange["A8:I9"].Style.Font.Bold = true;
            worksheet.SelectedRange["A8:I9"].Style.Font.Size = 14;

            worksheet.Cells["A8:I9"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.SelectedRange["A8:I9"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(252, 213, 180));

            worksheet.SelectedRange["A10:I" + currentRowNo].Style.Font.Bold = false;
            worksheet.SelectedRange["A10:I" + currentRowNo].Style.Font.Size = 12;

            worksheet.SelectedRange["A1:I" + currentRowNo].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
            worksheet.SelectedRange["A3:I" + currentRowNo].Style.WrapText = true;

            worksheet.SelectedRange["A8:I9"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells["A8:I9"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            worksheet.Cells["A8:I9"].Style.Font.Bold = true;

            currentRowNo -= 3;
            worksheet.SelectedRange["A10:B" + currentRowNo].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

            worksheet.SelectedRange["D10:I" + currentRowNo].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
            worksheet.SelectedRange["A5:I7"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

            return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "TinhHinhThucHienNghiaVuVoiNhaNuoc");
        }
        catch
        {
            return string.Empty;
        }
    }

    #endregion CONVERT DATA TO FILE

    #region ULITIES

    /// <summary>
    /// Get model base report from json file
    /// </summary>
    /// <returns></returns>
    private List<PlanMissionCountryTaxModelBase> GetModelOnjectReport()
    {
        try
        {
            string _template = "NghiaVuVoiNhaNuoc.json",
                _folderPath = @"Uploads\Json",
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = System.IO.File.ReadAllText(path);
            List<PlanMissionCountryTaxModelBase> _objBase = JsonConvert.DeserializeObject<List<PlanMissionCountryTaxModelBase>>(_allText);
            SetDefaultInit(_objBase);
            return _objBase;
        }
        catch
        {
            return new List<PlanMissionCountryTaxModelBase>();
        }
    }

    private void SetDefaultInit(List<PlanMissionCountryTaxModelBase> p)
    {
        foreach (PlanMissionCountryTaxModelBase k in p)
        {
            k.soconphainopdauky = 0;
            k.sopstk_phainop = 0;
            k.sopstk_danop = 0;
            k.luykedn_phainop = 0;
            k.luykedn_danop = 0;
            k.soconphainopcuoiky = 0;
        }
    }

    #endregion ULITIES
}