using DinkToPdf.Contracts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Accounts;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.Reports;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ManageEmployee.Services.Reports;


public class SavedMovedMoneyReportService : ISavedMovedMoneyReportService
{
    private readonly IConverter converterPDF;
    private readonly ICompanyService _companyService;
    private readonly IAccountBalanceSheetService _accountBalanceSheetService;
    public SavedMovedMoneyReportService(
        IConverter _converPDF, ICompanyService companyService, IAccountBalanceSheetService accountBalanceSheetService)
    {
        converterPDF = _converPDF;
        _companyService = companyService;
        _accountBalanceSheetService = accountBalanceSheetService;
    }

    public async Task<string> ExportDataReport(SavedMoneyReportParam request, int year, bool isNoiBo = false)
    {
        try
        {
            DateTime dtFrom = new DateTime(), dtTo = new DateTime();
            if (request == null) return string.Empty;
            if (request.FromMonth > 0) dtFrom = new DateTime(DateTime.Now.Year, (int)request.FromMonth, 1);
            if (request.ToMonth > 0)
            {
                dtTo = new DateTime(DateTime.Now.Year, (int)request.ToMonth, 1).AddMonths(1).AddDays(-1);
            }
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
                VoteMaker = request.VoteMaker,
                NoteChiefAccountantName = company.NoteOfChiefAccountant,
                CEOOfName = company.NameOfCEO,
                CEOOfNote = company.NoteOfCEO,

            };

            List<SaveMovedModelBase> _lstModelBaseReport = GetModelOnjectReport();

            List<AccountBalanceItemModel> _lstAccount = await _accountBalanceSheetService
                       .GenerateReport(dtFrom, dtTo, year, isNoiBo: isNoiBo);

            //lũy kế là tồn đầu kỳ
            foreach (SaveMovedModelBase _modeBase in _lstModelBaseReport)
            {
                if(_modeBase.code == "10")
                {
                    var _acc01 = _lstModelBaseReport.FirstOrDefault(x => x.code == "01");
                    var _acc02 = _lstModelBaseReport.FirstOrDefault(x => x.code == "02");
                    _modeBase.this_year = (_acc01?.this_year ?? 0) - (_acc02?.this_year ?? 0);
                    _modeBase.prev_year = (_acc01?.prev_year ?? 0) - (_acc02?.prev_year ?? 0);
                    _modeBase.accumulated_start_year = (_acc01?.accumulated_start_year ?? 0) - (_acc02?.accumulated_start_year ?? 0);
                    continue;
                }
                else if (_modeBase.code == "20")
                {
                    var _acc01 = _lstModelBaseReport.FirstOrDefault(x => x.code == "10");
                    var _acc02 = _lstModelBaseReport.FirstOrDefault(x => x.code == "11");
                    _modeBase.this_year = (_acc01?.this_year ?? 0) - (_acc02?.this_year ?? 0);
                    _modeBase.prev_year = (_acc01?.prev_year ?? 0) - (_acc02?.prev_year ?? 0);
                    _modeBase.accumulated_start_year = _acc01.accumulated_start_year - _acc02.accumulated_start_year;
                    continue;
                }
                else if (_modeBase.code == "30")
                {
                    var _acc20 = _lstModelBaseReport.FirstOrDefault(x => x.code == "20");
                    var _acc21 = _lstModelBaseReport.FirstOrDefault(x => x.code == "21");
                    var _acc22 = _lstModelBaseReport.FirstOrDefault(x => x.code == "22");
                    var _acc25 = _lstModelBaseReport.FirstOrDefault(x => x.code == "25");
                    var _acc26 = _lstModelBaseReport.FirstOrDefault(x => x.code == "26");
                    _modeBase.this_year = (_acc20?.this_year ?? 0) + ((_acc21?.this_year ?? 0) - (_acc22?.this_year ?? 0)) - ((_acc25?.this_year ?? 0) + (_acc26?.this_year ?? 0));
                    _modeBase.prev_year = (_acc20?.prev_year ?? 0) + ((_acc21?.prev_year ?? 0) - (_acc22?.prev_year ?? 0)) - ((_acc25?.prev_year ?? 0) + (_acc26?.prev_year ?? 0));
                    _modeBase.accumulated_start_year = _acc20.accumulated_start_year + (_acc21.accumulated_start_year - _acc22.accumulated_start_year) - (_acc25.accumulated_start_year + _acc26.accumulated_start_year);
                    continue;
                }
                else if (_modeBase.code == "40")
                {
                    var _acc01 = _lstModelBaseReport.FirstOrDefault(x => x.code == "31");
                    var _acc02 = _lstModelBaseReport.FirstOrDefault(x => x.code == "32");
                    _modeBase.this_year = _acc01.this_year - _acc02.this_year;
                    _modeBase.prev_year = _acc01.prev_year - _acc02.prev_year;
                    _modeBase.accumulated_start_year = _acc01.accumulated_start_year - _acc02.accumulated_start_year;
                    continue;
                }
                else if (_modeBase.code == "50")
                {
                    var _acc01 = _lstModelBaseReport.FirstOrDefault(x => x.code == "30");
                    var _acc02 = _lstModelBaseReport.FirstOrDefault(x => x.code == "40");
                    _modeBase.this_year = _acc01.this_year + _acc02.this_year;
                    _modeBase.prev_year = _acc01.prev_year + _acc02.prev_year;
                    _modeBase.accumulated_start_year = _acc01.accumulated_start_year + _acc02.accumulated_start_year;
                    continue;
                }
                else if (_modeBase.code == "60")
                {
                    var _acc01 = _lstModelBaseReport.FirstOrDefault(x => x.code == "50");
                    var _acc02 = _lstModelBaseReport.FirstOrDefault(x => x.code == "51");
                    var _acc03 = _lstModelBaseReport.FirstOrDefault(x => x.code == "52");
                    _modeBase.this_year = _acc01.this_year - _acc02.this_year - _acc03.this_year;
                    _modeBase.prev_year = _acc01.prev_year - _acc02.prev_year - _acc03.prev_year;
                    _modeBase.accumulated_start_year = _acc01.accumulated_start_year - _acc02.accumulated_start_year - _acc03.accumulated_start_year;
                    continue;
                }
                if(_modeBase.items != null && _modeBase.items.Count == 2)
                {
                    foreach(SaveMovedModelBase itemCon in _modeBase.items)
                    {
                        Calculator_Report(itemCon, _lstAccount, isNoiBo);
                    }
                    _modeBase.this_year = _modeBase.items[0].this_year - _modeBase.items[1].this_year;
                }
                Calculator_Report(_modeBase, _lstAccount, isNoiBo);
            }


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

    private string ConvertToHTML(List<SaveMovedModelBase> _modelRequest, VoucherReportViewModel p, int? fromMonth, int? toMonth, DateTime? fromDate, DateTime? toDate, string voucherType, string preparedBy, bool isFillName)
    {
        try
        {
            if (p == null) return string.Empty;
            if (fromMonth == null) fromMonth = 0;
            if (toMonth == null) toMonth = 0;
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;

            string _template = "BangLuanChuyenTienTeTemplate.html",
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
                { "MaxNgay", p.Items.Count > 0 ? p.Items.Max(x => x.Date).ToString("dd/MM/yyyy") : string.Empty },
                { "TuThang", ((fromMonth > 0 && toMonth > 0) ? ((int)fromMonth) : ((DateTime)fromDate).Month ).ToString("D2")   },
                { "DenThang", ( (fromMonth > 0 && toMonth > 0) ? ((int)toMonth) : ((DateTime)toDate).Month ).ToString("D2") },
                { "Nam", ((fromMonth > 0 && toMonth > 0) ? DateTime.Now.Year : ((DateTime)fromDate).Year ).ToString("D4") },
                { "NguoiLap", !string.IsNullOrEmpty(preparedBy) ? preparedBy : string.Empty },
                { "KeToanTruong", isFillName ? p.ChiefAccountantName : string.Empty },
                { "KeToanTruong_CV", p.NoteChiefAccountantName},
                { "GiamDoc", isFillName ? p.CEOOfName : string.Empty },
                { "GiamDoc_CV", p.CEOOfNote},
                { "Ngay", "......" },
                { "Thang", "......" },
                { "NamSign", "......" },
                { "TONG_TK_NO", string.Empty },
                { "TONG_SOTIEN_NO", string.Format("{0:N0}", p.Items.Sum(x => x.DebitTotalAmount) ) },
                { "TONG_TK_CO", string.Empty },
                { "TONG_SOTIEN_CO", string.Format("{0:N0}", p.Items.Sum(x => x.CreditTotalAmount) ) },
            };

            v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));

