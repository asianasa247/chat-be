using Common.Extensions;
using DinkToPdf.Contracts;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.Reports;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Reports;
public class ReportDebitCustomerService: IReportDebitCustomerService
{
    private readonly ApplicationDbContext _context;
    private readonly ICompanyService _companyService;
    private readonly IConverter _converterPDF;

    public ReportDebitCustomerService(ICompanyService companyService, IConverter converterPDF, ApplicationDbContext context)
    {
        _companyService = companyService;
        _converterPDF = converterPDF;
        _context = context;
    }
    public async Task<string> ReportAsyn(LedgerReportParam _param, int year)
    {
        try
        {
            DateTime dtFrom, dtTo;
            if (string.IsNullOrEmpty(_param.AccountCodeDetail1)) _param.AccountCodeDetail1 = string.Empty;
            _param.AccountCodeDetail1 = _param.AccountCodeDetail1.Trim();
            if (string.IsNullOrEmpty(_param.AccountCodeDetail2)) _param.AccountCodeDetail2 = string.Empty;
            _param.AccountCodeDetail2 = _param.AccountCodeDetail2.Trim();
            if (_param.FilterType == 1)
            {
                dtFrom = new DateTime(year, _param.FromMonth.Value, 1);
                _param.FromDate = dtFrom;
                dtTo = new DateTime(year, _param.ToMonth.Value, 1);
                dtTo = dtTo.AddMonths(1);
                _param.ToDate = dtTo;
            }
            else
            {
                dtFrom = _param.FromDate.Value;
                dtTo = _param.ToDate.Value;
            }

            if (string.IsNullOrEmpty(_param.AccountCode) || string.IsNullOrEmpty(_param.AccountCodeDetail1))
                return string.Empty;

            ChartOfAccount account;
            if(!string.IsNullOrEmpty(_param.AccountCodeDetail2))
                account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == _param.AccountCodeDetail2 && x.ParentRef == (_param.AccountCode+":"+ _param.AccountCodeDetail1));
            else
                account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == _param.AccountCodeDetail1 && x.ParentRef == _param.AccountCode);
            if(account is null)
                return string.Empty;

