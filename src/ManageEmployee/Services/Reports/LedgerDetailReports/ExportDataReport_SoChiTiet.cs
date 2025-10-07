using Common.Constants;
using DinkToPdf.Contracts;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Accounts;
using ManageEmployee.Services.Interfaces.Reports;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Text;

namespace ManageEmployee.Services.Reports.LedgerDetailReports
{
    public class ExportDataReport_SoChiTiet: IExportDataReport_SoChiTiet
    {
        private readonly IAccountBalanceSheetService _accountBalanceSheet;
        private readonly ApplicationDbContext _context;
        private readonly IConverter _converterPDF;

        public ExportDataReport_SoChiTiet(ApplicationDbContext context,
            IAccountBalanceSheetService accountBalanceSheet, IConverter converterPDF)
        {
            _context = context;
            _accountBalanceSheet = accountBalanceSheet;
            _converterPDF = converterPDF;
        }
        public async Task<string> ReportAsync(LedgerReportModel ledgers, LedgerReportParamDetail param, int year)
        {
            try
            {
                var _accountGet = await _accountBalanceSheet.
                    GenerateAccrualAccounting("date", param.FromDate, param.ToDate, param.AccountCode, param.AccountCodeDetail1, param.AccountCodeDetail2, param.IsNoiBo);

                string _path = string.Empty;
                switch (param.FileType)
                {
                    case "html":
                        _path = ConvertToHTML_SoChiTiet_PhanMau(ledgers, param, _accountGet.OpeningStock, year);
                        break;

                    case "excel":
                        _path = ExportExcel_Report_SoChiTiet_PhanMau(ledgers, param, _accountGet.OpeningStock);
                        break;

                    case "pdf":
                        _path = ConvertToPDFFile_SoChiTiet(ledgers, param, _accountGet.OpeningStock, year);
                        break;
                }
                return _path;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string ConvertToHTML_SoChiTiet_PhanMau(LedgerReportModel p, LedgerReportParamDetail param, double openingStock, int year)
        {
            try
            {
                string _html = string.Empty;
                switch (param.BookDetailType)
                {
                    case (int)ReportBookDetailTypeEnum.soCoDu_1_ben:
                        _html = ConvertToHTML_SoChiTiet_Loai_1(p, param, openingStock, year);
                        break;

                    case (int)ReportBookDetailTypeEnum.soCoDu_2_ben:
                        _html = ConvertToHTML_SoChiTiet_And_2(p, param, openingStock, year);
                        break;

                    case (int)ReportBookDetailTypeEnum.soCoNgoaiTe:
                        _html = ConvertToHTML_SoChiTiet_Loai_3(p, param, openingStock, year);
                        break;

                    case (int)ReportBookDetailTypeEnum.soCoHangTonKho:
                        _html = ConvertToHTML_SoChiTiet_Loai_4(p, param, year);
                        break;

                    case (int)ReportBookDetailTypeEnum.soQuy:
                        _html = ConvertToHTML_SoChiTiet_Loai_5(p, param, openingStock, year);
                        break;

                    case (int)ReportBookDetailTypeEnum.soSoLuongTonKho:
                        _html = ConvertToHTML_SoChiTiet_Loai_6(p, param, year);
                        break;

                    default:
                        break;
                }
                return _html;
            }
            catch
            {
                return string.Empty;
            }
        }


        private string ExportExcel_Report_SoChiTiet_PhanMau(LedgerReportModel p, LedgerReportParam param, double openingStock)
        {
            try
            {
                if (param.BookDetailType == (int)ReportBookDetailTypeEnum.soSoLuongTonKho)
                {
                    string _html6 = ExportExcel_Report_SoChiTiet_Loai_6(p, param);
                    return _html6;
                }
                else if (param.BookDetailType == (int)ReportBookDetailTypeEnum.soCoHangTonKho)
                {
                    string _html6 = ExportExcel_Report_SoChiTiet_Loai_4(p, param);
                    return _html6;
                }
                string _html = ExportExcel_Report_SoChiTiet_Loai_1_And_2(p, param, openingStock);

                return _html;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string ConvertToPDFFile_SoChiTiet(LedgerReportModel p, LedgerReportParamDetail param, double openingStock, int year)
        {
            try
            {
                string _allText = ConvertToHTML_SoChiTiet_PhanMau(p, param, openingStock, year);
                return ExcelHelpers.ConvertUseDinkLandscape(_allText, _converterPDF, Directory.GetCurrentDirectory(), "SoChiTiet");
            }
            catch
            {
                return string.Empty;
            }
        }

        private string ConvertToHTML_SoChiTiet_And_2(LedgerReportModel p, LedgerReportParamDetail param, double openingStock, int year)
        {
            try
            {
                if (p == null) return string.Empty;

                string _template = "SoChiTiet_TwoTemplate.html";
                string _folderPath = @"Uploads\Html";
                string path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template);
                string _allText = File.ReadAllText(path);

                IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TIEU_DE","SỔ CHI TIẾT "+ p.AccountName.ToUpper() },
                { "TenCongTy", p.Company },
                { "DiaChi", p.Address },
                { "MST", p.TaxId },
                { "NgayChungTu", string.Empty },
                { "TaiKhoanAccount", p.AccountCode +" : "+p.AccountName },
                { "TuThang", param.FromDate.Value.Month.ToString("D2")},
                { "DenThang", param.ToDate.Value.AddDays(-1).Month.ToString("D2") },
                { "Nam",  year.ToString("D4") },
                { "NguoiLap", !string.IsNullOrEmpty(param.LedgerReportMaker) ? param.LedgerReportMaker : string.Empty },
                { "Ngay", (param.ToDate.Value.Day).ToString("D2") },
                { "Thang",  (param.ToDate.Value.Month).ToString("D2") },
                { "NamSign",   year.ToString() },
                { "KeToanTruong", param.isCheckName ? p.ChiefAccountantName : string.Empty },
                { "GiamDoc", param.isCheckName ? p.CEOName : string.Empty },
                { "SO_DU_BEN", "(BÊN NỢ)" },
                { "LUY_KE_DAU_NAM", openingStock > 0 ? String.Format("{0:N0}", openingStock) : string.Empty},
                { "KeToanTruong_CV", p.ChiefAccountantNote},
                { "GiamDoc_CV", p.CEONote},
            };

                string soThapPhan = "N" + p.MethodCalcExportPrice;
                v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));
                int _ind = -1;

                var resultHTML = new StringBuilder(@"<tr class='font-b'>
                                                                <td colspan='6'>Lũy kế đầu năm</td>
                                                                <td></td>
                                                                <td></td>
                                                                <td class='txt-right'>{{LUY_KE_DAU_NAM_NO}}</td>
                                                                <td class='txt-right'>{{LUY_KE_DAU_NAM_CO}}</td>
                                                            </tr>");
                resultHTML.Replace("{{LUY_KE_DAU_NAM_NO}}", openingStock > 0 ? String.Format("{0:N0}", Math.Abs(openingStock)) : string.Empty);

                resultHTML.Replace("{{LUY_KE_DAU_NAM_CO}}", openingStock < 0 ? String.Format("{0:N0}", openingStock) : string.Empty);

                if (p.BookDetails.Count > 0)
                {
                    p.BookDetails.ForEach(x =>
                    {
                        _ind++;
                        var _txt = new StringBuilder(@"<tr>
                                            <td>{{{NGAY_GHI_SO}}}</td>
                                    <td><a href='{{{URL_LINK}}}#{{{FILTER_TYPE}}}#{{{FILTER_TEXT}}}#{{{FILTER_MONTH}}}#{{{FILTER_ISINTERNAL}}}' target='_blank'>{{{CHUNG_TU_SO}}}</a></td>

                                            <td>{{{CHUNG_TU_NGAY}}}</td>
                                            <td class='tbl-td-diengiai'>{{{DIEN_GIAI}}}</td>
                                            <td>{{{TK_DOI_UNG}}}</td>
                                            <td>{{{CHI_TIET}}}</td>
                                            <td class='txt-right'>{{{SO_TIEN_PS_NO}}}</td>
                                            <td class='txt-right'>{{{SO_TIEN_PS_CO}}}</td>
                                            <td class='txt-right'>{{{SO_TIEN_DU_NO}}}</td>
                                            <td class='txt-right'>{{{SO_TIEN_DU_CO}}}</td>
                                        </tr>");

                        _txt.Replace("{{{NGAY_GHI_SO}}}", x.OrginalBookDate.HasValue ? x.OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty)
                        .Replace("{{{URL_LINK}}}", UrlLinkConst.UrlLinkArise);

                        _txt.Replace("{{{CHUNG_TU_SO}}}", x.OrginalVoucherNumber);
                        _txt.Replace("{{{CHUNG_TU_NGAY}}}", x.OrginalBookDate.HasValue ? x.OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty);

                        _txt.Replace("{{{DIEN_GIAI}}}", x.Description);

                        _txt.Replace("{{{TK_DOI_UNG}}}", x.IsDebit ? x.CreditCode : x.DebitCode);
                        _txt.Replace("{{{CHI_TIET}}}", x.DetailCode);

                        _txt.Replace("{{{SO_TIEN_PS_NO}}}", x.ArisingDebit > 0 ? String.Format("{0:" + soThapPhan + "}", x.ArisingDebit) : string.Empty);
                        _txt.Replace("{{{SO_TIEN_PS_CO}}}", x.ArisingCredit > 0 ? String.Format("{0:" + soThapPhan + "}", x.ArisingCredit) : string.Empty);
                        _txt.Replace("{{{SO_TIEN_DU_NO}}}", x.ResidualDebit > 0 ? String.Format("{0:" + soThapPhan + "}", x.ResidualDebit) : string.Empty);
                        _txt.Replace("{{{SO_TIEN_DU_CO}}}", x.ResidualCredit > 0 ? String.Format("{0:" + soThapPhan + "}", x.ResidualCredit) : string.Empty);

                        _txt.Replace("{{{FILTER_TYPE}}}", x.VoucherNumber.Split('/')[1]);
                        _txt.Replace("{{{FILTER_MONTH}}}", x.Month.ToString());
                        _txt.Replace("{{{FILTER_ISINTERNAL}}}", (param.IsNoiBo ? "3" : "1"));
                        _txt.Replace("{{{FILTER_TEXT}}}", x.OrginalVoucherNumber);
                        ;

                        resultHTML.Append(_txt);

                        SoChiTietViewModel _sct = p.BookDetails[_ind + 1 < p.BookDetails.Count ? _ind + 1 : p.BookDetails.Count - 1];
                        if (!param.IsNewReport && ((_sct.Month + "" + _sct.Year) != (x.Month + "" + x.Year) && _ind < p.BookDetails.Count) || (_ind == p.BookDetails.Count - 1) && p.SumItem_SCT_ThuChi.ContainsKey(x.Temp))
                        {
                            List<SoChiTietThuChiViewModel> _ledgerSum = p.SumItem_SCT_ThuChi[x.Temp];

                            SoChiTietThuChiViewModel _CongPhatSinh = _ledgerSum.FirstOrDefault(x => x.Month == -1) ?? new SoChiTietThuChiViewModel();
                            SoChiTietThuChiViewModel _LuyKe = _ledgerSum.FirstOrDefault(x => x.Month == -2) ?? new SoChiTietThuChiViewModel();

                            var _sumRowMonthHTML = new StringBuilder(@"<tr class='font-b'>
                                                                <td colspan='4'>Cộng phát sinh tháng {{THANG_ROW}}</td>
                                                                <td></td>
                                                                <td></td>
                                                                <td class='txt-right'>{{SR_TONG_SO_TIEN_PS_NO_CONG}}</td>
                                                                <td class='txt-right'>{{SR_TONG_SO_TIEN_PS_CO_CONG}}</td>
                                                                <td class='txt-right'>{{SR_TONG_SO_TIEN_DU_NO_CONG}}</td>
                                                                <td class='txt-right'>{{SR_TONG_SO_TIEN_DU_CO_CONG}}</td>
                                                            </tr>
			                                                <tr class='font-b'>
                                                                <td colspan='4'>Lũy kế phát sinh đến cuối kỳ</td>
                                                                <td></td>
                                                                <td></td>
                                                                <td class='txt-right'>{{SR_TONG_SO_TIEN_PS_NO_LK}}</td>
                                                                <td class='txt-right'>{{SR_TONG_SO_TIEN_PS_CO_LK}}</td>
                                                                <td class='txt-right'>{{SR_TONG_SO_TIEN_DU_NO_LK}}</td>
                                                                <td class='txt-right'>{{SR_TONG_SO_TIEN_DU_CO_LK}}</td>
                                                            </tr>    ");

                            _sumRowMonthHTML.Replace("{{THANG_ROW}}", x.Month.ToString("00") + "/" + x.Year.ToString());

                            _sumRowMonthHTML.Replace("{{SR_TONG_SO_TIEN_PS_NO_CONG}}", _CongPhatSinh.Thu_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _CongPhatSinh.Thu_Amount) : string.Empty);
                            _sumRowMonthHTML.Replace("{{SR_TONG_SO_TIEN_PS_CO_CONG}}", _CongPhatSinh.Chi_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _CongPhatSinh.Chi_Amount) : string.Empty);
                            _sumRowMonthHTML.Replace("{{SR_TONG_SO_TIEN_DU_NO_CONG}}", _CongPhatSinh.Residual_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _CongPhatSinh.Residual_Amount) : string.Empty);
                            _sumRowMonthHTML.Replace("{{SR_TONG_SO_TIEN_DU_CO_CONG}}", _CongPhatSinh.Residual_Amount < 0 ? String.Format("{0:" + soThapPhan + "}", Math.Abs(_CongPhatSinh.Residual_Amount)) : string.Empty);

                            _sumRowMonthHTML.Replace("{{SR_TONG_SO_TIEN_PS_NO_LK}}", _LuyKe.Thu_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _LuyKe.Thu_Amount) : string.Empty);
                            _sumRowMonthHTML.Replace("{{SR_TONG_SO_TIEN_PS_CO_LK}}", _LuyKe.Chi_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _LuyKe.Chi_Amount) : string.Empty);
                            _sumRowMonthHTML.Replace("{{SR_TONG_SO_TIEN_DU_NO_LK}}", _LuyKe.Residual_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _LuyKe.Residual_Amount) : string.Empty);
                            _sumRowMonthHTML.Replace("{{SR_TONG_SO_TIEN_DU_CO_LK}}", _LuyKe.Residual_Amount < 0 ? String.Format("{0:" + soThapPhan + "}", Math.Abs(_LuyKe.Residual_Amount)) : string.Empty);

                            resultHTML.Append(_sumRowMonthHTML);
                        }
                    });
                }

