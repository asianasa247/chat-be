using DinkToPdf.Contracts;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Accounts;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.Reports;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace ManageEmployee.Services.Reports;
public class BalanceSheetService : IBalanceSheetService
{
    private readonly ApplicationDbContext _context;
    private readonly IConverter converterPDF;
    private readonly ICompanyService _companyService;
    private readonly IAccountBalanceSheetService _accountBalanceSheetService;
    public BalanceSheetService(ApplicationDbContext context, IConverter _converPDF, ICompanyService companyService,
        IAccountBalanceSheetService accountBalanceSheetService
        )
    {
        _context = context;
        converterPDF = _converPDF;
        _companyService = companyService;
        _accountBalanceSheetService = accountBalanceSheetService;
    }
    public async Task<string> ExportDataAccountantBalance(LedgerReportParam _param, int year, bool isNoiBo = false)
    {
        try
        {
            DateTime dtFrom, dtTo;
            if (_param.FromMonth == null) _param.FromMonth = 0;
            if (_param.ToMonth == null) _param.ToMonth = 0;
            if (_param.FromDate == null) _param.FromDate = new DateTime();
            if (_param.ToDate == null) _param.ToDate = new DateTime();
            if (string.IsNullOrEmpty(_param.AccountCode)) _param.AccountCode = string.Empty;
            _param.AccountCode = _param.AccountCode.Trim();
            string _path = string.Empty;
            if (_param.FromMonth > 0)
            {
                dtFrom = new DateTime(DateTime.Now.Year, (int)_param.FromMonth, 1);
                dtTo = new DateTime(DateTime.Now.Year, (int)_param.ToMonth, 1).AddMonths(1).AddDays(-1);
            }
            else
            {
                dtFrom = (DateTime)_param.FromDate;
                dtTo = (DateTime)_param.ToDate;
            }

            var company = await _companyService.GetCompany();
            var modelCompanyInfo = new BalanceAccountantReportVM()
            {
                Company = company.Name,
                Address = company.Address,
                TaxId = company.MST,
                Type = _param.VoucherType,
                TypeName = _param.VoucherType,
                ChiefAccountantName = company.NameOfChiefAccountant,
                CEOName = company.NameOfCEO,
                VoteMaker = string.Empty,
                ChiefAccountantNote = company.NoteOfChiefAccountant,
                CEONote = company.NoteOfCEO,
            };

            List<AccountBalanceItemModel> _lst_AccReport = await  _accountBalanceSheetService.GenerateReport(dtFrom, dtTo, year, isNoiBo: isNoiBo);

            List<AccoutantBalanceModelBase> p_Base = GetModelOnjectReport();

            List<AccountBalanceItemModel> _lst_AccReport_new = _lst_AccReport.Where(x => x.ParentRef != null).ToList();

            p_Base.ForEach(_modelBase =>
            {
                DeQuy(_modelBase, _lst_AccReport_new, isNoiBo);
            });
            p_Base.ForEach(_modelBase =>
            {
                DeQuyTinhTong(_modelBase);
            });

            switch (_param.FileType)
            {
                case "html":
                    _path = ConvertToHTML_AaccountantReport(p_Base, modelCompanyInfo, _param.FromMonth, _param.ToMonth, _param.FromDate, _param.ToDate, _param.VoucherType, _param.LedgerReportMaker, _param.isCheckName);
                    break;
                case "excel":
                    _path = ExportExcel_Report(p_Base, modelCompanyInfo, _param.FromMonth, _param.ToMonth, _param.FromDate, _param.ToDate, _param.VoucherType, _param.LedgerReportMaker, _param.isCheckName);
                    break;
                case "pdf":
                    _path = ConvertToPDFFile(p_Base, modelCompanyInfo, _param.FromMonth, _param.ToMonth, _param.FromDate, _param.ToDate, _param.VoucherType, _param.LedgerReportMaker, _param.isCheckName);
                    break;
            }
            return _path;
        }
        catch
        {
            return string.Empty;
        }
    }
    public void DeQuy(AccoutantBalanceModelBase itemParent, List<AccountBalanceItemModel> _lst_AccReport, bool isNoiBo = false)
    {
        if(itemParent.items == null || !itemParent.items.Any())
        {
            string _accCode = itemParent.codeAction;
            int accCodeLength = _accCode.Length;
            string duration = itemParent.duration;
            double _dn = 0, _dc = 0;
            
            List<AccountBalanceItemModel> listItemCheck = _lst_AccReport.Where(x => (((x.ParentRef.Length > accCodeLength) && x.ParentRef.Substring(0, accCodeLength) == _accCode )
                                ||(x.ParentRef == _accCode))
                                && (!x.HasChild|| x.AccountType == 5)
                                &&(_accCode != "338" || x.AccountCode != "3387")
                                && (string.IsNullOrEmpty(duration) || (x.Duration == duration))).ToList();

            AccountBalanceItemModel _v = _lst_AccReport.FirstOrDefault(x => x.AccountCode == _accCode && (string.IsNullOrEmpty(duration) || (x.Duration == duration)));
            
            if (_v == null)
            {
                _v = new AccountBalanceItemModel
                {
                    OpeningDebit = 0,
                    OpeningCredit = 0,
                    ClosingDebit = 0,
                    ClosingCredit = 0,

                    OpeningDebitNB = 0,
                    OpeningCreditNB = 0,
                };
            }
            if(!isNoiBo)
            {
                switch (itemParent.action)
                {
                    case ActionTypeConstants.Du_No:
                    case ActionTypeConstants.Tong_Du_No:
                        _dn = _v.OpeningDebit;
                        _dc = _v.ClosingDebit;
                        break;
                    case ActionTypeConstants.Du_No_Am:
                        _dn = -_v.OpeningDebit;
                        _dc = -_v.ClosingDebit;
                        break;
                    case ActionTypeConstants.Tong_Du_No_Chi_Tiet:
                        if (listItemCheck.Count == 0 && _v.AccountType < 5)
                        {
                            _dn = _v.OpeningDebit;
                            _dc = _v.ClosingDebit;
                            break;
                        }
                        _dn = listItemCheck.Sum(x => x.OpeningDebit);
                        _dc = listItemCheck.Sum(x => x.ClosingDebit);
                        break;
                    case ActionTypeConstants.Du_Co:
                    case ActionTypeConstants.Tong_Du_Co:
                        _dn = _v.OpeningCredit;
                        _dc = _v.ClosingCredit;
                        break;
                    case ActionTypeConstants.Du_Co_Am:
                        _dn = -_v.OpeningCredit;
                        _dc = -_v.ClosingCredit;
                        break;
                    case ActionTypeConstants.Tong_Du_Co_Chi_Tiet:
                        if (listItemCheck.Count == 0 && _v.AccountType < 5)
                        {
                            _dn = _v.OpeningCredit;
                            _dc = _v.ClosingCredit;
                            break;
                        }
                        _dn = listItemCheck.Sum(x => x.OpeningCredit);
                        _dc = listItemCheck.Sum(x => x.ClosingCredit);
                        break;
                }
            }
            else
            {
                switch (itemParent.action)
                {
                    case ActionTypeConstants.Du_No:
                    case ActionTypeConstants.Tong_Du_No:
                        _dn = _v.OpeningDebitNB;
                        _dc = _v.ClosingDebit;
                        break;
                    case ActionTypeConstants.Du_No_Am:
                        _dn = -_v.OpeningDebitNB;
                        _dc = -_v.ClosingDebit;
                        break;
                    case ActionTypeConstants.Tong_Du_No_Chi_Tiet:
                        if (listItemCheck.Count == 0 && _v.AccountType < 5)
                        {
                            _dn = _v.OpeningDebitNB;
                            _dc = _v.ClosingDebit;
                            break;
                        }
                        _dn = listItemCheck.Sum(x => x.OpeningDebitNB);
                        _dc = listItemCheck.Sum(x => x.ClosingDebit);
                        break;
                    case ActionTypeConstants.Du_Co:
                    case ActionTypeConstants.Tong_Du_Co:
                        _dn = _v.OpeningCreditNB;
                        _dc = _v.ClosingCredit;
                        break;
                    case ActionTypeConstants.Du_Co_Am:
                        _dn = -_v.OpeningCreditNB;
                        _dc = -_v.ClosingCredit;
                        break;
                    case ActionTypeConstants.Tong_Du_Co_Chi_Tiet:
                        if (listItemCheck.Count == 0 && _v.AccountType < 5)
                        {
                            _dn = _v.OpeningCreditNB;
                            _dc = _v.ClosingCredit;
                            break;
                        }
                        _dn = listItemCheck.Sum(x => x.OpeningCreditNB);
                        _dc = listItemCheck.Sum(x => x.ClosingCredit);
                        break;
                }
            }
            
            itemParent.sodaunam = _dn;
            itemParent.socuoinam = _dc;
        }
        else
        {
            foreach(AccoutantBalanceModelBase item in itemParent.items)
            {
                DeQuy(item, _lst_AccReport, isNoiBo);
            }
        }
    }