            if (_param.FromDate.Value.Year != year)
            {
                var accountOld = await _context.ChartOfAccounts.FirstOrDefaultAsync(x => x.Code == account.Code && x.ParentRef == account.ParentRef
                                && (string.IsNullOrEmpty(account.WarehouseCode) || x.WarehouseCode == account.WarehouseCode) && x.Year == _param.FromDate.Value.Year);
                account.OpeningDebit = accountOld?.OpeningDebit;
                account.OpeningCredit = accountOld?.OpeningCredit;
                account.OpeningStockQuantity = accountOld?.OpeningStockQuantity;

                account.OpeningDebitNB = accountOld?.OpeningDebitNB;
                account.OpeningCreditNB = accountOld?.OpeningCreditNB;
                account.OpeningStockQuantityNB = accountOld?.OpeningStockQuantityNB;
            }
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.DebitCode == _param.AccountCode
                                    && (string.IsNullOrEmpty(_param.AccountCodeDetail1) || x.DebitDetailCodeFirst == _param.AccountCodeDetail1)
                                    && (string.IsNullOrEmpty(_param.AccountCodeDetail2) || x.DebitDetailCodeSecond == _param.AccountCodeDetail2));
            if (customer is null)
                return string.Empty;

            var _company = await _companyService.GetCompany();

            var ledgers = await _context.GetLedgerNotForYear(_param.IsNoiBo ? 3 : 2).Where(x => 
                            (x.CreditCode == _param.AccountCode || x.DebitCode == _param.AccountCode)
                            && (string.IsNullOrEmpty(_param.AccountCodeDetail1) || x.CreditDetailCodeFirst == _param.AccountCodeDetail1 || x.DebitDetailCodeFirst == _param.AccountCodeDetail1)
                            && (string.IsNullOrEmpty(_param.AccountCodeDetail2) || x.CreditDetailCodeSecond == _param.AccountCodeDetail2 || x.DebitDetailCodeSecond == _param.AccountCodeDetail2)
                            && x.OrginalBookDate >= _param.FromDate && x.OrginalBookDate < _param.ToDate).ToListAsync();

            ReportDebitCustomer _model = new()
            {
                Address = _company.Address,
                Company = _company.Name,
                MethodCalcExportPrice = _company.MethodCalcExportPrice,
                TaxId = _company.MST,
                CEOName = _company.NameOfCEO,
                ChiefAccountantName = _company.NameOfChiefAccountant,
                CEONote = _company.NoteOfCEO,
                ChiefAccountantNote = _company.NoteOfChiefAccountant,
                CustomerAddress = customer?.Address,
                CustomerName = customer?.Name,
                CustomerCode = customer?.Code,
                CustomerPhone = customer?.Phone,
                InputAmount = (account.OpeningDebit ?? 0) - (account.OpeningCredit ?? 0),
                ArisingAmountIncrease = ledgers.Where(x => x.DebitCode == _param.AccountCode
                                && (string.IsNullOrEmpty(_param.AccountCodeDetail1) || x.DebitDetailCodeFirst == _param.AccountCodeDetail1)
                                && (string.IsNullOrEmpty(_param.AccountCodeDetail2) || x.DebitDetailCodeSecond == _param.AccountCodeDetail2)
                            ).Sum(x => x.Amount) ,
                ArisingAmountDecrease = ledgers.Where(x => x.CreditCode == _param.AccountCode
                                && (string.IsNullOrEmpty(_param.AccountCodeDetail1) || x.CreditDetailCodeFirst == _param.AccountCodeDetail1)
                                && (string.IsNullOrEmpty(_param.AccountCodeDetail2) || x.CreditDetailCodeSecond == _param.AccountCodeDetail2)
                            ).Sum(x => x.Amount),
               
            };
            _model.EndDebit = _model.InputAmount + _model.ArisingAmountIncrease - _model.ArisingAmountDecrease; 
            _model.AccumulatedDebit = (account.OpeningDebit ?? 0) + _model.ArisingAmountIncrease;
            _model.AccumulatedCredit = (account.OpeningCredit ?? 0) + _model.ArisingAmountDecrease;
            return ExportDataReport_SoChiTiet(_model, _param);
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ExportDataReport_SoChiTiet(ReportDebitCustomer ledgers, LedgerReportParam param)
    {
        try
        {
            string _path = string.Empty;
            switch (param.FileType)
            {
                case "html":
                    _path = ConvertToHTML(ledgers, param);
                    break;

                //case "excel":
                //    _path = ExportExcel_Report_SoChiTiet_PhanMau(ledgers, param, _accountGet.OpeningStock);
                //    break;

                case "pdf":
                    _path = ConvertToPDFFile(ledgers, param);
                    break;
            }
            return _path;
        }
        catch
        {
            return string.Empty;
        }
    }
    private string ConvertToHTML(ReportDebitCustomer p, LedgerReportParam param)
    {
        try
        {
            if (p == null) 
                return string.Empty;

            string soThapPhan = "N" + p.MethodCalcExportPrice;

            string _template = "BienBanCongNo.html",
                _folderPath = @"Uploads\Html", resultStr = string.Empty,
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = System.IO.File.ReadAllText(path), resultHTML = string.Empty;

            IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TenCongTy", p.Company },
                { "DiaChi", p.Address },
                { "MST", p.TaxId },
                { "MaKhachHang", p.CustomerCode },
                { "TenKhachHang", p.CustomerName },
                { "DiaChiKhachHang", p.CustomerAddress },
                { "PhoneKhachHang", p.CustomerPhone },
                { "CongNoTonDauKy", p.InputAmount > 0 ? String.Format("{0:" + soThapPhan + "}", p.InputAmount) : string.Empty },
                { "PhatSinhTangTrongKy", p.ArisingAmountIncrease  > 0 ? String.Format("{0:" + soThapPhan + "}", p.ArisingAmountIncrease) : string.Empty },
                { "PhaiThuTuBanHang", p.ArisingAmountIncrease  > 0 ? String.Format("{0:" + soThapPhan + "}", p.ArisingAmountIncrease) : string.Empty },
                { "PhaiThuKhac", string.Empty  },
                { "PhatSinhGiamTrongKy", p.ArisingAmountDecrease  > 0 ? String.Format("{0:" + soThapPhan + "}", p.ArisingAmountDecrease) : string.Empty  },
                { "ThanhToanTrongKy", p.ArisingAmountDecrease  > 0 ? String.Format("{0:" + soThapPhan + "}", p.ArisingAmountDecrease) : string.Empty  },
                { "ChietKhauThanhToan", string.Empty  },
                { "GiamKhac", string.Empty  },
                { "CongNoCuoiKy", p.EndDebit  > 0 ? String.Format("{0:" + soThapPhan + "}", p.EndDebit) : string.Empty  },
                { "PhatSinhTangCongNoLuyKe", p.AccumulatedDebit  > 0 ? String.Format("{0:" + soThapPhan + "}", p.AccumulatedDebit) : string.Empty  },
                { "PhatSinhGiamCongNoLuyKe", p.AccumulatedCredit  > 0 ? String.Format("{0:" + soThapPhan + "}", p.AccumulatedCredit) : string.Empty  },
                { "SoTienVietBangChu", p.EndDebit  > 0 ?  (AmountExtension.ConvertFromDecimal(p.EndDebit) +" VND") : string.Empty},
                { "NguoiLap", !string.IsNullOrEmpty(param.LedgerReportMaker) ? param.LedgerReportMaker : string.Empty },
                { "KeToanTruong", param.isCheckName ? p.ChiefAccountantName : string.Empty },
                { "GiamDoc", param.isCheckName ? p.CEOName : string.Empty },
            };
            v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));
            
            return _allText;
        }
        catch
        {
            return string.Empty;
        }
    }

    private string ConvertToPDFFile(ReportDebitCustomer p, LedgerReportParam param)
    {
        try
        {
            string _allText = ConvertToHTML(p, param);
            return ExcelHelpers.ConvertUseDinkLandscape(_allText, _converterPDF, Directory.GetCurrentDirectory(), "BienBanCongNo");
        }
        catch
        {
            return string.Empty;
        }
    }

}