                _allText = _allText.Replace("##REPLACE_PLACE##", resultHTML.ToString());

                return _allText;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string ConvertToHTML_SoChiTiet_Loai_1(LedgerReportModel p, LedgerReportParamDetail param, double openingStock, int year)
        {
            try
            {
                if (p == null) return string.Empty;

                string _template = "SoChiTiet_OneTemplate.html",
                    _folderPath = @"Uploads\Html",
                    path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                    _allText = System.IO.File.ReadAllText(path), resultHTML = string.Empty;
                IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TIEU_DE","SỔ CHI TIẾT "+ p.AccountName.ToUpper() },
                { "TenCongTy", p.Company },
                { "DiaChi", p.Address },
                { "MST", p.TaxId },
                { "NgayChungTu", string.Empty },
                { "TaiKhoanAccount", p.AccountCode +" : "+p.AccountName },
                { "TuThang", ((param.FromMonth > 0 && param.ToMonth > 0) ? ((int)param.FromMonth) : ((DateTime)param.FromDate).Month ).ToString("D2")   },
                { "DenThang", ( (param.FromMonth > 0 && param.ToMonth > 0) ? ((int)param.ToMonth) : ((DateTime)param.ToDate).Month ).ToString("D2") },
                { "Nam", ((param.FromMonth > 0 && param.ToMonth > 0) ? year : ((DateTime)param.FromDate).Year ).ToString("D4") },
                { "NguoiLap", !string.IsNullOrEmpty(param.LedgerReportMaker) ? param.LedgerReportMaker : string.Empty },
                { "Ngay", (param.ToDate.Value.Day).ToString("D2") },
                { "Thang",  (param.ToDate.Value.Month).ToString("D2") },
                { "NamSign",   year.ToString() },
                { "KeToanTruong", param.isCheckName ? p.ChiefAccountantName : string.Empty },
                { "GiamDoc", param.isCheckName ? p.CEOName : string.Empty },
                { "SO_DU_BEN", "(BÊN NỢ)" },
                { "LUY_KE_DAU_NAM", openingStock > 0 ? String.Format("{0:N0}", openingStock) : string.Empty},
                { "KeToanTruong_CV", p.ChiefAccountantNote},
                { "GiamDoc_CV", p.CEONote},
            };
                string soThapPhan = "N" + p.MethodCalcExportPrice;
                v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));
                int _ind = -1;

                resultHTML = @"<tr class='font-b'>
                                                                <td colspan='5'>Lũy kế đầu năm</td>
                                                                <td></td>
                                                                <td></td>
                                                                <td  colspan='3' class='txt-right'>{{LUY_KE_DAU_NAM}}</td>
                                                            </tr>";
                resultHTML = resultHTML.Replace("{{LUY_KE_DAU_NAM}}", openingStock > 0 ? String.Format("{0:N0}", openingStock) : string.Empty)
                                                         .Replace("{{LUY_KE_DAU_NAM}}", openingStock > 0 ? String.Format("{0:N0}", openingStock) : string.Empty);

                if (p.BookDetails.Count > 0)
                {
                    p.BookDetails.ForEach(x =>
                    {
                        _ind++;
                        string _txt = @"<tr>
                                            <td>{{{NGAY_GHI_SO}}}</td>
                                            <td><a href='{{{URL_LINK}}}#{{{FILTER_TYPE}}}#{{{FILTER_TEXT}}}#{{{FILTER_MONTH}}}#{{{FILTER_ISINTERNAL}}}' target='_blank'>{{{CHUNG_TU_SO}}}</a></td>

                                            <td>{{{CHUNG_TU_NGAY}}}</td>
                                            <td>{{{SO_HOA_DON}}}</td>
                                            <td class='tbl-td-diengiai'>{{{DIEN_GIAI}}}</td>
                                            <td>{{{TK_DOI_UNG}}}</td>
                                            <td>{{{CHI_TIET}}}</td>
                                            <td class='txt-right'>{{{SO_TIEN_PS_NO}}}</td>
                                            <td class='txt-right'>{{{SO_TIEN_PS_CO}}}</td>
                                            <td class='txt-right' colspan='2'>{{{SO_TIEN_DU}}}</td>
                                        </tr>";

                        _txt = _txt
                        .Replace("{{{NGAY_GHI_SO}}}", x.OrginalBookDate.HasValue ? x.OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty)
                        .Replace("{{{URL_LINK}}}", UrlLinkConst.UrlLinkArise)
                                            .Replace("{{{CHUNG_TU_SO}}}", x.OrginalVoucherNumber)
                                            .Replace("{{{CHUNG_TU_NGAY}}}", x.OrginalBookDate.HasValue ? x.OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty)
                                            .Replace("{{{SO_HOA_DON}}}", x.InvoiceNumber)
                                            .Replace("{{{DIEN_GIAI}}}", x.Description)
                                            .Replace("{{{TK_DOI_UNG}}}", x.IsDebit ? x.CreditCode : x.DebitCode)
                                            .Replace("{{{CHI_TIET}}}", x.DetailCode)
                                            .Replace("{{{SO_TIEN_PS_NO}}}", x.ArisingDebit > 0 ? String.Format("{0:" + soThapPhan + "}", x.ArisingDebit) : string.Empty)
                                            .Replace("{{{SO_TIEN_PS_CO}}}", x.ArisingCredit > 0 ? String.Format("{0:" + soThapPhan + "}", x.ArisingCredit) : string.Empty)
                                            .Replace("{{{SO_TIEN_DU}}}", x.ResidualDebit > 0 ? String.Format("{0:" + soThapPhan + "}", x.ResidualDebit) : String.Format("{0:N0}", x.ResidualCredit))

                                   .Replace("{{{FILTER_TEXT}}}", x.OrginalVoucherNumber)

                                            .Replace("{{{FILTER_TYPE}}}", x.VoucherNumber.Split('/')[1])
                                   .Replace("{{{FILTER_MONTH}}}", x.Month.ToString())
                                                                          .Replace("{{{FILTER_ISINTERNAL}}}", (param.IsNoiBo ? "3" : "1"))

    ;

                        resultHTML += _txt;

                        SoChiTietViewModel _sct = p.BookDetails[_ind + 1 < p.BookDetails.Count ? _ind + 1 : p.BookDetails.Count - 1];
                        if (!param.IsNewReport && ((_sct.Month + "" + _sct.Year) != (x.Month + "" + x.Year) && _ind < p.BookDetails.Count) || (_ind == p.BookDetails.Count - 1) && p.SumItem_SCT_ThuChi.ContainsKey(x.Temp))
                        {
                            List<SoChiTietThuChiViewModel> _ledgerSum = p.SumItem_SCT_ThuChi[x.Temp];

                            SoChiTietThuChiViewModel _CongPhatSinh = _ledgerSum.FirstOrDefault(x => x.Month == -1);
                            SoChiTietThuChiViewModel _LuyKe = _ledgerSum.FirstOrDefault(x => x.Month == -2);

                            string _sumRowMonthHTML = @"                                                        <tr class='font-b'>
                                                                <td colspan='5'>Cộng phát sinh tháng {{THANG_ROW}}</td>
                                                                <td></td>
                                                                <td></td>
                                                                <td class='txt-right'>{{SR_THU}}</td>
                                                                <td class='txt-right'>{{SR_CHI}}</td>
                                                                <td class='txt-right' colspan='2'>{{SR_TON}}</td>
                                                            </tr>
			                                                <tr class='font-b'>
                                                                <td colspan='5'>Lũy kế phát sinh đến cuối kỳ</td>
                                                                <td></td>
                                                                <td></td>
                                                                <td class='txt-right'>{{SR_LK_THU}}</td>
                                                                <td class='txt-right'>{{SR_LK_CHI}}</td>
                                                                <td class='txt-right' colspan='2'>{{SR_LK_TON}}</td>
                                                            </tr>";

                            _sumRowMonthHTML = _sumRowMonthHTML.Replace("{{THANG_ROW}}", x.Month.ToString("00") + "/" + x.Year.ToString())
                             .Replace("{{SR_THU}}", _CongPhatSinh?.Thu_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _CongPhatSinh?.Thu_Amount) : string.Empty)
                             .Replace("{{SR_CHI}}", _CongPhatSinh?.Chi_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _CongPhatSinh?.Chi_Amount) : string.Empty)
                             .Replace("{{SR_TON}}", _CongPhatSinh?.Residual_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _CongPhatSinh?.Residual_Amount) : string.Empty)

                             .Replace("{{SR_LK_THU}}", _LuyKe?.Thu_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _LuyKe?.Thu_Amount) : string.Empty)
                             .Replace("{{SR_LK_CHI}}", _LuyKe?.Chi_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _LuyKe?.Chi_Amount) : string.Empty)
                             .Replace("{{SR_LK_TON}}", _LuyKe?.Residual_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _LuyKe?.Residual_Amount) : string.Empty)
                            ;

                            resultHTML += _sumRowMonthHTML;
                        }
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

        private string ConvertToHTML_SoChiTiet_Loai_3(LedgerReportModel p, LedgerReportParamDetail param, double openingStock, int year)
        {
            try
            {
                if (p == null) return string.Empty;

                string _template = param.BookDetailType == 3 ? "SoChiTiet_ThreeTemplate.html" : "SoChiTiet_FourTemplate.html",
                    _folderPath = @"Uploads\Html",
                    path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                    _allText = System.IO.File.ReadAllText(path), resultHTML = string.Empty;
                IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TIEU_DE","SỔ CHI TIẾT "+ p.AccountName.ToUpper() },
                { "TenCongTy", p.Company },
                { "DiaChi", p.Address },
                { "MST", p.TaxId },
                { "NgayChungTu", string.Empty },
                { "TaiKhoanAccount", p.AccountCode+": "+p.AccountName },
                { "TuThang", param.FromDate.Value.Month.ToString("D2")   },
                { "DenThang", param.ToDate.Value.AddDays(-1).Month.ToString("D2") },
                { "Nam",  year.ToString("D4") },
                { "NguoiLap", !string.IsNullOrEmpty(param.LedgerReportMaker) ? param.LedgerReportMaker : string.Empty },
                { "Ngay", (param.ToDate.Value.Day).ToString("D2") },
                { "Thang",  (param.ToDate.Value.Month).ToString("D2") },
                { "NamSign",   year.ToString() },
                { "KeToanTruong", param.isCheckName ? p.ChiefAccountantName : string.Empty },
                { "GiamDoc", param.isCheckName ? p.CEOName : string.Empty },
                {"LUY_KE_DAU_NAM", openingStock > 0 ? String.Format("{0:N0}", openingStock) : string.Empty },

                { "KeToanTruong_CV", p.ChiefAccountantNote},
                { "GiamDoc_CV", p.CEONote},
            };
                string soThapPhan = "N" + p.MethodCalcExportPrice;
                v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));
                int _ind = -1;

                resultHTML = @"<tr class='font-b'>
                                                                <td colspan='5'>Lũy kế đầu năm</td>
                                                                <td></td>
                                                                <td></td>
                                                                <td></td>
                                                                <td  colspan='6' class='txt-right'>{{LUY_KE_DAU_NAM}}</td>
                                                            </tr>";
                resultHTML = resultHTML.Replace("{{LUY_KE_DAU_NAM}}", openingStock > 0 ? String.Format("{0:N0}", openingStock) : string.Empty)
                                                         .Replace("{{LUY_KE_DAU_NAM}}", openingStock > 0 ? String.Format("{0:N0}", openingStock) : string.Empty);

                if (p.BookDetails.Count > 0 && param.BookDetailType == 3)
                {
                    p.BookDetails.ForEach(x =>
                    {
                        _ind++;
                        string _txt = @"<tr>
                                            <td>{{{NGAY_GHI_SO}}}</td>
                                    <td><a href='{{{URL_LINK}}}#{{{FILTER_TYPE}}}#{{{FILTER_TEXT}}}#{{{FILTER_MONTH}}}#{{{FILTER_ISINTERNAL}}}' target='_blank'>{{{CHUNG_TU_SO}}}</a></td>

                                            <td>{{{CHUNG_TU_NGAY}}}</td>
                                            <td class='tbl-td-diengiai' colspan='2'>{{{DIEN_GIAI}}}</td>
                                            <td>{{{TK_DOI_UNG}}}</td>
                                            <td>{{{CHI_TIET}}}</td>
                                            <td class='txt-right'>{{{TY_GIA}}}</td>
                                            <td class='txt-right'>{{{PSN_NGOAI_TE}}}</td>
                                            <td class='txt-right'>{{{PSN_VND}}}</td>
                                            <td class='txt-right'>{{{PSC_NGOAI_TE}}}</td>
                                            <td class='txt-right'>{{{PSC_VND}}}</td>
                                            <td class='txt-right'>{{{SD_NGOAI_TE}}}</td>
                                            <td class='txt-right'>{{{SD_VND}}}</td>
                                        </tr>";

                        _txt = _txt.Replace("{{{NGAY_GHI_SO}}}", x.OrginalBookDate.HasValue ? x.OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty)
                                            .Replace("{{{URL_LINK}}}", UrlLinkConst.UrlLinkArise)
                                            .Replace("{{{CHUNG_TU_SO}}}", x.OrginalVoucherNumber)
                                            .Replace("{{{CHUNG_TU_NGAY}}}", x.OrginalBookDate.HasValue ? x.OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty)
                                            .Replace("{{{DIEN_GIAI}}}", x.Description)
                                            .Replace("{{{TK_DOI_UNG}}}", x.IsDebit ? x.CreditCode : x.DebitCode)
                                            .Replace("{{{CHI_TIET}}}", x.DetailCode)

                                            .Replace("{{{TY_GIA}}}", (x.ExchangeRate > 0) ? String.Format("{0:" + soThapPhan + "}", x.ExchangeRate) : string.Empty)
                                            .Replace("{{{PSN_NGOAI_TE}}}", x.IsDebit ? String.Format("{0:" + soThapPhan + "}", x.OrginalCurrency) : string.Empty)
                                            .Replace("{{{PSN_VND}}}", x.IsDebit ? String.Format("{0:" + soThapPhan + "}", x.Amount) : string.Empty)
                                            .Replace("{{{PSC_NGOAI_TE}}}", !x.IsDebit ? String.Format("{0:" + soThapPhan + "}", x.OrginalCurrency) : string.Empty)
                                            .Replace("{{{PSC_VND}}}", !x.IsDebit ? String.Format("{0:" + soThapPhan + "}", x.Amount) : string.Empty)
                                            .Replace("{{{SD_NGOAI_TE}}}", x.ResidualAmount_Foreign < 0 ? "(" + String.Format("{0:" + soThapPhan + "}", Math.Abs(x.ResidualAmount_Foreign)) + ")" : String.Format("{0:" + soThapPhan + "}", x.ResidualAmount_Foreign))
                                            .Replace("{{{SD_VND}}}", x.ResidualAmount_OrginalCur < 0 ? "(" + String.Format("{0:" + soThapPhan + "}", Math.Abs(x.ResidualAmount_OrginalCur)) + ")" : String.Format("{0:" + soThapPhan + "}", x.ResidualAmount_OrginalCur))

                                                                                   .Replace("{{{FILTER_TEXT}}}", x.OrginalVoucherNumber)

                                            .Replace("{{{FILTER_TYPE}}}", x.VoucherNumber.Split('/')[1])
                                   .Replace("{{{FILTER_MONTH}}}", x.Month.ToString())
                                                                          .Replace("{{{FILTER_ISINTERNAL}}}", (param.IsNoiBo ? "3" : "1"))

    ;

                        resultHTML += _txt;

                        //asdasdasd
                        if (!param.IsNewReport)
                        {
                            SoChiTietViewModel _sct = p.BookDetails[_ind + 1 < p.BookDetails.Count ? _ind + 1 : p.BookDetails.Count - 1];
                            if (((_sct.Month + "" + _sct.Year) != (x.Month + "" + x.Year) && _ind < p.BookDetails.Count) || (_ind == p.BookDetails.Count - 1) && p.SumItem_SCT_ThuChi.ContainsKey(x.Temp))
                            {
                                List<SoChiTietThuChiViewModel> _ledgerSum = p.SumItem_SCT_ThuChi[x.Temp];

                                SoChiTietThuChiViewModel _CongPhatSinh = _ledgerSum.FirstOrDefault(x => x.Month == -1) ?? new SoChiTietThuChiViewModel();
                                SoChiTietThuChiViewModel _LuyKe = _ledgerSum.FirstOrDefault(x => x.Month == -2) ?? new SoChiTietThuChiViewModel();
                                SoChiTietThuChiViewModel _Du = _ledgerSum.FirstOrDefault(x => x.Month == -3) ?? new SoChiTietThuChiViewModel();

                                string _sumRowMonthHTML = @"
                                                            <tr class='font-b'>
                                                                <td colspan='3'></td>
                                                                <td colspan='2'>Cộng phát sinh tháng {{THANG_ROW}}</td>
                                                                <td></td>
                                                                <td></td>
                                                                <td></td>
                                                                <td class='txt-right'>{{SR_PSN_NGOAI_TE}}</td>
                                                                <td class='txt-right'>{{SR_PSN_VND}}</td>
                                                                <td class='txt-right'>{{SR_PSC_NGOAI_TE}}</td>
                                                                <td class='txt-right'>{{SR_PSC_VND}}</td>
                                                                <td class='txt-right'>{{SR_SD_NGOAI_TE}}</td>
                                                                <td class='txt-right'>{{SR_SD_VND}}</td>
                                                            </tr>
			                                                <tr class='font-b'>
                                                                <td colspan='3'></td>
                                                                <td colspan='2'>Lũy kế phát sinh đến cuối kỳ</td>
                                                                <td></td>
                                                                <td></td>
                                                                <td></td>
                                                                <td class='txt-right'>{{SR_LK_PSN_NGOAI_TE}}</td>
                                                                <td class='txt-right'>{{SR_LK_PSN_VND}}</td>
                                                                <td class='txt-right'>{{SR_LK_PSC_NGOAI_TE}}</td>
                                                                <td class='txt-right'>{{SR_LK_PSC_VND}}</td>
                                                                <td class='txt-right'>{{SR_LK_SD_NGOAI_TE}}</td>
                                                                <td class='txt-right'>{{SR_LK_SD_VND}}</td>
                                                            </tr>
                                                            <tr class='font-b'>
                                                                <td colspan='3'></td>
                                                                <td colspan='2'>Dư cuối {{THANG_ROW}}</td>
                                                                <td></td>
                                                                <td></td>
                                                                <td></td>
                                                                <td class='txt-right'>{{SR_DC_PSN_NGOAI_TE}}</td>
                                                                <td class='txt-right'>{{SR_DC_PSN_VND}}</td>
                                                                <td class='txt-right'>{{SR_DC_PSC_NGOAI_TE}}</td>
                                                                <td class='txt-right'>{{SR_DC_PSC_VND}}</td>
                                                                <td class='txt-right'>{{SR_DC_SD_NGOAI_TE}}</td>
                                                                <td class='txt-right'>{{SR_DC_SD_VND}}</td>
                                                            </tr>
";

                                _sumRowMonthHTML = _sumRowMonthHTML.Replace("{{THANG_ROW}}", x.Month.ToString("00") + "/" + x.Year.ToString())

                                 .Replace("{{SR_PSN_NGOAI_TE}}", _CongPhatSinh.ArisingDebit_Foreign > 0 ? String.Format("{0:" + soThapPhan + "}", _CongPhatSinh.ArisingDebit_Foreign) : string.Empty)
                                 .Replace("{{SR_PSN_VND}}", _CongPhatSinh.Thu_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _CongPhatSinh.Thu_Amount) : string.Empty)
                                 .Replace("{{SR_PSC_NGOAI_TE}}", _CongPhatSinh.ArisingCredit_Foreign > 0 ? String.Format("{0:" + soThapPhan + "}", _CongPhatSinh.ArisingCredit_Foreign) : string.Empty)
                                 .Replace("{{SR_PSC_VND}}", _CongPhatSinh.Chi_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _CongPhatSinh.Chi_Amount) : string.Empty)
                                 .Replace("{{SR_SD_NGOAI_TE}}", String.Format("{0:" + soThapPhan + "}", string.Empty))
                                 .Replace("{{SR_SD_VND}}", String.Format("{0:" + soThapPhan + "}", string.Empty))

                                 .Replace("{{SR_LK_PSN_NGOAI_TE}}", _LuyKe.ArisingDebit_Foreign > 0 ? String.Format("{0:" + soThapPhan + "}", _LuyKe.ArisingDebit_Foreign) : string.Empty)
                                 .Replace("{{SR_LK_PSN_VND}}", _LuyKe.Thu_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _LuyKe.Thu_Amount) : string.Empty)
                                 .Replace("{{SR_LK_PSC_NGOAI_TE}}", _LuyKe.ArisingCredit_Foreign > 0 ? String.Format("{0:" + soThapPhan + "}", _LuyKe.ArisingCredit_Foreign) : string.Empty)
                                 .Replace("{{SR_LK_PSC_VND}}", _LuyKe.Chi_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _LuyKe.Chi_Amount) : string.Empty)
                                 .Replace("{{SR_LK_SD_NGOAI_TE}}", String.Format("{0:" + soThapPhan + "}", string.Empty))
                                 .Replace("{{SR_LK_SD_VND}}", String.Format("{0:" + soThapPhan + "}", string.Empty))

                                 .Replace("{{SR_DC_PSN_NGOAI_TE}}", String.Format("{0:" + soThapPhan + "}", string.Empty))
                                 .Replace("{{SR_DC_PSN_VND}}", String.Format("{0:" + soThapPhan + "}", string.Empty))
                                 .Replace("{{SR_DC_PSC_NGOAI_TE}}", String.Format("{0:" + soThapPhan + "}", string.Empty))
                                 .Replace("{{SR_DC_PSC_VND}}", String.Format("{0:" + soThapPhan + "}", string.Empty))
                                 .Replace("{{SR_DC_SD_NGOAI_TE}}", _Du.ResidualAmount_Foreign < 0 ? "(" + String.Format("{0:" + soThapPhan + "}", Math.Abs(_Du.ResidualAmount_Foreign)) + ")" : String.Format("{0:" + soThapPhan + "}", _Du.ResidualAmount_Foreign))
                                 .Replace("{{SR_DC_SD_VND}}", _Du.ResidualAmount_OrginalCur < 0 ? "(" + String.Format("{0:" + soThapPhan + "}", Math.Abs(_Du.ResidualAmount_OrginalCur)) + ")" : String.Format("{0:" + soThapPhan + "}", _Du.ResidualAmount_OrginalCur))

                                ;

                                resultHTML += _sumRowMonthHTML;
                            }
                        }
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

        private string ConvertToHTML_SoChiTiet_Loai_5(LedgerReportModel p, LedgerReportParamDetail param, double openingStock, int year)
        {
            try
            {
                if (p == null) return string.Empty;
                if (param.FromMonth == null) param.FromMonth = 0;
                if (param.ToMonth == null) param.ToMonth = 0;
                if (param.FromDate == null) param.FromDate = DateTime.Now;
                if (param.ToDate == null) param.ToDate = DateTime.Now;

                string _template = "SoChiTiet_FiveTemplate.html",
                    _folderPath = @"Uploads\Html",
                    path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                    _allText = System.IO.File.ReadAllText(path), resultHTML = string.Empty;
                IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TIEU_DE","SỔ CHI TIẾT "+ p.AccountName.ToUpper() },
                { "TenCongTy", p.Company },
                { "DiaChi", p.Address },
                { "MST", p.TaxId },
                { "NgayChungTu", string.Empty },
                { "TaiKhoanAccount", p.AccountCode+": "+p.AccountName },
                { "TuThang", param.FromDate.Value.Month.ToString("D2")   },
                { "DenThang", param.ToDate.Value.AddDays(-1).Month.ToString("D2") },
                { "Nam",  year.ToString("D4") },
                { "NguoiLap", !string.IsNullOrEmpty(param.LedgerReportMaker) ? param.LedgerReportMaker : string.Empty },
                { "Ngay", (param.ToDate.Value.Day).ToString("D2") },
                { "Thang",  (param.ToDate.Value.Month).ToString("D2") },
                { "NamSign",   year.ToString() },
                { "KeToanTruong", param.isCheckName ? p.ChiefAccountantName : string.Empty },
                { "GiamDoc", param.isCheckName ? p.CEOName : string.Empty },
                {"LUY_KE_DAU_NAM", openingStock > 0 ? String.Format("{0:N0}", openingStock) : string.Empty },
                { "KeToanTruong_CV", p.ChiefAccountantNote},
                            { "GiamDoc_CV",  p.CEONote},
            };
                string soThapPhan = "N" + p.MethodCalcExportPrice;
                v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));
                int _ind = -1;

                resultHTML = @"<tr class='font-b'>
                                                                <td colspan='6'>Lũy kế đầu năm</td>
                                                                <td></td>
                                                                <td  colspan='3' class='txt-right'>{{LUY_KE_DAU_NAM}}</td>
                                                            </tr>";
                resultHTML = resultHTML.Replace("{{LUY_KE_DAU_NAM}}", openingStock > 0 ? String.Format("{0:N0}", openingStock) : string.Empty)
                                                         .Replace("{{LUY_KE_DAU_NAM}}", openingStock > 0 ? String.Format("{0:N0}", openingStock) : string.Empty);

                if (p.BookDetails.Count > 0 && param.BookDetailType == 5)
                {
                    p.BookDetails.ForEach(x =>
                    {
                        _ind++;
                        string _txt = @"<tr>
                                            <td>{{{NGAY_GHI_SO}}}</td>
                                    <td><a href='{{{URL_LINK}}}#{{{FILTER_TYPE}}}#{{{FILTER_TEXT}}}#{{{FILTER_MONTH}}}#{{{FILTER_ISINTERNAL}}}' target='_blank'>{{{CHUNG_TU_SO}}}</a></td>

                                            <td>{{{CHUNG_TU_NGAY}}}</td>
                                            <td>{{{NGUOI_NOP}}}</td>
                                            <td class='tbl-td-diengiai' colspan='2'>{{{DIEN_GIAI}}}</td>
                                            <td>{{{TK_DOI_UNG}}}</td>
                                            <td class='txt-right'>{{{THU}}}</td>
                                            <td class='txt-right'>{{{CHI}}}</td>
                                            <td class='txt-right'>{{{TON}}}</td>
                                        </tr>";

                        _txt = _txt.Replace("{{{NGAY_GHI_SO}}}", x.OrginalBookDate.HasValue ? x.OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty)
                        .Replace("{{{URL_LINK}}}", UrlLinkConst.UrlLinkArise)
                                            .Replace("{{{CHUNG_TU_SO}}}", x.OrginalVoucherNumber)
                                            .Replace("{{{CHUNG_TU_NGAY}}}", x.OrginalBookDate.HasValue ? x.OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty)
                                            .Replace("{{{NGUOI_NOP}}}", x.NameOfPerson)
                                            .Replace("{{{DIEN_GIAI}}}", x.Description)
                                            .Replace("{{{TK_DOI_UNG}}}", x.IsDebit ? x.CreditCode : x.DebitCode)

                                            .Replace("{{{THU}}}", x.IsDebit ? String.Format("{0:" + soThapPhan + "}", x.Thu_Amount) : string.Empty)
                                            .Replace("{{{CHI}}}", !x.IsDebit ? String.Format("{0:" + soThapPhan + "}", x.Chi_Amount) : string.Empty)
                                            .Replace("{{{TON}}}", x.Residual_Amount < 0 ? "(" + String.Format("{0:" + soThapPhan + "}", Math.Abs(x.Residual_Amount)) + ")" : String.Format("{0:" + soThapPhan + "}", Math.Abs(x.Residual_Amount)))

                                             .Replace("{{{FILTER_TEXT}}}", x.OrginalVoucherNumber)
                                            .Replace("{{{FILTER_TYPE}}}", x.VoucherNumber.Split('/')[1])
                                   .Replace("{{{FILTER_MONTH}}}", x.Month.ToString())
                                                                          .Replace("{{{FILTER_ISINTERNAL}}}", (param.IsNoiBo ? "3" : "1"));

                        resultHTML += _txt;

                        if (!param.IsNewReport)
                        {
                            SoChiTietViewModel _sct = p.BookDetails[_ind + 1 < p.BookDetails.Count ? _ind + 1 : p.BookDetails.Count - 1];
                            if (((_sct.Month + "" + _sct.Year) != (x.Month + "" + x.Year) && _ind < p.BookDetails.Count) || (_ind == p.BookDetails.Count - 1) && p.SumItem_SCT_ThuChi.ContainsKey(x.Temp))
                            {
                                List<SoChiTietThuChiViewModel> _ledgerSum = p.SumItem_SCT_ThuChi[x.Temp];

                                SoChiTietThuChiViewModel _CongPhatSinh = _ledgerSum.FirstOrDefault(x => x.Month == -1) ?? new SoChiTietThuChiViewModel();
                                SoChiTietThuChiViewModel _LuyKe = _ledgerSum.FirstOrDefault(x => x.Month == -2) ?? new SoChiTietThuChiViewModel();
                                SoChiTietThuChiViewModel _Du = _ledgerSum.FirstOrDefault(x => x.Month == -3) ?? new SoChiTietThuChiViewModel();

                                string _sumRowMonthHTML = @"
<tr class='font-b'>
                                                                <td colspan='6'>Cộng phát sinh tháng {{THANG_ROW}}</td>
                                                                <td></td>
                                                                <td class='txt-right'>{{SR_THU}}</td>
                                                                <td class='txt-right'>{{SR_CHI}}</td>
                                                                <td class='txt-right'>{{SR_TON}}</td>
                                                            </tr>
			                                                <tr class='font-b'>
                                                                <td colspan='6'>Lũy kế phát sinh đến cuối kỳ</td>
                                                                <td></td>
                                                                <td class='txt-right'>{{SR_LK_THU}}</td>
                                                                <td class='txt-right'>{{SR_LK_CHI}}</td>
                                                                <td class='txt-right'>{{SR_LK_TON}}</td>
                                                            </tr>
                                                            <tr class='font-b' style='display:none;'>
                                                                <td colspan='6'>Dư cuối {{THANG_ROW}}</td>
                                                                <td></td>
                                                                <td class='txt-right'>{{SR_DC_THU}}</td>
                                                                <td class='txt-right'>{{SR_DC_CHI}}</td>
                                                                <td class='txt-right'>{{SR_DC_TON}}</td>
                                                            </tr>";

                                _sumRowMonthHTML = _sumRowMonthHTML.Replace("{{THANG_ROW}}", x.Month.ToString("00") + "/" + x.Year.ToString())

                                 .Replace("{{SR_THU}}", _CongPhatSinh.Thu_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _CongPhatSinh.Thu_Amount) : string.Empty)
                                 .Replace("{{SR_CHI}}", _CongPhatSinh.Chi_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _CongPhatSinh.Chi_Amount) : string.Empty)
                                 .Replace("{{SR_TON}}", _CongPhatSinh.Residual_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _CongPhatSinh.Residual_Amount) : string.Empty)

                                 .Replace("{{SR_LK_THU}}", _LuyKe.Thu_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _LuyKe.Thu_Amount) : string.Empty)
                                 .Replace("{{SR_LK_CHI}}", _LuyKe.Chi_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _LuyKe.Chi_Amount) : string.Empty)
                                 .Replace("{{SR_LK_TON}}", _LuyKe.Residual_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _LuyKe.Residual_Amount) : string.Empty)

                                 .Replace("{{SR_DC_THU}}", _Du.Thu_Amount < 0 ? "(" + String.Format("{0:" + soThapPhan + "}", Math.Abs(_Du.Thu_Amount)) + ")" : String.Format("{0:" + soThapPhan + "}", _Du.Thu_Amount))
                                 .Replace("{{SR_DC_CHI}}", _Du.Chi_Amount < 0 ? "(" + String.Format("{0:" + soThapPhan + "}", Math.Abs(_Du.Chi_Amount)) + ")" : String.Format("{0:" + soThapPhan + "}", _Du.Chi_Amount))
                                 .Replace("{{SR_DC_TON}}", _Du.Residual_Amount < 0 ? "(" + String.Format("{0:" + soThapPhan + "}", Math.Abs(_Du.Residual_Amount)) + ")" : String.Format("{0:" + soThapPhan + "}", _Du.Residual_Amount))

                                ;

                                resultHTML += _sumRowMonthHTML;
                            }
                        }
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

        private string ConvertToHTML_SoChiTiet_Loai_6(LedgerReportModel p, LedgerReportParamDetail param, int year)
        {
            try
            {
                if (p == null) return string.Empty;

                string _template = "SoChiTiet_SixTemplate.html",
                    _folderPath = @"Uploads\Html",
                    path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                    _allText = System.IO.File.ReadAllText(path), resultHTML = string.Empty;
                IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TIEU_DE","SỔ CHI TIẾT "+ p.AccountName.ToUpper() },
                { "TenCongTy", p.Company },
                { "DiaChi", p.Address },
                { "MST", p.TaxId },
                { "NgayChungTu", string.Empty },
                { "TaiKhoanAccount", (p.AccountCode ?? "" ) +": "+ (p.AccountName  ?? "" )},
                { "TuThang", param.FromDate.Value.Month.ToString("D2")   },
                { "DenThang", param.ToDate.Value.AddDays(-1).Month.ToString("D2") },
                { "Nam", year.ToString("D4") },
                { "NguoiLap", !string.IsNullOrEmpty(param.LedgerReportMaker) ? param.LedgerReportMaker : string.Empty },
                { "Ngay", (param.ToDate.Value.Day).ToString("D2") },
                { "Thang",  (param.ToDate.Value.Month).ToString("D2") },
                { "NamSign",   year.ToString() },
                { "KeToanTruong", param.isCheckName ? p.ChiefAccountantName : string.Empty },
                { "GiamDoc", param.isCheckName ? p.CEOName : string.Empty },
                { "KeToanTruong_CV", p.ChiefAccountantNote},
                            { "GiamDoc_CV",  p.CEONote},
            };

                string soThapPhan = "N" + p.MethodCalcExportPrice;
                v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));
                int _ind = 0;
                if (p.ItemSLTons.Count > 0)
                {
                    p.ItemSLTons.ForEach(x =>
                    {
                        _ind++;
                        string _txt = @"<tr>
                                            <td>{{{STT}}}</td>
                                            <td>{{{KHO}}}</td>
                                            <td>{{{MA_HANG}}}</td>
                                            <td>{{{TEN_HANG}}}</td>
                                            <td class='txt-right'>{{{OPEN_SL}}}</td>
                                            <td class='txt-right'>{{{IP_SL}}}</td>
                                            <td class='txt-right'>{{{OP_SL}}}</td>
                                            <td class='txt-right'>{{{LEFT_SL}}}</td>
                                        </tr>";
                        x.CloseQuantity = x.OpenQuantity + x.InputQuantity - x.OutputQuantity;
                        _txt = _txt.Replace("{{{STT}}}", _ind.ToString())
                                            .Replace("{{{KHO}}}", x.Warehouse)
                                            .Replace("{{{MA_HANG}}}", string.IsNullOrEmpty(x.Detail2) ? x.Detail1 : x.Detail2)
                                            .Replace("{{{TEN_HANG}}}", x.NameGood)
                                            .Replace("{{{OPEN_SL}}}", x.OpenQuantity > 0 ? String.Format("{0:" + soThapPhan + "}", x.OpenQuantity) : String.Empty)
                                            .Replace("{{{IP_SL}}}", x.InputQuantity > 0 ? String.Format("{0:" + soThapPhan + "}", x.InputQuantity) : String.Empty)
                                            .Replace("{{{OP_SL}}}", x.OutputQuantity > 0 ? String.Format("{0:" + soThapPhan + "}", x.OutputQuantity) : String.Empty)
                                            .Replace("{{{LEFT_SL}}}", x.CloseQuantity < 0 ? "(" + String.Format("{0:" + soThapPhan + "}", Math.Abs(x.CloseQuantity)) + ")" : String.Format("{0:" + soThapPhan + "}", x.CloseQuantity))
                                            ;

                        resultHTML += _txt;
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

        private string ConvertToHTML_SoChiTiet_Loai_4(LedgerReportModel p, LedgerReportParamDetail param, int year)
        {
            try
            {
                if (p == null) return string.Empty;

                StringBuilder _allTextFull = new StringBuilder();

                if (p.BookDetails.Count > 0 && param.BookDetailType == 4)
                {
                    foreach (var item in p.listAccoutCodeThuChi)
                    {
                        string[] listKey = item.Key.Split('/');
                        string detail1 = listKey[0];
                        string detail2 = listKey[1];
                        string warehouseCode = listKey[2];
                        int _ind = -1;
                        string _template = "SoChiTiet_FourTemplate.html",
                        _folderPath = @"Uploads\Html",
                        path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template);
                        StringBuilder resultHTML = new StringBuilder();
                        string _allText = System.IO.File.ReadAllText(path);
                        IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
                        {
                            { "TIEU_DE","SỔ CHI TIẾT "+ p.AccountName.ToUpper() },
                            { "TenCongTy", p.Company },
                            { "DiaChi", p.Address },
                            { "MST", p.TaxId },
                            { "NgayChungTu", string.Empty },
                            { "TaiKhoanAccount", param.AccountCodeDetail1 + " - " + (string.IsNullOrEmpty(detail2) ? detail1 : detail2) + (string.IsNullOrEmpty(warehouseCode) ? "" : (" - " + warehouseCode))},
                            { "TuThang", param.FromDate.Value.Month.ToString("D2")   },
                            { "DenThang", param.ToDate.Value.Month.ToString("D2") },
                            { "Nam", year.ToString("D4") },
                            { "NguoiLap", !string.IsNullOrEmpty(param.LedgerReportMaker) ? param.LedgerReportMaker : string.Empty },
                            { "Ngay", (param.ToDate.Value.Day).ToString("D2") },
                            { "Thang",  (param.ToDate.Value.Month).ToString("D2") },
                            { "NamSign",   year.ToString() },
                            { "KeToanTruong", param.isCheckName ? p.ChiefAccountantName : string.Empty },
                            { "GiamDoc", param.isCheckName ? p.CEOName : string.Empty },
                            {"LUY_KE_DAU_NAM", listKey[3] + " - " +  listKey[4]},
                            { "KeToanTruong_CV", p.ChiefAccountantNote},
                            { "GiamDoc_CV",  p.CEONote},
                        };
                        v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));
                        string soThapPhan = "N" + p.MethodCalcExportPrice;
                        var listBookDetails = p.BookDetails.Where(x => string.IsNullOrEmpty(detail1) || (x.DebitDetailCodeFirst == detail1 || x.CreditDetailCodeFirst == detail1)
                                        && string.IsNullOrEmpty(detail2) || (x.DebitDetailCodeSecond == detail2 || x.CreditDetailCodeSecond == detail2)
                                        && string.IsNullOrEmpty(warehouseCode) || (x.CreditWarehouseCode == warehouseCode || x.DebitWarehouseCode == warehouseCode)
                                        ).ToList();
                        string luyKeDauNam = @"<tr class='font-b'>
<td colspan='3'></td>
                                                                <td colspan='2'>Lũy kế đầu năm</td>
                                                                <td></td>
                                                                <td></td>
<td></td>
                                                                <td  colspan='6' class='txt-right'>{{LUY_KE_DAU_NAM}}</td>
                                                            </tr>";
                        luyKeDauNam = luyKeDauNam.Replace("{{LUY_KE_DAU_NAM}}", listKey[3]);
                        resultHTML.Append(luyKeDauNam);

                        listBookDetails.ForEach(x =>
                        {
                            _ind++;
                            string _txt = @"<tr>
                                            <td>{{{NGAY_GHI_SO}}}</td>
                                    <td><a href='{{{URL_LINK}}}#{{{FILTER_TYPE}}}#{{{FILTER_TEXT}}}#{{{FILTER_MONTH}}}#{{{FILTER_ISINTERNAL}}}' target='_blank'>{{{CHUNG_TU_SO}}}</a></td>

                                            <td>{{{CHUNG_TU_NGAY}}}</td>
                                            <td class='tbl-td-diengiai' colspan='2'>{{{DIEN_GIAI}}}</td>
                                            <td>{{{TK_DOI_UNG}}}</td>
                                            <td>{{{CHI_TIET}}}</td>
                                            <td class='txt-right'>{{{DON_GIA}}}</td>
                                            <td class='txt-right'>{{{IP_SL}}}</td>
                                            <td class='txt-right'>{{{IP_MONEY}}}</td>
                                            <td class='txt-right'>{{{OP_SL}}}</td>
                                            <td class='txt-right'>{{{OP_MONEY}}}</td>
                                            <td class='txt-right'>{{{LEFT_SL}}}</td>
                                            <td class='txt-right'>{{{LEFT_MONEY}}}</td>
                                        </tr>";

                            _txt = _txt.Replace("{{{NGAY_GHI_SO}}}", x.OrginalBookDate.HasValue ? x.OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty)
                            .Replace("{{{URL_LINK}}}", UrlLinkConst.UrlLinkArise)
                                                .Replace("{{{CHUNG_TU_SO}}}", x.OrginalVoucherNumber)
                                                .Replace("{{{CHUNG_TU_NGAY}}}", x.OrginalBookDate.HasValue ? x.OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty)
                                                .Replace("{{{DIEN_GIAI}}}", string.IsNullOrEmpty(detail2) ? detail1 : detail2)
                                                .Replace("{{{TK_DOI_UNG}}}", x.IsDebit ? x.CreditCode : x.DebitCode)
                                                .Replace("{{{CHI_TIET}}}", x.DetailCode)

                                                .Replace("{{{DON_GIA}}}", String.Format("{0:" + soThapPhan + "}", x.UnitPrice))
                                                .Replace("{{{IP_SL}}}", x.IsDebit ? String.Format("{0:" + soThapPhan + "}", x.Quantity) : string.Empty)
                                                .Replace("{{{IP_MONEY}}}", x.IsDebit ? String.Format("{0:" + soThapPhan + "}", x.Amount) : string.Empty)
                                                .Replace("{{{OP_SL}}}", !x.IsDebit ? String.Format("{0:" + soThapPhan + "}", x.Quantity) : string.Empty)
                                                .Replace("{{{OP_MONEY}}}", !x.IsDebit ? String.Format("{0:" + soThapPhan + "}", x.Amount) : string.Empty)
                                                .Replace("{{{LEFT_SL}}}", x.Residual_Quantity < 0 ? "(" + String.Format("{0:" + soThapPhan + "}", Math.Abs(x.Residual_Quantity)) + ")" : String.Format("{0:" + soThapPhan + "}", x.Residual_Quantity))
                                                .Replace("{{{LEFT_MONEY}}}", x.Residual_Amount < 0 ? "(" + String.Format("{0:" + soThapPhan + "}", Math.Abs(x.Residual_Amount)) + ")" : String.Format("{0:" + soThapPhan + "}", x.Residual_Amount))

                                                                                       .Replace("{{{FILTER_ISINTERNAL}}}", (param.IsNoiBo ? "3" : "1"))

                                            .Replace("{{{FILTER_TYPE}}}", x.VoucherNumber.Split('/')[1])
                                   .Replace("{{{FILTER_MONTH}}}", x.Month.ToString())
                                   .Replace("{{{FILTER_ISINTERNAL}}}", (param.IsNoiBo ? "3" : "1"))
                                   .Replace("{{{FILTER_TEXT}}}", x.OrginalVoucherNumber);

                            resultHTML.Append(_txt);

                            SoChiTietViewModel _sct = listBookDetails[_ind + 1 < listBookDetails.Count ? _ind + 1 : listBookDetails.Count - 1];
                            if (!param.IsNewReport && (_sct.Month != x.Month && _ind < listBookDetails.Count) || (_ind == listBookDetails.Count - 1))
                            {
                                List<SoChiTietThuChiViewModel> _ledgerSum = item.Value.Where(y => y.Month == x.Month).ToList();

                                SoChiTietThuChiViewModel _CongPhatSinh = _ledgerSum.FirstOrDefault(y => y.Type == 1);
                                if (_CongPhatSinh == null)
                                    _CongPhatSinh = new SoChiTietThuChiViewModel();
                                SoChiTietThuChiViewModel _LuyKe = _ledgerSum.FirstOrDefault(y => y.Type == 2);
                                if (_LuyKe == null)
                                    _LuyKe = new SoChiTietThuChiViewModel();
                                SoChiTietThuChiViewModel _Du = _ledgerSum.FirstOrDefault(y => y.Type == 3);
                                if (_Du == null)
                                    _Du = new SoChiTietThuChiViewModel();
                                string _sumRowMonthHTML = @"
<tr class='font-b'>
                                                                <td colspan='3'></td>
                                                                <td colspan='2'>Cộng phát sinh tháng {{THANG_ROW}}</td>
                                                                <td></td>
                                                                <td></td>
                                                                <td></td>
                                                                <td class='txt-right'>{{SR_IP_SL}}</td>
                                                                <td class='txt-right'>{{SR_IP_TIEN}}</td>
                                                                <td class='txt-right'>{{SR_OP_SL}}</td>
                                                                <td class='txt-right'>{{SR_OP_TIEN}}</td>
                                                                <td class='txt-right'>{{SR_LEFT_SL}}</td>
                                                                <td class='txt-right'>{{SR_LEFT_TIEN}}</td>
                                                            </tr>
			                                                <tr class='font-b'>
                                                                <td colspan='3'></td>
                                                                <td colspan='2'>Lũy kế phát sinh đến cuối kỳ</td>
                                                                <td></td>
                                                                <td></td>
                                                                <td></td>
                                                                <td class='txt-right'>{{SR_LK_IP_SL}}</td>
                                                                <td class='txt-right'>{{SR_LK_IP_TIEN}}</td>
                                                                <td class='txt-right'>{{SR_LK_OP_SL}}</td>
                                                                <td class='txt-right'>{{SR_LK_OP_TIEN}}</td>
                                                                <td class='txt-right'>{{SR_LK_LEFT_SL}}</td>
                                                                <td class='txt-right'>{{SR_LK_LEFT_TIEN}}</td>
                                                            </tr>
                                                            <tr class='font-b'>
                                                                <td colspan='3'></td>
                                                                <td colspan='2'>Dư cuối {{THANG_ROW}}</td>
                                                                <td></td>
                                                                <td></td>
                                                                <td class='txt-right'>{{SR_DC_IP_DG}}</td>
                                                                <td class='txt-right'>{{SR_DC_IP_SL}}</td>
                                                                <td class='txt-right'>{{SR_DC_IP_TIEN}}</td>
                                                                <td class='txt-right'>{{SR_DC_OP_SL}}</td>
                                                                <td class='txt-right'>{{SR_DC_OP_TIEN}}</td>
                                                                <td class='txt-right'>{{SR_DC_LEFT_SL}}</td>
                                                                <td class='txt-right'>{{SR_DC_LEFT_TIEN}}</td>
                                                            </tr>";

                                double SR_DC_IP_DG = 0;
                                if (_Du.Residual_Quantity > 0)
                                    SR_DC_IP_DG = Math.Abs(_Du.Residual_Amount) / Math.Abs(_Du.Residual_Quantity);
                                _sumRowMonthHTML = _sumRowMonthHTML.Replace("{{THANG_ROW}}", x.Month.ToString("00") + "/" + x.Year.ToString())

                                 .Replace("{{SR_IP_SL}}", _CongPhatSinh.Input_Quantity > 0 ? String.Format("{0:" + soThapPhan + "}", _CongPhatSinh.Input_Quantity) : string.Empty)
                                 .Replace("{{SR_IP_TIEN}}", _CongPhatSinh.Thu_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _CongPhatSinh.Thu_Amount) : string.Empty)
                                 .Replace("{{SR_OP_SL}}", _CongPhatSinh.Output_Quantity > 0 ? String.Format("{0:" + soThapPhan + "}", _CongPhatSinh.Output_Quantity) : string.Empty)
                                 .Replace("{{SR_OP_TIEN}}", _CongPhatSinh.Chi_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _CongPhatSinh.Chi_Amount) : string.Empty)
                                 .Replace("{{SR_LEFT_SL}}", String.Format("{0:" + soThapPhan + "}", string.Empty))
                                 .Replace("{{SR_LEFT_TIEN}}", String.Format("{0:" + soThapPhan + "}", string.Empty))

                                 .Replace("{{SR_LK_IP_SL}}", _LuyKe.Input_Quantity > 0 ? String.Format("{0:" + soThapPhan + "}", _LuyKe.Input_Quantity) : string.Empty)
                                 .Replace("{{SR_LK_IP_TIEN}}", _LuyKe.Thu_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _LuyKe.Thu_Amount) : string.Empty)
                                 .Replace("{{SR_LK_OP_SL}}", _LuyKe.Output_Quantity > 0 ? String.Format("{0:" + soThapPhan + "}", _LuyKe.Output_Quantity) : string.Empty)
                                 .Replace("{{SR_LK_OP_TIEN}}", _LuyKe.Chi_Amount > 0 ? String.Format("{0:" + soThapPhan + "}", _LuyKe.Chi_Amount) : string.Empty)
                                 .Replace("{{SR_LK_LEFT_SL}}", String.Format("{0:" + soThapPhan + "}", string.Empty))
                                 .Replace("{{SR_LK_LEFT_TIEN}}", String.Format("{0:" + soThapPhan + "}", string.Empty))

                                 .Replace("{{SR_DC_IP_DG}}", SR_DC_IP_DG > 0 ? String.Format("{0:" + soThapPhan + "}", SR_DC_IP_DG) : string.Empty)
                                 .Replace("{{SR_DC_IP_SL}}", String.Format("{0:" + soThapPhan + "}", string.Empty))
                                 .Replace("{{SR_DC_IP_TIEN}}", String.Format("{0:" + soThapPhan + "}", string.Empty))
                                 .Replace("{{SR_DC_OP_SL}}", String.Format("{0:" + soThapPhan + "}", string.Empty))
                                 .Replace("{{SR_DC_OP_TIEN}}", String.Format("{0:" + soThapPhan + "}", string.Empty))
                                 .Replace("{{SR_DC_LEFT_SL}}", _Du.Residual_Quantity < 0 ? "(" + String.Format("{0:" + soThapPhan + "}", Math.Abs(_Du.Residual_Quantity)) + ")" : String.Format("{0:" + soThapPhan + "}", _Du.Residual_Quantity))
                                 .Replace("{{SR_DC_LEFT_TIEN}}", _Du.Residual_Amount < 0 ? "(" + String.Format("{0:" + soThapPhan + "}", Math.Abs(_Du.Residual_Amount)) + ")" : String.Format("{0:" + soThapPhan + "}", _Du.Residual_Amount))
                                ;
                                resultHTML.Append(_sumRowMonthHTML);
                            }
                        });
                        _allTextFull.Append(_allText.Replace("##REPLACE_PLACE##", resultHTML.ToString()));
                    }
                }

                string _template1 = "SoChiTiet_FourTotalTemplate.html",
                   _folderPath1 = @"Uploads\Html",
                   path1 = Path.Combine(Directory.GetCurrentDirectory(), _folderPath1, _template1),
                   _allText1 = System.IO.File.ReadAllText(path1);
                return _allText1.Replace("##REPLACE_PLACE_BODY##", _allTextFull.ToString());
            }
            catch
            {
                return string.Empty;
            }
        }

        private string ExportExcel_Report_SoChiTiet_Loai_1_And_2(LedgerReportModel p, LedgerReportParam param, double openingStock)
        {
            try
            {
                if (p == null) return string.Empty;
                if (param.FromMonth == null) param.FromMonth = 0;
                if (param.ToMonth == null) param.ToMonth = 0;
                if (param.FromDate == null) param.FromDate = DateTime.Now;
                if (param.ToDate == null) param.ToDate = DateTime.Now;
                string sTenFile = "BaoCaoChiTiet1.xlsx";
                int nCol = 9;
                if (param.BookDetailType == (int)ReportBookDetailTypeEnum.soCoDu_2_ben)
                {
                    nCol = 10;
                    sTenFile = "BaoCaoChiTiet2.xlsx";
                }
                else if (param.BookDetailType == (int)ReportBookDetailTypeEnum.soCoNgoaiTe)
                {
                    nCol = 13;
                    sTenFile = "BaoCaoChiTiet3.xlsx";
                }
                else if (param.BookDetailType == (int)ReportBookDetailTypeEnum.soCoHangTonKho)
                {
                    nCol = 13;
                    sTenFile = "BaoCaoChiTiet4.xlsx";
                }
                else if (param.BookDetailType == (int)ReportBookDetailTypeEnum.soQuy)
                {
                    nCol = 9;
                    sTenFile = "BaoCaoChiTiet5.xlsx";
                }
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/" + sTenFile);
                using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
                {
                    using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        worksheet.Cells["A1"].Value = p.Company;
                        worksheet.Cells["A2"].Value = p.Address;
                        worksheet.Cells["A3"].Value = p.TaxId;

                        worksheet.Cells["A5"].Value = "SỔ CHI TIẾT";
                        worksheet.Cells["A6"].Value = "Từ tháng ... đến tháng ... năm ... ";

                        worksheet.Cells["A7"].Value = "Tài khoản: " + param.AccountCode + ": " + p.AccountCode + " - " + p.AccountName;

                        worksheet.Cells["A8"].Value = "Đơn vị tính: Đồng";

                        worksheet.Cells["F8"].Value = "Lũy kế đầu năm: " + openingStock.ToString("#,##0;");
                        int currentRowNo = 10, flagRowNo = 0, nRowBegin = 10;

                        if (param.BookDetailType == (int)ReportBookDetailTypeEnum.soCoNgoaiTe)
                            ImportDataToExcel_Loai3(worksheet, p, ref currentRowNo);
                        else if (param.BookDetailType == (int)ReportBookDetailTypeEnum.soQuy)
                            ImportDataToExcel_Loai5(worksheet, p, ref currentRowNo);
                        else
                            ImportDataToExcel(worksheet, p, param.BookDetailType, ref currentRowNo);

                        flagRowNo = currentRowNo;

                        IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
                    {
                        { "TIEU_DE","SỔ CHI TIẾT "+ p.AccountName.ToUpper() },
                        { "TenCongTy", p.Company },
                        { "DiaChi", p.Address },
                        { "MST", p.TaxId },
                        { "NgayChungTu", string.Empty },
                        { "TaiKhoanAccount", p.AccountCode+" - "+p.AccountName },
                        { "TuThang", ((param.FromMonth > 0 && param.ToMonth > 0) ? ((int)param.FromMonth) : ((DateTime)param.FromDate).Month ).ToString("D2")   },
                        { "DenThang", ( (param.FromMonth > 0 && param.ToMonth > 0) ? ((int)param.ToMonth) : ((DateTime)param.ToDate).Month ).ToString("D2") },
                        { "Nam", ((param.FromMonth > 0 && param.ToMonth > 0) ? DateTime.Now.Year : ((DateTime)param.FromDate).Year ).ToString("D4") },
                        { "NguoiLap", !string.IsNullOrEmpty(param.LedgerReportMaker) ? param.LedgerReportMaker : string.Empty },
                        { "Ngay", " .... " },
                        { "Thang",  " .... " },
                        { "NamSign",  " .... " },
                        { "KeToanTruong", param.isCheckName ? p.ChiefAccountantName : string.Empty },
                        { "GiamDoc", param.isCheckName ? p.CEOName : string.Empty },
                        { "KeToanTruong_CV", p.ChiefAccountantNote},
                            { "GiamDoc_CV",  p.CEONote},
                    };

                        currentRowNo++;
                        worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Merge = true;
                        worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Value = "Ngày ... tháng ... năm";

                        currentRowNo++;
                        worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Merge = true;
                        worksheet.Cells[currentRowNo, 4, currentRowNo, 5].Merge = true;
                        worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Merge = true;
                        worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Value = "Người ghi sổ";
                        worksheet.Cells[currentRowNo, 4, currentRowNo, 5].Value = v_dicFixed["KeToanTruong_CV"];
                        worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Value = v_dicFixed["GiamDoc_CV"];

                        currentRowNo += 4;
                        worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Merge = true;
                        worksheet.Cells[currentRowNo, 4, currentRowNo, 5].Merge = true;
                        worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Merge = true;
                        worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Value = v_dicFixed["NguoiLap"];
                        worksheet.Cells[currentRowNo, 4, currentRowNo, 5].Value = v_dicFixed["KeToanTruong"];
                        worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Value = v_dicFixed["GiamDoc"];

                        currentRowNo--;
                        if (currentRowNo > 10)
                        {
                            worksheet.Cells[nRowBegin, 7, flagRowNo, nCol].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }

                        return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "SoChiTietLoai");
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private string ExportExcel_Report_SoChiTiet_Loai_6(LedgerReportModel p, LedgerReportParam param)
        {
            try
            {
                if (p == null) return string.Empty;
                if (param.FromMonth == null) param.FromMonth = 0;
                if (param.ToMonth == null) param.ToMonth = 0;
                if (param.FromDate == null) param.FromDate = DateTime.Now;
                if (param.ToDate == null) param.ToDate = DateTime.Now;
                string sTenFile = "BaoCaoChiTiet6.xlsx";
                int nCol = 8;

                string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/" + sTenFile);
                using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
                {
                    using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        worksheet.Cells["A1"].Value = p.Company;
                        worksheet.Cells["A2"].Value = p.Address;
                        worksheet.Cells["A3"].Value = p.TaxId;

                        worksheet.Cells["A5"].Value = "SỔ CHI TIẾT";
                        worksheet.Cells["A6"].Value = "Từ tháng ... đến tháng ... năm ... ";

                        worksheet.Cells["A7"].Value = "Tài khoản: " + param.AccountCode + ": " + p.AccountCode + " - " + p.AccountName;

                        worksheet.Cells["A8"].Value = "Đơn vị tính: Đồng";

                        int currentRowNo = 10, flagRowNo = 0, nRowBegin = 10;

                        ImportDataToExcel_Loai6(worksheet, p, ref currentRowNo);

                        flagRowNo = currentRowNo;

                        IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
                    {
                        { "TIEU_DE","SỔ CHI TIẾT "+ p.AccountName.ToUpper() },
                        { "TenCongTy", p.Company },
                        { "DiaChi", p.Address },
                        { "MST", p.TaxId },
                        { "NgayChungTu", string.Empty },
                        { "TaiKhoanAccount", p.AccountCode+" - "+p.AccountName },
                        { "TuThang", ((param.FromMonth > 0 && param.ToMonth > 0) ? ((int)param.FromMonth) : ((DateTime)param.FromDate).Month ).ToString("D2")   },
                        { "DenThang", ( (param.FromMonth > 0 && param.ToMonth > 0) ? ((int)param.ToMonth) : ((DateTime)param.ToDate).Month ).ToString("D2") },
                        { "Nam", ((param.FromMonth > 0 && param.ToMonth > 0) ? DateTime.Now.Year : ((DateTime)param.FromDate).Year ).ToString("D4") },
                        { "NguoiLap", !string.IsNullOrEmpty(param.LedgerReportMaker) ? param.LedgerReportMaker : string.Empty },
                        { "Ngay", " .... " },
                        { "Thang",  " .... " },
                        { "NamSign",  " .... " },
                        { "KeToanTruong", param.isCheckName ? p.ChiefAccountantName : string.Empty },
                        { "GiamDoc", param.isCheckName ? p.CEOName : string.Empty },
                        { "KeToanTruong_CV", p.ChiefAccountantNote},
                        { "GiamDoc_CV", p.CEONote},
                    };

                        currentRowNo++;
                        worksheet.Cells[currentRowNo, 4, currentRowNo, nCol].Merge = true;
                        worksheet.Cells[currentRowNo, 4, currentRowNo, nCol].Value = "Ngày ... tháng ... năm";

                        currentRowNo++;
                        worksheet.Cells[currentRowNo, 1, currentRowNo, 2].Merge = true;
                        worksheet.Cells[currentRowNo, 3, currentRowNo, 4].Merge = true;
                        worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Merge = true;
                        worksheet.Cells[currentRowNo, 1].Value = "Người ghi sổ";
                        worksheet.Cells[currentRowNo, 3].Value = v_dicFixed["KeToanTruong_CV"];
                        worksheet.Cells[currentRowNo, 5].Value = v_dicFixed["GiamDoc_CV"];

                        currentRowNo += 4;
                        worksheet.Cells[currentRowNo, 1, currentRowNo, 2].Merge = true;
                        worksheet.Cells[currentRowNo, 3, currentRowNo, 4].Merge = true;
                        worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Merge = true;
                        worksheet.Cells[currentRowNo, 1, currentRowNo, 2].Value = v_dicFixed["NguoiLap"];
                        worksheet.Cells[currentRowNo, 3, currentRowNo, 4].Value = v_dicFixed["KeToanTruong"];
                        worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Value = v_dicFixed["GiamDoc"];

                        currentRowNo--;
                        if (currentRowNo > 10)
                        {
                            worksheet.Cells[nRowBegin, 5, flagRowNo, nCol].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }

                        return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "SoChiTietLoai");
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private string ExportExcel_Report_SoChiTiet_Loai_4(LedgerReportModel p, LedgerReportParam param)
        {
            try
            {
                if (p == null) return string.Empty;
                if (param.FromMonth == null) param.FromMonth = 0;
                if (param.ToMonth == null) param.ToMonth = 0;
                if (param.FromDate == null) param.FromDate = DateTime.Now;
                if (param.ToDate == null) param.ToDate = DateTime.Now;
                string sTenFile = "BaoCaoChiTiet4.xlsx";
                int nCol = 13;

                string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/" + sTenFile);
                using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
                {
                    using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                        worksheet.Cells["A1"].Value = p.Company;
                        worksheet.Cells["A2"].Value = p.Address;
                        worksheet.Cells["A3"].Value = p.TaxId;

                        worksheet.Cells["A5"].Value = "SỔ CHI TIẾT";
                        worksheet.Cells["A6"].Value = "Từ tháng ... đến tháng ... năm ... ";

                        worksheet.Cells["A7"].Value = "Tài khoản: " + param.AccountCode + ": " + p.AccountCode + " - " + p.AccountName;

                        worksheet.Cells["A8"].Value = "Đơn vị tính: Đồng";

                        int currentRowNo = 10, flagRowNo = 0, nRowBegin = 10;

                        ImportDataToExcel_Loai4(worksheet, p, ref currentRowNo);

                        flagRowNo = currentRowNo;

                        IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
                    {
                        { "TIEU_DE","SỔ CHI TIẾT "+ p.AccountName.ToUpper() },
                        { "TenCongTy", p.Company },
                        { "DiaChi", p.Address },
                        { "MST", p.TaxId },
                        { "NgayChungTu", string.Empty },
                        { "TaiKhoanAccount", p.AccountCode+" - "+p.AccountName },
                        { "TuThang", ((param.FromMonth > 0 && param.ToMonth > 0) ? ((int)param.FromMonth) : ((DateTime)param.FromDate).Month ).ToString("D2")   },
                        { "DenThang", ( (param.FromMonth > 0 && param.ToMonth > 0) ? ((int)param.ToMonth) : ((DateTime)param.ToDate).Month ).ToString("D2") },
                        { "Nam", ((param.FromMonth > 0 && param.ToMonth > 0) ? DateTime.Now.Year : ((DateTime)param.FromDate).Year ).ToString("D4") },
                        { "NguoiLap", !string.IsNullOrEmpty(param.LedgerReportMaker) ? param.LedgerReportMaker : string.Empty },
                        { "Ngay", " .... " },
                        { "Thang",  " .... " },
                        { "NamSign",  " .... " },
                        { "KeToanTruong", param.isCheckName ? p.ChiefAccountantName : string.Empty },
                        { "GiamDoc", param.isCheckName ? p.CEOName : string.Empty },
                        { "KeToanTruong_CV", p.ChiefAccountantNote},
                        { "GiamDoc_CV", p.CEONote},
                    };

                        currentRowNo++;
                        worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Merge = true;
                        worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Value = "Ngày ... tháng ... năm";

                        currentRowNo++;
                        worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Merge = true;
                        worksheet.Cells[currentRowNo, 4, currentRowNo, 5].Merge = true;
                        worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Merge = true;
                        worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Value = "Người ghi sổ";
                        worksheet.Cells[currentRowNo, 4, currentRowNo, 5].Value = v_dicFixed["KeToanTruong_CV"];
                        worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Value = v_dicFixed["GiamDoc_CV"];

                        currentRowNo += 4;
                        worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Merge = true;
                        worksheet.Cells[currentRowNo, 4, currentRowNo, 5].Merge = true;
                        worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Merge = true;
                        worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Value = v_dicFixed["NguoiLap"];
                        worksheet.Cells[currentRowNo, 4, currentRowNo, 5].Value = v_dicFixed["KeToanTruong"];
                        worksheet.Cells[currentRowNo, 6, currentRowNo, 8].Value = v_dicFixed["GiamDoc"];

                        currentRowNo--;
                        if (currentRowNo > 10)
                        {
                            worksheet.Cells[nRowBegin, 7, flagRowNo, nCol].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                            worksheet.Cells[nRowBegin, 1, flagRowNo, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        }

                        return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "SoChiTietLoai");
                    }
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        private void ImportDataToExcel_Loai6(ExcelWorksheet _excel, LedgerReportModel p, ref int _currentRow)
        {
            int _ind = 0;
            var pItems = p.ItemSLTons;
            foreach (LedgerReportTonSLViewModel _k in pItems)
            {
                _currentRow++;
                _ind++;

                _excel.Cells[_currentRow, 1].Value = _ind;
                _excel.Cells[_currentRow, 2].Value = _k.Warehouse;
                _excel.Cells[_currentRow, 3].Value = string.IsNullOrEmpty(_k.Detail2) ? _k.Detail1 : _k.Detail2;
                _excel.Cells[_currentRow, 4].Value = _k.NameGood;
                _excel.Cells[_currentRow, 5].Value = _k.OpenQuantity;
                _excel.Cells[_currentRow, 6].Value = _k.InputQuantity;
                _excel.Cells[_currentRow, 7].Value = _k.OutputQuantity;
                _excel.Cells[_currentRow, 8].Value = _k.OpenQuantity + _k.InputQuantity - _k.OutputQuantity;
            }
        }

        private void ImportDataToExcel(ExcelWorksheet _excel, LedgerReportModel p, int bookDetailType, ref int _currentRow)
        {
            int _ind = -1;

            var pItems = p.BookDetails;
            foreach (SoChiTietViewModel _k in pItems)
            {
                _ind++;
                _currentRow++;
                _excel.Cells[_currentRow, 1].Value = _k.OrginalBookDate.HasValue ? _k.OrginalBookDate.Value.ToString("dd/MM") : string.Empty;
                _excel.Cells[_currentRow, 2].Value = _k.OrginalVoucherNumber;
                _excel.Cells[_currentRow, 3].Value = _k.OrginalBookDate.HasValue ? _k.OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                _excel.Cells[_currentRow, 4].Value = _k.Description;
                _excel.Cells[_currentRow, 5].Value = _k.IsDebit ? _k.CreditCode : _k.DebitCode;
                _excel.Cells[_currentRow, 6].Value = _k.DetailCode;
                _excel.Cells[_currentRow, 7].Value = _k.ArisingDebit > 0 ? _k.ArisingDebit : 0;
                _excel.Cells[_currentRow, 8].Value = _k.ArisingCredit > 0 ? _k.ArisingCredit : 0;
                _excel.Cells[_currentRow, 9].Value = _k.ResidualDebit > 0 ? _k.ResidualDebit : 0;
                if (bookDetailType == (int)ReportBookDetailTypeEnum.soCoDu_2_ben)
                    _excel.Cells[_currentRow, 10].Value = _k.ResidualCredit > 0 ? _k.ResidualCredit : 0;

                SoChiTietViewModel _sct = pItems[_ind + 1 < pItems.Count ? _ind + 1 : pItems.Count - 1];
                if (((_sct.Month + "" + _sct.Year) != (_k.Month + "" + _k.Year) && _ind < pItems.Count) || (_ind == pItems.Count - 1) && p.SumItem_SCT_ThuChi.ContainsKey(_k.Temp))
                {
                    List<SoChiTietThuChiViewModel> _ledgerSum = p.SumItem_SCT_ThuChi[_k.Temp];

                    SoChiTietThuChiViewModel _CongPhatSinh = _ledgerSum.FirstOrDefault(x => x.Month == -1) ?? new SoChiTietThuChiViewModel();
                    SoChiTietThuChiViewModel _LuyKe = _ledgerSum.FirstOrDefault(x => x.Month == -2) ?? new SoChiTietThuChiViewModel();

                    string _month = _k.Month.ToString("00") + "/" + _k.Year.ToString();

                    _currentRow++;
                    _excel.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
                    _excel.Cells[_currentRow, 1, _currentRow, 4].Value = "Cộng phát sinh tháng " + _month;
                    _excel.Cells[_currentRow, 5].Value = string.Empty;
                    _excel.Cells[_currentRow, 6].Value = string.Empty;
                    _excel.Cells[_currentRow, 7].Value = _CongPhatSinh.Thu_Amount;
                    _excel.Cells[_currentRow, 8].Value = _CongPhatSinh.Chi_Amount;
                    _excel.Cells[_currentRow, 9].Value = _CongPhatSinh.Residual_Amount > 0 ? _CongPhatSinh.Residual_Amount : 0;
                    if (bookDetailType == (int)ReportBookDetailTypeEnum.soCoDu_2_ben)
                        _excel.Cells[_currentRow, 10].Value = _CongPhatSinh.Residual_Amount < 0 ? _CongPhatSinh.Residual_Amount : 0;

                    _currentRow++;
                    _excel.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
                    _excel.Cells[_currentRow, 1, _currentRow, 4].Value = "Lũy kế phát sinh đến cuối kỳ";
                    _excel.Cells[_currentRow, 5].Value = string.Empty;
                    _excel.Cells[_currentRow, 6].Value = string.Empty;
                    _excel.Cells[_currentRow, 7].Value = _LuyKe.Thu_Amount;
                    _excel.Cells[_currentRow, 8].Value = _LuyKe.Chi_Amount;
                    _excel.Cells[_currentRow, 9].Value = _LuyKe.Residual_Amount > 0 ? _LuyKe.Residual_Amount : 0;
                    if (bookDetailType == (int)ReportBookDetailTypeEnum.soCoDu_2_ben)
                        _excel.Cells[_currentRow, 10].Value = _LuyKe.Residual_Amount < 0 ? _LuyKe.Residual_Amount : 0;
                }
            }
        }

        private void ImportDataToExcel_Loai3(ExcelWorksheet _excel, LedgerReportModel p, ref int _currentRow)
        {
            int _ind = -1;
            var pItems = p.BookDetails;
            foreach (SoChiTietViewModel _k in pItems)
            {
                _currentRow++;
                _ind++;

                _excel.Cells[_currentRow, 1].Value = _k.OrginalBookDate.HasValue ? _k.OrginalBookDate.Value.ToString("dd/MM") : string.Empty;
                _excel.Cells[_currentRow, 2].Value = _k.OrginalVoucherNumber;
                _excel.Cells[_currentRow, 3].Value = _k.OrginalBookDate.HasValue ? _k.OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                _excel.Cells[_currentRow, 4].Value = _k.Description;
                _excel.Cells[_currentRow, 5].Value = _k.IsDebit ? _k.CreditCode : _k.DebitCode;
                _excel.Cells[_currentRow, 6].Value = _k.DetailCode;
                _excel.Cells[_currentRow, 7].Value = _k.ExchangeRate;
                _excel.Cells[_currentRow, 8].Value = _k.IsDebit ? _k.OrginalCurrency : 0;
                _excel.Cells[_currentRow, 9].Value = _k.IsDebit ? _k.Amount : 0;
                _excel.Cells[_currentRow, 10].Value = !_k.IsDebit ? _k.OrginalCurrency : 0;
                _excel.Cells[_currentRow, 11].Value = !_k.IsDebit ? _k.Amount : 0;
                _excel.Cells[_currentRow, 12].Value = _k.ResidualAmount_Foreign;
                _excel.Cells[_currentRow, 13].Value = _k.ResidualAmount_OrginalCur;
                SoChiTietViewModel _sct = p.BookDetails[_ind + 1 < p.BookDetails.Count ? _ind + 1 : p.BookDetails.Count - 1];
                if (((_sct.Month + "" + _sct.Year) != (_k.Month + "" + _k.Year) && _ind < p.BookDetails.Count) || (_ind == p.BookDetails.Count - 1) && p.SumItem_SCT_ThuChi.ContainsKey(_k.Temp))
                {
                    List<SoChiTietThuChiViewModel> _ledgerSum = p.SumItem_SCT_ThuChi[_k.Temp];

                    SoChiTietThuChiViewModel _CongPhatSinh = _ledgerSum.FirstOrDefault(x => x.Month == -1) ?? new SoChiTietThuChiViewModel();
                    SoChiTietThuChiViewModel _LuyKe = _ledgerSum.FirstOrDefault(x => x.Month == -2) ?? new SoChiTietThuChiViewModel();
                    SoChiTietThuChiViewModel _Du = _ledgerSum.FirstOrDefault(x => x.Month == -3) ?? new SoChiTietThuChiViewModel();

                    string _monthb = _k.Month.ToString("00") + "/" + _k.Year.ToString();

                    _currentRow++;
                    _excel.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
                    _excel.Cells[_currentRow, 1, _currentRow, 4].Value = "Cộng phát sinh tháng " + _monthb;
                    _excel.Cells[_currentRow, 5].Value = string.Empty;
                    _excel.Cells[_currentRow, 6].Value = string.Empty;
                    _excel.Cells[_currentRow, 7].Value = string.Empty;
                    _excel.Cells[_currentRow, 8].Value = _CongPhatSinh.ArisingDebit_Foreign;
                    _excel.Cells[_currentRow, 9].Value = _CongPhatSinh.Thu_Amount;
                    _excel.Cells[_currentRow, 10].Value = _CongPhatSinh.ArisingCredit_Foreign;
                    _excel.Cells[_currentRow, 11].Value = _CongPhatSinh.Chi_Amount;
                    _excel.Cells[_currentRow, 12].Value = 0;
                    _excel.Cells[_currentRow, 13].Value = 0;

                    _currentRow++;
                    _excel.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
                    _excel.Cells[_currentRow, 1, _currentRow, 4].Value = "Lũy kế phát sinh đến cuối kỳ";
                    _excel.Cells[_currentRow, 5].Value = string.Empty;
                    _excel.Cells[_currentRow, 6].Value = string.Empty;
                    _excel.Cells[_currentRow, 7].Value = string.Empty;
                    _excel.Cells[_currentRow, 8].Value = _LuyKe.ArisingDebit_Foreign;
                    _excel.Cells[_currentRow, 9].Value = _LuyKe.Thu_Amount;
                    _excel.Cells[_currentRow, 10].Value = _LuyKe.ArisingCredit_Foreign;
                    _excel.Cells[_currentRow, 11].Value = _LuyKe.Chi_Amount;
                    _excel.Cells[_currentRow, 12].Value = 0;
                    _excel.Cells[_currentRow, 13].Value = 0;

                    _currentRow++;
                    _excel.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
                    _excel.Cells[_currentRow, 1, _currentRow, 4].Value = "Dư cuối " + _monthb;
                    _excel.Cells[_currentRow, 5].Value = string.Empty;
                    _excel.Cells[_currentRow, 6].Value = string.Empty;
                    _excel.Cells[_currentRow, 7].Value = string.Empty;
                    _excel.Cells[_currentRow, 8].Value = 0;
                    _excel.Cells[_currentRow, 9].Value = 0;
                    _excel.Cells[_currentRow, 10].Value = 0;
                    _excel.Cells[_currentRow, 11].Value = 0;
                    _excel.Cells[_currentRow, 12].Value = _Du.ResidualAmount_Foreign;
                    _excel.Cells[_currentRow, 13].Value = _Du.ResidualAmount_OrginalCur;
                }
            }
        }

        private void ImportDataToExcel_Loai5(ExcelWorksheet _excel, LedgerReportModel p, ref int _currentRow)
        {
            int _ind = -1;
            var pItems = p.BookDetails;
            foreach (SoChiTietViewModel _k in pItems)
            {
                _currentRow++;
                _ind++;
                _excel.Cells[_currentRow, 1].Value = _k.OrginalBookDate.HasValue ? _k.OrginalBookDate.Value.ToString("dd/MM") : string.Empty;
                _excel.Cells[_currentRow, 2].Value = _k.OrginalVoucherNumber;
                _excel.Cells[_currentRow, 3].Value = _k.OrginalBookDate.HasValue ? _k.OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                _excel.Cells[_currentRow, 4].Value = _k.NameOfPerson;
                _excel.Cells[_currentRow, 5].Value = _k.Description;
                _excel.Cells[_currentRow, 6].Value = _k.IsDebit ? _k.CreditCode : _k.DebitCode;
                _excel.Cells[_currentRow, 7].Value = _k.IsDebit ? _k.Thu_Amount : 0;
                _excel.Cells[_currentRow, 8].Value = !_k.IsDebit ? _k.Chi_Amount : 0;
                _excel.Cells[_currentRow, 9].Value = _k.Residual_Amount;
                SoChiTietViewModel _sct = p.BookDetails[_ind + 1 < p.BookDetails.Count ? _ind + 1 : p.BookDetails.Count - 1];
                if (((_sct.Month + "" + _sct.Year) != (_k.Month + "" + _k.Year) && _ind < p.BookDetails.Count) || (_ind == p.BookDetails.Count - 1) && p.SumItem_SCT_ThuChi.ContainsKey(_k.Temp))
                {
                    List<SoChiTietThuChiViewModel> _ledgerSum = p.SumItem_SCT_ThuChi[_k.Temp];

                    SoChiTietThuChiViewModel _CongPhatSinh = _ledgerSum.FirstOrDefault(x => x.Month == -1) ?? new SoChiTietThuChiViewModel();
                    SoChiTietThuChiViewModel _LuyKe = _ledgerSum.FirstOrDefault(x => x.Month == -2) ?? new SoChiTietThuChiViewModel();

                    string _monthb = _k.Month.ToString("00") + "/" + _k.Year.ToString();

                    _currentRow++;
                    _excel.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
                    _excel.Cells[_currentRow, 1, _currentRow, 4].Value = "Cộng phát sinh tháng " + _monthb;
                    _excel.Cells[_currentRow, 5].Value = string.Empty;
                    _excel.Cells[_currentRow, 6].Value = string.Empty;
                    _excel.Cells[_currentRow, 7].Value = _CongPhatSinh.Thu_Amount;
                    _excel.Cells[_currentRow, 8].Value = _CongPhatSinh.Chi_Amount;
                    _excel.Cells[_currentRow, 9].Value = _CongPhatSinh.Residual_Amount;

                    _currentRow++;
                    _excel.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
                    _excel.Cells[_currentRow, 1, _currentRow, 4].Value = "Lũy kế phát sinh đến cuối kỳ";
                    _excel.Cells[_currentRow, 5].Value = string.Empty;
                    _excel.Cells[_currentRow, 6].Value = string.Empty;
                    _excel.Cells[_currentRow, 7].Value = _LuyKe.Thu_Amount;
                    _excel.Cells[_currentRow, 8].Value = _LuyKe.Chi_Amount;
                    _excel.Cells[_currentRow, 9].Value = _LuyKe.Residual_Amount;
                }
            }
        }


        private void ImportDataToExcel_Loai4(ExcelWorksheet _excel, LedgerReportModel p, ref int _currentRow)
        {
            foreach (var item in p.listAccoutCodeThuChi)
            {
                string[] listKey = item.Key.Split('-');
                string detail1 = listKey[0];
                string detail2 = listKey[1];
                string warehouseCode = listKey[2];
                int _ind = -1;
                var listBookDetails = p.BookDetails.Where(x => string.IsNullOrEmpty(detail1) || (x.DebitDetailCodeFirst == detail1 || x.CreditDetailCodeFirst == detail1)
                                        && string.IsNullOrEmpty(detail2) || (x.DebitDetailCodeSecond == detail2 || x.CreditDetailCodeSecond == detail2)
                                        && string.IsNullOrEmpty(warehouseCode) || (x.CreditWarehouseCode == warehouseCode || x.DebitWarehouseCode == warehouseCode)
                                        ).ToList();

                foreach (SoChiTietViewModel _k in listBookDetails)
                {
                    _currentRow++;
                    _ind++;

                    _excel.Cells[_currentRow, 1].Value = _k.OrginalBookDate.HasValue ? _k.OrginalBookDate.Value.ToString("dd/MM") : string.Empty;
                    _excel.Cells[_currentRow, 2].Value = _k.OrginalVoucherNumber;
                    _excel.Cells[_currentRow, 3].Value = _k.OrginalBookDate.HasValue ? _k.OrginalBookDate.Value.ToString("dd/MM/yyyy") : string.Empty;
                    _excel.Cells[_currentRow, 4].Value = string.IsNullOrEmpty(detail2) ? detail1 : detail2;
                    _excel.Cells[_currentRow, 5].Value = _k.IsDebit ? _k.CreditCode : _k.DebitCode;
                    _excel.Cells[_currentRow, 6].Value = _k.DetailCode;
                    _excel.Cells[_currentRow, 7].Value = _k.UnitPrice;
                    _excel.Cells[_currentRow, 8].Value = _k.IsDebit ? _k.Quantity : 0;
                    _excel.Cells[_currentRow, 9].Value = _k.IsDebit ? _k.Amount : 0;
                    _excel.Cells[_currentRow, 10].Value = !_k.IsDebit ? _k.Quantity : 0;
                    _excel.Cells[_currentRow, 11].Value = !_k.IsDebit ? _k.Amount : 0;
                    _excel.Cells[_currentRow, 12].Value = _k.Residual_Quantity;
                    _excel.Cells[_currentRow, 13].Value = _k.Residual_Amount;
                    SoChiTietViewModel _sct = listBookDetails[_ind + 1 < listBookDetails.Count ? _ind + 1 : listBookDetails.Count - 1];
                    if ((_sct.Month != _k.Month && _ind < listBookDetails.Count) || (_ind == listBookDetails.Count - 1))
                    {
                        List<SoChiTietThuChiViewModel> _ledgerSum = item.Value.Where(y => y.Month == _k.Month).ToList();

                        SoChiTietThuChiViewModel _CongPhatSinh = _ledgerSum.FirstOrDefault(y => y.Type == 1);
                        if (_CongPhatSinh == null)
                            _CongPhatSinh = new SoChiTietThuChiViewModel();
                        SoChiTietThuChiViewModel _LuyKe = _ledgerSum.FirstOrDefault(y => y.Type == 2);
                        if (_LuyKe == null)
                            _LuyKe = new SoChiTietThuChiViewModel();
                        SoChiTietThuChiViewModel _Du = _ledgerSum.FirstOrDefault(y => y.Type == 3);
                        if (_Du == null)
                            _Du = new SoChiTietThuChiViewModel();

                        string _monthb = _k.Month.ToString("00") + "/" + _k.Year.ToString();

                        _currentRow++;
                        _excel.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
                        _excel.Cells[_currentRow, 1, _currentRow, 4].Value = "Cộng phát sinh tháng " + _monthb;
                        _excel.Cells[_currentRow, 5].Value = string.Empty;
                        _excel.Cells[_currentRow, 6].Value = string.Empty;
                        _excel.Cells[_currentRow, 7].Value = string.Empty;
                        _excel.Cells[_currentRow, 8].Value = _CongPhatSinh.Input_Quantity;
                        _excel.Cells[_currentRow, 9].Value = _CongPhatSinh.Thu_Amount;
                        _excel.Cells[_currentRow, 10].Value = _CongPhatSinh.Output_Quantity;
                        _excel.Cells[_currentRow, 11].Value = _CongPhatSinh.Chi_Amount;
                        _excel.Cells[_currentRow, 12].Value = 0;
                        _excel.Cells[_currentRow, 13].Value = 0;

                        _currentRow++;
                        _excel.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
                        _excel.Cells[_currentRow, 1, _currentRow, 4].Value = "Lũy kế phát sinh đến cuối kỳ";
                        _excel.Cells[_currentRow, 5].Value = string.Empty;
                        _excel.Cells[_currentRow, 6].Value = string.Empty;
                        _excel.Cells[_currentRow, 7].Value = string.Empty;
                        _excel.Cells[_currentRow, 8].Value = _LuyKe.Input_Quantity;
                        _excel.Cells[_currentRow, 9].Value = _LuyKe.Thu_Amount;
                        _excel.Cells[_currentRow, 10].Value = _LuyKe.Output_Quantity;
                        _excel.Cells[_currentRow, 11].Value = _LuyKe.Chi_Amount;
                        _excel.Cells[_currentRow, 12].Value = 0;
                        _excel.Cells[_currentRow, 13].Value = 0;

                        _currentRow++;
                        double SR_DC_IP_DG = 0;
                        if (_Du.Residual_Quantity > 0)
                            SR_DC_IP_DG = Math.Abs(_Du.Residual_Amount) / Math.Abs(_Du.Residual_Quantity);
                        _excel.Cells[_currentRow, 1, _currentRow, 4].Merge = true;
                        _excel.Cells[_currentRow, 1, _currentRow, 4].Value = "Dư cuối " + _monthb;
                        _excel.Cells[_currentRow, 5].Value = string.Empty;
                        _excel.Cells[_currentRow, 6].Value = string.Empty;
                        _excel.Cells[_currentRow, 7].Value = SR_DC_IP_DG;
                        _excel.Cells[_currentRow, 8].Value = 0;
                        _excel.Cells[_currentRow, 9].Value = 0;
                        _excel.Cells[_currentRow, 10].Value = 0;
                        _excel.Cells[_currentRow, 11].Value = 0;
                        _excel.Cells[_currentRow, 12].Value = _Du.Residual_Quantity;
                        _excel.Cells[_currentRow, 13].Value = _Du.Residual_Amount;
                        _currentRow++;
                    }
                }
            }
        }
    }
}