            if (_modelRequest.Count > 0)
            {
                _modelRequest.ForEach(x =>
                {
                    string _debitHtml = @"<tr>
                                                    <td>{{{CHI_TIEU}}}</td>
                                                    <td>{{{MA_SO}}}</td>
                                                    <td>{{{T.M}}}</td>
                                                    <td class='txt-right'>{{{NAM_NAY}}}</td>
                                                    <td class='txt-right'>{{{NAM_TRUOC}}}</td>
                                                    <td class='txt-right'>{{{LUY_KE}}}</td>
                                                </tr>";
                    _debitHtml = _debitHtml
                        .Replace("{{{CHI_TIEU}}}", x.title)
                        .Replace("{{{MA_SO}}}", x.code)
                        .Replace("{{{T.M}}}", x.subs)
                        .Replace("{{{NAM_NAY}}}", x.this_year != 0 ? String.Format("{0:N0}", x.this_year) : string.Empty)
                        .Replace("{{{NAM_TRUOC}}}", x.prev_year != 0 ? String.Format("{0:N0}", x.prev_year) : string.Empty)
                        .Replace("{{{LUY_KE}}}", x.accumulated_start_year != 0 ? String.Format("{0:N0}", x.accumulated_start_year) : string.Empty)
                        ;

                    resultStr += _debitHtml;
                    if(x.code != "51" && x.code != "52" && x.items != null && x.items.Count > 0)
                    {
                        x.items.ForEach(y =>
                        {
                            string _debitHtmly = @"<tr>
                                                    <td>{{{CHI_TIEU}}}</td>
                                                    <td>{{{MA_SO}}}</td>
                                                    <td>{{{T.M}}}</td>
                                                    <td class='txt-right'>{{{NAM_NAY}}}</td>
                                                    <td class='txt-right'>{{{NAM_TRUOC}}}</td>
                                                    <td class='txt-right'>{{{LUY_KE}}}</td>
                                                </tr>";
                            _debitHtmly = _debitHtmly
                                .Replace("{{{CHI_TIEU}}}", y.title)
                                .Replace("{{{MA_SO}}}", y.code)
                                .Replace("{{{T.M}}}", y.subs)
                                .Replace("{{{NAM_NAY}}}", y.this_year != 0 ? String.Format("{0:N0}", y.this_year) : string.Empty)
                                .Replace("{{{NAM_TRUOC}}}", y.prev_year != 0 ? String.Format("{0:N0}", y.prev_year) : string.Empty)
                                .Replace("{{{LUY_KE}}}", y.accumulated_start_year != 0 ? String.Format("{0:N0}", y.accumulated_start_year) : string.Empty)
                                ;

                            resultStr += _debitHtmly;
                        });
                    }

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

    private string ConvertToPDFFile(List<SaveMovedModelBase> _modelRequest, VoucherReportViewModel p, int? fromMonth, int? toMonth, DateTime? fromDate, DateTime? toDate, string voucherType, string preparedBy, bool isFillName)
    {
        try
        {
            string _allText = ConvertToHTML(_modelRequest, p, fromMonth, toMonth, fromDate, toDate, voucherType, preparedBy, isFillName);
            return ExcelHelpers.ConvertUseDink(_allText, converterPDF, Directory.GetCurrentDirectory(), "BangLuuChuyenTienTe");
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ExportExcel_Report(List<SaveMovedModelBase> _modelRequest, VoucherReportViewModel p, int? fromMonth, int? toMonth,string preparedBy, bool isFillName)
    {
        try
        {
            if (p == null) return string.Empty;
            if (fromMonth == null) fromMonth = 0;
            if (toMonth == null) toMonth = 0;
            string sTenFile = "BangLuuChuyenTienTe.xlsx";
            int nCol = 6;
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/" + sTenFile);
            using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    worksheet.Cells["A1:F1"].Value = p.Company;
                    worksheet.Cells["A2:F2"].Value = p.Address;
                    worksheet.Cells["A3:F3"].Value = p.TaxId;
                    worksheet.Cells[7,1].Value = $"Từ tháng {fromMonth} đến tháng {toMonth}" ;

                    int currentRowNo = 8, flagRowNo = 8;


                    for (int i = 0; i < _modelRequest.Count; i++)
                    {
                        currentRowNo++;

                        SaveMovedModelBase _mBase = _modelRequest[i];
                        worksheet.Cells[currentRowNo, 1].Value = _mBase.title;
                        worksheet.Cells[currentRowNo, 2].Value = _mBase.code;
                        worksheet.Cells[currentRowNo, 3].Value = _mBase.subs;
                        worksheet.Cells[currentRowNo, 4].Value = _mBase.this_year;
                        worksheet.Cells[currentRowNo, 5].Value = _mBase.prev_year;
                        worksheet.Cells[currentRowNo, 6].Value = _mBase.accumulated_start_year;

                        currentRowNo = _mBase.items == null ? currentRowNo++ : (_mBase.items.Count == 0 ? currentRowNo++ : currentRowNo);

                        if (_mBase.items.Count == 1 )
                        {
                            foreach (SaveMovedModelBase _itemY in _mBase.items)
                            {
                                currentRowNo++;
                                worksheet.Cells[currentRowNo, 1].Value = _itemY.title;
                                worksheet.Cells[currentRowNo, 2].Value = _itemY.code;
                                worksheet.Cells[currentRowNo, 3].Value = _itemY.subs;
                                worksheet.Cells[currentRowNo, 4].Value = _itemY.this_year;
                                worksheet.Cells[currentRowNo, 5].Value = _itemY.prev_year;
                                worksheet.Cells[currentRowNo, 6].Value = _itemY.accumulated_start_year;
                            }
                        }
                    }


                    currentRowNo += 2;
                    worksheet.Cells[currentRowNo, 4, currentRowNo, 6].Merge = true;
                    worksheet.Cells[currentRowNo, 4, currentRowNo, 6].Value = "Ngày ..... tháng ..... năm ..... ";
                    worksheet.Cells[currentRowNo, 4, currentRowNo, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    currentRowNo++;
                    worksheet.Cells[currentRowNo, 1, currentRowNo, 2].Merge = true;
                    worksheet.Cells[currentRowNo, 1, currentRowNo, 2].Value = "Người lập phiếu";
                    worksheet.Cells[currentRowNo, 1, currentRowNo, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[currentRowNo, 3, currentRowNo, 4].Merge = true;
                    worksheet.Cells[currentRowNo, 3, currentRowNo, 4].Value = p.NoteChiefAccountantName;
                    worksheet.Cells[currentRowNo, 3, currentRowNo, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Merge = true;
                    worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Value = p.CEOOfNote;
                    worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    currentRowNo+= 4;
                    worksheet.Cells[currentRowNo, 1, currentRowNo, 2].Merge = true;
                    worksheet.Cells[currentRowNo, 1, currentRowNo, 2].Value = (!string.IsNullOrEmpty(preparedBy) ? preparedBy : string.Empty);
                    worksheet.Cells[currentRowNo, 1, currentRowNo, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[currentRowNo, 3, currentRowNo, 4].Merge = true;
                    worksheet.Cells[currentRowNo, 3, currentRowNo, 4].Value = isFillName ? p.ChiefAccountantName : "";
                    worksheet.Cells[currentRowNo, 3, currentRowNo, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Merge = true;
                    worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Value = isFillName ? p.CEOOfName : "";
                    worksheet.Cells[currentRowNo, 5, currentRowNo, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    
                    currentRowNo = currentRowNo - 8;
                    if (currentRowNo > 8)
                    {
                        worksheet.Cells[flagRowNo, 4, currentRowNo, 6].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                        worksheet.Cells[flagRowNo, 1, currentRowNo, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        worksheet.Cells[flagRowNo, 1, currentRowNo, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        worksheet.Cells[flagRowNo, 1, currentRowNo, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        worksheet.Cells[flagRowNo, 1, currentRowNo, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                        worksheet.Cells[flagRowNo, 1, currentRowNo, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[flagRowNo, 1, currentRowNo, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[flagRowNo, 1, currentRowNo, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[flagRowNo, 1, currentRowNo, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }

                    return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "BangLuuChuyenTienTe");
                }
            }

        }
        catch
        {
            return string.Empty;
        }
    }

    #endregion



    #region ULITIES

    public void Calculator_Report(SaveMovedModelBase _modelBase, List<AccountBalanceItemModel> _accBalance, bool isNoiBo)
    {

        AccountBalanceItemModel _accSingle = _accBalance.FirstOrDefault(x => x.AccountCode == _modelBase.debit_code);
        
        if (_modelBase.code == "01")
        {
            _modelBase.this_year = _accSingle?.ArisingDebit ?? 0;
            if (!isNoiBo)
                _modelBase.prev_year = _accSingle?.OpeningDebit ?? 0;
            else
                _modelBase.prev_year = _accSingle?.OpeningDebitNB ?? 0;

            _modelBase.accumulated_start_year = _accSingle?.CumulativeDebit ?? 0;
            return;
        }
        AccountBalanceItemModel accCredit = _accBalance.FirstOrDefault(x => x.AccountCode == _modelBase.credit_code);

        if (_accSingle != null && accCredit != null)
        {
            if (_accSingle.ArisingDebit > accCredit.ArisingCredit)
                _modelBase.this_year = accCredit.ArisingCredit;
            else
                _modelBase.this_year = _accSingle.ArisingDebit;
            if (!isNoiBo)
            {
                if (_accSingle.OpeningDebit > accCredit.OpeningCredit)
                    _modelBase.prev_year = accCredit.OpeningCredit;
                else
                    _modelBase.prev_year = _accSingle.OpeningDebit;
            }
            else
            {
                if (_accSingle.OpeningDebitNB > accCredit.OpeningCreditNB)
                    _modelBase.prev_year = accCredit.OpeningCreditNB;
                else
                    _modelBase.prev_year = _accSingle.OpeningDebitNB;
            }
            if (_accSingle.CumulativeDebit > accCredit.CumulativeCredit)
                _modelBase.accumulated_start_year = accCredit.CumulativeCredit;
            else
                _modelBase.accumulated_start_year = _accSingle.CumulativeDebit;
        }
    }

    /// <summary>
    /// Get model base report from json file
    /// </summary>
    /// <returns></returns>
    public List<SaveMovedModelBase> GetModelOnjectReport()
    {
        string _template = "SavedMovedMoneyReport.json",
            _folderPath = @"Uploads\Json",
            path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
            _allText = System.IO.File.ReadAllText(path);
        List<SaveMovedModelBase> _objBase = JsonConvert.DeserializeObject<List<SaveMovedModelBase>>(_allText);
        setDefaultInit(_objBase);
        return _objBase;
    }

    private void setDefaultInit(List<SaveMovedModelBase> p)
    {
        foreach (SaveMovedModelBase k in p)
        {
            k.prev_year = 0;
            k.accumulated_start_year = 0;
            k.this_year = 0;
            if (k.items != null)
            {
                setDefaultInit(k.items);
            }
        }
    }
    #endregion

}