    public void DeQuyTinhTong(AccoutantBalanceModelBase itemParent)
    {
        if(itemParent.items != null && itemParent.items.Count > 0)
        {
            foreach (AccoutantBalanceModelBase item in itemParent.items)
            {
                DeQuyTinhTong(item);
            }
            itemParent.socuoinam = itemParent.items.Sum(x => x.socuoinam);
            itemParent.sodaunam = itemParent.items.Sum(x => x.sodaunam);
        }
    }

    #region EXPORT DATA
    private string ConvertToPDFFile(List<AccoutantBalanceModelBase> _accountBase, BalanceAccountantReportVM p, int? fromMonth, int? toMonth, DateTime? fromDate, DateTime? toDate, string voucherType, string preparedBy, bool isFillName)
    {
        try
        {
            string _allText = ConvertToHTML_AaccountantReport(_accountBase, p, fromMonth, toMonth, fromDate, toDate, voucherType, preparedBy, isFillName);
            return ExcelHelpers.ConvertUseDink(_allText, converterPDF, Directory.GetCurrentDirectory(), "BangCanDoiKeToan");
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ConvertToHTML_AaccountantReport(List<AccoutantBalanceModelBase> _accountBase, BalanceAccountantReportVM p, int? fromMonth, int? toMonth, DateTime? fromDate, DateTime? toDate, string voucherType, string preparedBy, bool isFillName)
    {
        try
        {


            if (p == null) return string.Empty;
            if (fromMonth == null) fromMonth = 0;
            if (toMonth == null) toMonth = 0;
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;

            string _template = "BangCanDoiKeToanTemplate.html",
                _folderPath = @"Uploads\Html", resultStr = string.Empty,
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = System.IO.File.ReadAllText(path);

            IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TenCongTy", p.Company },
                { "DiaChi", p.Address },
                { "MST", p.TaxId },
                { "NgayChungTu", string.Empty },
                { "TaiKhoanCT", string.Empty },
                { "TuThang",  ((fromMonth > 0 && toMonth > 0) ? ((int)fromMonth) : ((DateTime)fromDate).Month ).ToString("D2")   },
                { "DenThang", ( (fromMonth > 0 && toMonth > 0) ? ((int)toMonth) : ((DateTime)toDate).Month ).ToString("D2") },
                { "Nam", ((fromMonth > 0 && toMonth > 0) ? DateTime.Now.Year : ((DateTime)fromDate).Year ).ToString("D4") },
                { "NguoiLap", !string.IsNullOrEmpty(preparedBy) ? preparedBy : string.Empty },
                { "Ngay", " ..... " },
                { "Thang", " ..... " },
                { "NamSign", " ..... " },
                { "KeToanTruong", isFillName ? p.ChiefAccountantName : string.Empty },
                { "GiamDoc", isFillName ? p.CEOName : string.Empty },
                { "KeToanTruong_CV",p.ChiefAccountantNote},
                { "GiamDoc_CV", p.CEONote},

            };

            v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));

            resultStr = getHTMLData(_accountBase, _accountBase);


            _allText = _allText.Replace("##TOTAL_REPLACE_PLACE##", resultStr);

            return _allText;
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ExportExcel_Report(List<AccoutantBalanceModelBase> _accountBase, BalanceAccountantReportVM p, int? fromMonth, int? toMonth, DateTime? fromDate, DateTime? toDate, string voucherType, string preparedBy, bool isFillName)
    {
        try
        {
            if (p == null) return string.Empty;
            if (fromMonth == null) fromMonth = 0;
            if (toMonth == null) toMonth = 0;
            if (fromDate == null) fromDate = DateTime.Now;
            if (toDate == null) toDate = DateTime.Now;

            string sTenFile = "BangCanDoiKeToan.xlsx";
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads/Excel/" + sTenFile);
            using (FileStream templateDocumentStream = System.IO.File.OpenRead(path))
            {
                using (ExcelPackage package = new ExcelPackage(templateDocumentStream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int currentRowNo = 9, nRowBegin = 9, nCol = 6;

                    worksheet.Cells[1, 1].Value = p.Company;
                    worksheet.Cells[2, 1].Value = p.Address;
                    worksheet.Cells[3, 1].Value = p.TaxId;

                    foreach (AccoutantBalanceModelBase _base in _accountBase)
                    {
                        currentRowNo = AddRowData(worksheet, currentRowNo, _base, _accountBase);
                    }

                    currentRowNo = currentRowNo - 1;
                    if (currentRowNo > 9)
                    {
                        worksheet.Cells[nRowBegin, 5, currentRowNo, nCol].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

                        worksheet.Cells[nRowBegin, 1, currentRowNo, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        worksheet.Cells[nRowBegin, 1, currentRowNo, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        worksheet.Cells[nRowBegin, 1, currentRowNo, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;
                        worksheet.Cells[nRowBegin, 1, currentRowNo, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Medium;

                        worksheet.Cells[nRowBegin, 1, currentRowNo, nCol].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[nRowBegin, 1, currentRowNo, nCol].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[nRowBegin, 1, currentRowNo, nCol].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                        worksheet.Cells[nRowBegin, 1, currentRowNo, nCol].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
                    }

                    currentRowNo++;
                    worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Merge = true;
                    worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Value = "Người lập";
                    worksheet.Cells[currentRowNo, 4, currentRowNo, 6].Merge = true;
                    worksheet.Cells[currentRowNo, 4, currentRowNo, 6].Value = p.ChiefAccountantNote;
                    currentRowNo += 4;
                    worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Merge = true;
                    worksheet.Cells[currentRowNo, 1, currentRowNo, 3].Value = preparedBy;
                    worksheet.Cells[currentRowNo, 4, currentRowNo, 6].Merge = true;
                    worksheet.Cells[currentRowNo, 4, currentRowNo, 6].Value = string.IsNullOrEmpty(preparedBy) ? "" : p.ChiefAccountantNote;
                    return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "BangCanDoiKeToan");
                }
            }
        }
        catch
        {
            return string.Empty;
        }
    }

    #endregion

    private int AddRowData(ExcelWorksheet _excelWork, int _currentRow, AccoutantBalanceModelBase p, List<AccoutantBalanceModelBase> _accountBase)
    {
        try
        {

            if (p.code == "270")
            {
                p.sodaunam = _accountBase[0].sodaunam + _accountBase[1].sodaunam;
                p.socuoinam = _accountBase[0].socuoinam + _accountBase[1].socuoinam;
            }
            else if (p.code == "440")
            {
                p.sodaunam = _accountBase[2].sodaunam + _accountBase[3].sodaunam;
                p.socuoinam = _accountBase[2].socuoinam + _accountBase[3].socuoinam;
            }
            _excelWork.SelectedRange[_currentRow, 1, _currentRow, 2].Merge = true;
            _excelWork.SelectedRange[_currentRow, 1, _currentRow, 2].Value = p.title;
            _excelWork.SelectedRange[_currentRow, 3].Value = p.code;
            _excelWork.SelectedRange[_currentRow, 4].Value = p.subs;
            _excelWork.SelectedRange[_currentRow, 5].Value = p.sodaunam;
            _excelWork.SelectedRange[_currentRow, 6].Value = p.socuoinam;

            switch (p.type)
            {
                case (int)AccountantBalanceEnumModelBase.Level_One:
                case (int)AccountantBalanceEnumModelBase.Level_Two:
                case (int)AccountantBalanceEnumModelBase.Level_Sum:
                    _excelWork.SelectedRange[_currentRow, 1, _currentRow, 6].Style.Font.Bold = true;
                    break;
                default:
                    _excelWork.SelectedRange[_currentRow, 1, _currentRow, 6].Style.Font.Bold = false;
                    break;
            }

            _currentRow++;
            if (p.items != null)
            {
                foreach (var item in p.items)
                {
                    _currentRow = AddRowData(_excelWork, _currentRow, item, _accountBase);
                }
            }
            return _currentRow;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Get model base report from json file
    /// </summary>
    /// <returns></returns>
    private List<AccoutantBalanceModelBase> GetModelOnjectReport()
    {
        try
        {
            string _template = "AccountantBalance.json",
                _folderPath = @"Uploads\Json",
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = System.IO.File.ReadAllText(path);
            List<AccoutantBalanceModelBase> _objBase = JsonConvert.DeserializeObject<List<AccoutantBalanceModelBase>>(_allText);
            setDefaultInit(_objBase);
            return _objBase;
        }
        catch
        {
            return new List<AccoutantBalanceModelBase>();
        }
    }

    private void setDefaultInit(List<AccoutantBalanceModelBase> p)
    {
        foreach (AccoutantBalanceModelBase k in p)
        {
            k.sodaunam = 0;
            k.socuoinam = 0;
            if (k.items != null)
            {
                setDefaultInit(k.items);
            }
        }
    }

    private void setSumData_SumRow(List<AccoutantBalanceModelBase> p, List<AccoutantBalanceModelBase> _src)
    {
        foreach (AccoutantBalanceModelBase k in p)
        {
            if (k.type == (int)AccountantBalanceEnumModelBase.Level_Sum)
            {
                k.sodaunam = _src.Where(_d => _d.code == "400" || _d.code == "300").Sum(_q => _q.sodaunam);
                k.socuoinam = _src.Where(_d => _d.code == "400" || _d.code == "300").Sum(_q => _q.socuoinam);
            }
            else if (k.items != null && k.items.Count > 0)
                    setSumData_SumRow(k.items, _src);
        }
    }

    private string getHTMLData(List<AccoutantBalanceModelBase> p, List<AccoutantBalanceModelBase> _accountBase)
    {
        try
        {
            string _result = string.Empty;

            string _trEach = @"<tr class='#CLASS_BOLD#'>
                                        <td class='txt-left'>{{TAI_SAN}}</td>
                                        <td class='txt-left'>{{MA_SO}}</td>
                                        <td class='txt-left'>{{TM}}</td>
                                        <td class='txt-right'>{{SO_DAU_NAM}}</td>
                                        <td class='txt-right'>{{SO_CUOI_NAM}}</td>
                                    </tr>";

            foreach (AccoutantBalanceModelBase x in p)
            {
                if (x.code == "270")
                {
                    x.sodaunam = _accountBase[0].sodaunam + _accountBase[1].sodaunam;
                    x.socuoinam = _accountBase[0].socuoinam + _accountBase[1].socuoinam;
                }
                else if (x.code == "440")
                {
                    x.sodaunam = _accountBase[2].sodaunam + _accountBase[3].sodaunam;
                    x.socuoinam = _accountBase[2].socuoinam + _accountBase[3].socuoinam;
                }
                string _row = _trEach.Replace("{{TAI_SAN}}", x.title)
                    .Replace("{{MA_SO}}", x.code)
                    .Replace("{{TM}}", x.subs)
                    .Replace("{{SO_DAU_NAM}}", (x.sodaunam != 0 ? String.Format("{0:N0}", x.sodaunam) : string.Empty))
                    .Replace("{{SO_CUOI_NAM}}", (x.socuoinam != 0 ? String.Format("{0:N0}", x.socuoinam) : string.Empty));

                switch (x.type)
                {
                    case (int)AccountantBalanceEnumModelBase.Level_One:
                    case (int)AccountantBalanceEnumModelBase.Level_Two:
                    case (int)AccountantBalanceEnumModelBase.Level_Sum:
                        _row = _row.Replace("#CLASS_BOLD#", "font-b");
                        break;
                    default:
                        _row = _row.Replace("#CLASS_BOLD#", "font-normal");
                        break;
                }

                _result += _row;

                if (x.items != null && x.items.Count > 0)
                {
                    _result += getHTMLData(x.items, _accountBase);
                }
                
            }

            return _result;
        }
        catch
        {
            return string.Empty;
        }
    }
}
