using Common.Constants;
using DinkToPdf.Contracts;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Text;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Ledgers;
using ManageEmployee.Services.Interfaces.Reports;
using ManageEmployee.Services.Interfaces.Accounts;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.LedgerEntities;
using System.Linq.Expressions;
using Common.Extensions;
using ManageEmployee.Dal.Enums;
using System.Linq;

namespace ManageEmployee.Services.Reports;

public class LedgerDetailBookService : ILedgerDetailBookService
{
    private readonly ApplicationDbContext _context;
    private readonly ILedgerService _ledgerServices;
    private readonly ICompanyService _companyService;
    private readonly IAccountBalanceSheetService _accountBalanceSheet;
    private readonly IExportDataReport_SoChiTiet _exportDataReport_SoChiTiet;

    public LedgerDetailBookService(ApplicationDbContext context,
        ILedgerService ledgerServices, ICompanyService companyService,
        IAccountBalanceSheetService accountBalanceSheet, IExportDataReport_SoChiTiet exportDataReport_SoChiTiet)
    {
        _context = context;
        _ledgerServices = ledgerServices;
        _companyService = companyService;
        _accountBalanceSheet = accountBalanceSheet;
        _exportDataReport_SoChiTiet = exportDataReport_SoChiTiet;
    }


    public async Task<string> GetDataReport_SoChiTiet_Six(LedgerReportParamDetail _param, int year, string wareHouseCode = "")
    {
        try
        {
            var _accountFind = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == _param.AccountCode);
            ChartOfAccount accountChild = null;
            if (!string.IsNullOrEmpty(_param.AccountCodeDetail1))
            {
                accountChild = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == _param.AccountCodeDetail1
                                        && x.ParentRef == _param.AccountCode);
            }
            if (!string.IsNullOrEmpty(_param.AccountCodeDetail2))
            {
                var parentRefCode = _param.AccountCode + ":" + _param.AccountCodeDetail1;
                accountChild = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == _param.AccountCodeDetail2
                                        && x.ParentRef == parentRefCode);
            }

            var _company = await _companyService.GetCompany();

            List<LedgerReportTonSLViewModel> relationReturn = await _ledgerServices.GetDataReport_SoChiTiet_Six_data(_param, year, wareHouseCode);

            LedgerReportModel _model = new LedgerReportModel
            {
                InfoSum = null,
                Items = null,
                BookDetails = null,
                LedgerCalculator = null,
                ItemSLTons = relationReturn,
                Address = _company.Address,
                Company = _company.Name,
                MethodCalcExportPrice = _company.MethodCalcExportPrice,
                TaxId = _company.MST,
                CEOName = _company.NameOfCEO,
                ChiefAccountantName = _company.NameOfChiefAccountant,
                AccountCode = _accountFind?.Code,
                AccountName = _accountFind?.Name + (accountChild != null ? (" - " + accountChild.Name) : string.Empty),
                CEONote = _company.NoteOfCEO,
                ChiefAccountantNote = _company.NoteOfChiefAccountant,
            };

            return await _exportDataReport_SoChiTiet.ReportAsync(_model, _param, year);
        }
        catch
        {
            return string.Empty;
        }
    }

    public async Task<string> GetDataReport_SoChiTiet_Full(LedgerReportParamDetail _param, int year, bool isNoiBo = false)
    {
        try
        {
            DateTime dtFrom, dtTo;

            _param.AccountCodeDetail1 = _param.AccountCodeDetail1?.Trim() ?? string.Empty;

            var _accountFind = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == _param.AccountCode);

            ChartOfAccount accountChild = await GetAccountChild(_param, year);

            var _company = await _companyService.GetCompany();

            if (_param.FilterType == 1)
            {
                dtFrom = new DateTime(year, _param.FromMonth.Value, 1);
                dtTo = new DateTime(year, _param.ToMonth.Value, 1);
                dtTo = dtTo.AddMonths(1);
                _param.FromDate = dtFrom;
                _param.ToDate = dtTo;
            }
            else
            {
                dtFrom = new DateTime(_param.FromDate.Value.Year, _param.FromDate.Value.Month, _param.FromDate.Value.Day);
                dtTo = new DateTime(_param.ToDate.Value.Year, _param.ToDate.Value.Month, _param.ToDate.Value.Day).AddDays(1);
            }

            var dtFromBefore = new DateTime(_param.FromDate.Value.Year, 1, 1);

            if (string.IsNullOrEmpty(_param.AccountCode))
            {
                return string.Empty;
            }

            List<SoChiTietViewModel> relations = await GetRelations(_param, isNoiBo, dtFrom, dtTo);

            List<SumSoChiTietViewModel> listLedgerBefore = await GetLedgersBefore(_param, isNoiBo, dtFrom, dtFromBefore);

            LedgerReportCalculatorIO _OpeningBackLog = await CalculatorFollowMonth_ThuChi(_param, year);

            double _dauKy_Thu = GetDauKyThu(_param, listLedgerBefore);

            double _dauKy_Chi = DauKyChi(_param, listLedgerBefore);

            double _luyKe_PS_No_NT = 0;
            double _luyKe_PS_No_VND = 0;
            double _luyKe_PS_Co_NT = 0;
            double _luyKe_PS_Co_VND = 0;

            var _accountGet = await _accountBalanceSheet.
               GenerateAccrualAccounting("date", _param.FromDate, _param.ToDate, _param.AccountCode, _param.AccountCodeDetail1, _param.AccountCodeDetail2, _param.IsNoiBo);

            GetSamplOpeningBackLog(isNoiBo, _OpeningBackLog, _dauKy_Thu, _dauKy_Chi);

            double _samplOpeningBackLog = _accountGet.OpeningStock;

            double _dauKyNo_NgoaiTe = GetDauKyNoNgoaiTe(_param, listLedgerBefore);

            double _dauKyNo_VND = GetDauKyNoVND(_param, listLedgerBefore);

            double _dauKyNo_SoLuong = GetDauKyNoSoLuong(_param, listLedgerBefore);

            double _dauKyCo_NgoaiTe = GetDauKyCoNgoaiTe(_param, listLedgerBefore);

            double _dauKyCo_VND = GetDauKyCoVND(_param, listLedgerBefore);

            double _dauKyCo_SoLuong = GetDauKyCoSoLuong(_param, listLedgerBefore);

            relations.ForEach(x =>
            {
                var _sampl = _samplOpeningBackLog;

                _sampl += x.DebitAmount - x.CreditAmount + _dauKyNo_VND - _dauKyCo_VND;

                x.ResidualDebit = _sampl > 0 ? _sampl : 0;

                x.ResidualCredit = _sampl < 0 ? Math.Abs(_sampl) : 0;

                x.Thu_Amount = x.IsDebit ? x.Amount : 0;// ArisingDebit_OrginalCur
                x.Chi_Amount = !x.IsDebit ? x.Amount : 0;//ArisingCredit_OrginalCur

                if (!isNoiBo)
                {
                    x.Residual_Amount = (_OpeningBackLog.OpeningAmountLeft) + x.Thu_Amount - x.Chi_Amount;
                    x.Temp = long.Parse(x.Month + "" + x.OrginalBookDate.Value.Year);
                    _OpeningBackLog.OpeningAmountLeft = x.Residual_Amount;
                    x.ArisingDebit_Foreign = x.IsDebit ? x.OrginalCurrency : 0;
                    x.ArisingCredit_Foreign = !x.IsDebit ? x.OrginalCurrency : 0;
                    x.ResidualAmount_Foreign = (_OpeningBackLog.OriginalCurrency + _dauKyNo_NgoaiTe - _dauKyCo_NgoaiTe) + x.ArisingDebit_Foreign - x.ArisingCredit_Foreign;
                    x.ResidualAmount_OrginalCur = (_OpeningBackLog.ExchangeRate + _dauKyNo_VND - _dauKyCo_VND) + x.Thu_Amount - x.Chi_Amount;
                    _OpeningBackLog.OriginalCurrency = x.ResidualAmount_Foreign;
                    _OpeningBackLog.ExchangeRate = x.ResidualAmount_OrginalCur;
                    //4
                    x.Input_Quantity = x.IsDebit ? x.Quantity : 0;
                    x.Output_Quantity = !x.IsDebit ? x.Quantity : 0;
                    x.Residual_Quantity = (_OpeningBackLog.OpeningStockQuantity + _dauKyNo_SoLuong - _dauKyCo_SoLuong) + x.Input_Quantity - x.Output_Quantity;
                    _OpeningBackLog.OpeningStockQuantity = x.Residual_Quantity;
                    return;
                }

                x.Residual_Amount = (_OpeningBackLog.OpeningAmountLeftNB) + x.Thu_Amount - x.Chi_Amount;
                x.Temp = long.Parse(x.Month + "" + x.OrginalBookDate.Value.Year);
                _OpeningBackLog.OpeningAmountLeftNB = x.Residual_Amount;
                x.ArisingDebit_Foreign = x.IsDebit ? x.OrginalCurrency : 0;
                x.ArisingCredit_Foreign = !x.IsDebit ? x.OrginalCurrency : 0;
                x.ResidualAmount_Foreign = (_OpeningBackLog.OriginalCurrencyNB + _dauKyNo_NgoaiTe - _dauKyCo_NgoaiTe) + x.ArisingDebit_Foreign - x.ArisingCredit_Foreign;
                x.ResidualAmount_OrginalCur = (_OpeningBackLog.ExchangeRateNB + _dauKyNo_VND - _dauKyCo_VND) + x.Thu_Amount - x.Chi_Amount;
                _OpeningBackLog.OriginalCurrencyNB = x.ResidualAmount_Foreign;
                _OpeningBackLog.ExchangeRateNB = x.ResidualAmount_OrginalCur;
                //4
                x.Input_Quantity = x.IsDebit ? x.Quantity : 0;
                x.Output_Quantity = !x.IsDebit ? x.Quantity : 0;
                x.Residual_Quantity = (_OpeningBackLog.OpeningStockQuantityNB + _dauKyNo_SoLuong - _dauKyCo_SoLuong) + x.Input_Quantity - x.Output_Quantity;
                _OpeningBackLog.OpeningStockQuantityNB = x.Residual_Quantity;
            });

            LedgerReportCalculatorIO _OpeningBackLog_2 = await CalculatorFollowMonth_ThuChi(_param, year);
        
            List<long> _months1 = relations.Select(x => x.Temp).Distinct().ToList();

            int _ind = 0;
            double _luyKe_PS_Thu = 0;
            double _luyKe_PS_Chi = 0;
            double _luyKe_PS_Ton = GetLuyKePSTon(isNoiBo, _dauKy_Thu, _dauKy_Chi, _OpeningBackLog_2);

            double _luyKe_PS_No_SoLuong = 0;
            double _luyKe_PS_Co_SoLuong = 0;
            double _quantityDK = 0;
            double _donGiaDK = 0;
            double _tienDK = 0;
            double _exchangeRateDK = 0;
            double _OrginalCurrencyDK = 0;

            var chartAcc = await GetChartOfAccount(_param, year, isNoiBo);

            if (chartAcc != null)
            {
                //4
                _quantityDK = (isNoiBo ? chartAcc.OpeningStockQuantityNB : chartAcc.OpeningStockQuantity) ?? 0;
                _donGiaDK = (isNoiBo ? chartAcc.StockUnitPriceNB : chartAcc.StockUnitPrice) ?? 0;
                _tienDK = ((isNoiBo ? chartAcc.OpeningDebitNB : chartAcc.OpeningDebit) ?? 0) - ((isNoiBo ? chartAcc.OpeningCreditNB : chartAcc.OpeningCredit) ?? 0);
                //3
                _exchangeRateDK = (isNoiBo ? chartAcc.OpeningForeignDebitNB : chartAcc.OpeningForeignDebit) ?? 0;
                _OrginalCurrencyDK = (isNoiBo ? chartAcc.OpeningDebitNB : chartAcc.OpeningDebit) ?? 0;
            }

            var v_dicTotal = new Dictionary<long, List<SoChiTietThuChiViewModel>>();

            foreach (var x in _months1)
            {
                if (!v_dicTotal.ContainsKey(x))
                {
                    SoChiTietThuChiViewModel _congPS = new SoChiTietThuChiViewModel();
                    var temps = relations.Where(y => y.Temp == x);
                    _congPS.Thu_Amount = temps.Sum(q => q.Thu_Amount);
                    _congPS.Chi_Amount = temps.Sum(q => q.Chi_Amount);
                    _congPS.Residual_Amount = _luyKe_PS_Ton + _congPS.Thu_Amount - _congPS.Chi_Amount;
                    _congPS.Month = -1;//cộng phát sinh
                    _congPS.ArisingDebit_Foreign = temps.Sum(q => q.ArisingDebit_Foreign);
                    _congPS.ArisingCredit_Foreign = temps.Sum(q => q.ArisingCredit_Foreign);
                    _congPS.ResidualAmount_Foreign = temps.Sum(q => q.ResidualAmount_Foreign);
                    _congPS.ResidualAmount_OrginalCur = temps.Sum(q => q.ResidualAmount_OrginalCur);
                    //4
                    _congPS.Input_Quantity = temps.Sum(q => q.Input_Quantity);
                    _congPS.Output_Quantity = temps.Sum(q => q.Output_Quantity);
                    _congPS.Residual_Quantity = temps.Sum(q => q.Residual_Quantity);

                    SoChiTietThuChiViewModel _LuyKeNam = new SoChiTietThuChiViewModel();
                    _LuyKeNam.Thu_Amount = temps.Sum(q => q.Thu_Amount);
                    _LuyKeNam.Chi_Amount = temps.Sum(q => q.Chi_Amount);
                    _LuyKeNam.Residual_Amount = _congPS.Residual_Amount;
                    _LuyKeNam.Month = -2;//cộng lũy kế
                    _LuyKeNam.ArisingDebit_Foreign = temps.Sum(q => q.ArisingDebit_Foreign);
                    _LuyKeNam.ArisingCredit_Foreign = temps.Sum(q => q.ArisingCredit_Foreign);
                    _LuyKeNam.ResidualAmount_Foreign = temps.Sum(q => q.ResidualAmount_Foreign);
                    _LuyKeNam.ResidualAmount_OrginalCur = temps.Sum(q => q.ResidualAmount_OrginalCur);
                    //4
                    _LuyKeNam.Input_Quantity = temps.Sum(q => q.Input_Quantity);
                    _LuyKeNam.Output_Quantity = temps.Sum(q => q.Output_Quantity);
                    _LuyKeNam.Residual_Quantity = temps.Sum(q => q.Residual_Quantity);

                    int _ms = (int)x / 10000;
                    int _year = (int)x % 10000;
                    if (x > long.Parse("1" + _year))
                    {
                        var _thuDK = isNoiBo 
                            ? _OpeningBackLog_2.OpeningDebitNB 
                            : _OpeningBackLog_2.OpeningDebit;

                        var _chiDK = isNoiBo 
                            ? _OpeningBackLog_2.OpeningCreditNB 
                            : _OpeningBackLog_2.OpeningCredit;

                        _luyKe_PS_Thu = await _context.GetLedgerNotForYear(isNoiBo ? 3 : 2).Where(x => x.IsInternal != LedgerInternalConst.LedgerTemporary && x.OrginalBookDate >= dtFromBefore)
                        .Where(y => y.Month < _ms && y.Year <= _year)
                        .Where(y => y.DebitCode == _param.AccountCode
            && (string.IsNullOrEmpty(_param.AccountCodeDetail1) || y.DebitDetailCodeFirst == _param.AccountCodeDetail1)
            && (string.IsNullOrEmpty(_param.AccountCodeDetail2) || y.DebitDetailCodeSecond == _param.AccountCodeDetail2)
            ).SumAsync(q => q.Amount);

                        _luyKe_PS_Chi = await _context.GetLedgerNotForYear(isNoiBo ? 3 : 2).Where(x => x.IsInternal != LedgerInternalConst.LedgerTemporary && x.OrginalBookDate >= dtFromBefore)
                        .Where(y => y.Month < _ms && y.Year <= _year)
                        .Where(y => y.CreditCode == _param.AccountCode
            && (string.IsNullOrEmpty(_param.AccountCodeDetail1) || y.CreditDetailCodeFirst == _param.AccountCodeDetail1)
            && (string.IsNullOrEmpty(_param.AccountCodeDetail2) || y.CreditDetailCodeSecond == _param.AccountCodeDetail2)
            )
                        .SumAsync(q => q.Amount);

                        _LuyKeNam.Thu_Amount += _luyKe_PS_Thu;
                        _LuyKeNam.Chi_Amount += _luyKe_PS_Chi;

                        _thuDK += _LuyKeNam.Thu_Amount;
                        _chiDK += _LuyKeNam.Chi_Amount;
                        _LuyKeNam.Residual_Amount = _thuDK - _chiDK;

                        var listLuyKeNo = relations.Where(y => y.Month < _ms && y.Year <= _year)
                            .Where(y => y.DebitCode == _param.AccountCode && y.DebitDetailCodeFirst == _param.AccountCodeDetail1).ToList();
                        var listLuyKeCo = relations.Where(y => y.Month < _ms && y.Year <= _year)
                        .Where(y => y.CreditCode == _param.AccountCode && y.CreditDetailCodeFirst == _param.AccountCodeDetail1).ToList();
                        if (_param.BookDetailType == 3)
                        {
                            _luyKe_PS_No_NT = listLuyKeNo.Sum(q => q.OrginalCurrency);
                            _luyKe_PS_No_VND = listLuyKeNo.Sum(q => q.Amount);
                            _luyKe_PS_Co_NT = listLuyKeCo.Sum(q => q.OrginalCurrency);
                            _luyKe_PS_Co_VND = listLuyKeCo.Sum(q => q.Amount);

                            _LuyKeNam.ArisingDebit_Foreign += _luyKe_PS_No_NT;
                            _LuyKeNam.ArisingCredit_Foreign += _luyKe_PS_Co_NT;

                            if (_luyKe_PS_No_NT > 0)
                            {
                                _luyKe_PS_No_NT += _exchangeRateDK;
                                _luyKe_PS_No_VND += _OrginalCurrencyDK;
                            }
                            else
                            {
                                if (_luyKe_PS_Co_NT > 0)
                                {
                                    _luyKe_PS_Co_NT -= _exchangeRateDK;
                                    _luyKe_PS_Co_VND -= _OrginalCurrencyDK;
                                }
                            }
                            _LuyKeNam.ResidualAmount_Foreign = relations.Where(y => y.Temp == x).Sum(q => q.ResidualAmount_Foreign);
                            _LuyKeNam.ResidualAmount_OrginalCur = relations.Where(y => y.Temp == x).Sum(q => q.ResidualAmount_OrginalCur);
                        }
                        else if (_param.BookDetailType == 4)
                        {
                            _luyKe_PS_No_SoLuong = listLuyKeNo.Sum(q => q.Quantity);
                            _luyKe_PS_Co_SoLuong = listLuyKeCo.Sum(q => q.Quantity);
                            _LuyKeNam.Input_Quantity += _luyKe_PS_No_SoLuong;
                            _LuyKeNam.Output_Quantity += _luyKe_PS_Co_SoLuong;

                            if (_luyKe_PS_No_SoLuong > 0)
                            {
                                _luyKe_PS_No_SoLuong += _quantityDK;
                            }
                            else if (_luyKe_PS_Co_SoLuong > 0)
                            {
                                _luyKe_PS_Co_SoLuong -= _quantityDK;
                            }
                            _LuyKeNam.Residual_Quantity = relations.Where(y => y.Temp == x).Sum(q => q.Residual_Quantity);
                            _LuyKeNam.Residual_Amount = relations.Where(y => y.Temp == x).Sum(q => q.Residual_Amount);
                        }
                    }
                    _luyKe_PS_Ton = _congPS.Residual_Amount;
                    SoChiTietThuChiViewModel _du = new SoChiTietThuChiViewModel();
                    _du.Month = -3;//dư
                    _du.ArisingDebit_Foreign = 0;
                    _du.ArisingCredit_Foreign = 0;
                    _du.Input_Quantity = 0;
                    _du.Output_Quantity = 0;

                    if (!isNoiBo)
                    {
                        _du.Thu_Amount = _OpeningBackLog_2.OpeningDebit + _LuyKeNam.Thu_Amount;
                        _du.Chi_Amount = _OpeningBackLog_2.OpeningCredit + _LuyKeNam.Chi_Amount;
                        _du.Residual_Amount = _OpeningBackLog_2.OpeningAmountLeft + _LuyKeNam.Thu_Amount - _LuyKeNam.Chi_Amount;

                        _du.ResidualAmount_Foreign = _OpeningBackLog_2.OriginalCurrency + _LuyKeNam.ArisingDebit_Foreign - _LuyKeNam.ArisingCredit_Foreign;
                        _du.ResidualAmount_OrginalCur = _OpeningBackLog_2.ExchangeRate + _LuyKeNam.Thu_Amount - _LuyKeNam.Chi_Amount;
                        _du.Residual_Quantity = _OpeningBackLog_2.OpeningStockQuantity + _LuyKeNam.Input_Quantity - _LuyKeNam.Output_Quantity;
                    }
                    else
                    {
                        _du.Thu_Amount = _OpeningBackLog_2.OpeningDebitNB + _LuyKeNam.Thu_Amount;
                        _du.Chi_Amount = _OpeningBackLog_2.OpeningCreditNB + _LuyKeNam.Chi_Amount;
                        _du.Residual_Amount = _OpeningBackLog_2.OpeningAmountLeftNB + _LuyKeNam.Thu_Amount - _LuyKeNam.Chi_Amount;

                        _du.ResidualAmount_Foreign = _OpeningBackLog_2.OriginalCurrencyNB + _LuyKeNam.ArisingDebit_Foreign - _LuyKeNam.ArisingCredit_Foreign;
                        _du.ResidualAmount_OrginalCur = _OpeningBackLog_2.ExchangeRateNB + _LuyKeNam.Thu_Amount - _LuyKeNam.Chi_Amount;
                        _du.Residual_Quantity = _OpeningBackLog_2.OpeningStockQuantityNB + _LuyKeNam.Input_Quantity - _LuyKeNam.Output_Quantity;
                    }

                    v_dicTotal.Add(x, new List<SoChiTietThuChiViewModel>
                    {
                        _congPS,
                        _LuyKeNam,
                        _du
                    });

                    _ind++;
                }
            }

            LedgerReportModel _model = new LedgerReportModel
            {
                InfoSum = null,
                Items = null,
                BookDetails = relations,
                LedgerCalculator = null,
                Address = _company.Address,
                Company = _company.Name,
                MethodCalcExportPrice = _company.MethodCalcExportPrice,
                TaxId = _company.MST,
                CEOName = _company.NameOfCEO,
                ChiefAccountantName = _company.NameOfChiefAccountant,
                AccountCode = _accountFind?.Code,
                AccountName = _accountFind?.Name + (accountChild != null ? (" - " + accountChild.Name) : string.Empty),
                SumItem_SCT_ThuChi = v_dicTotal,
                CEONote = _company.NoteOfCEO,
                ChiefAccountantNote = _company.NoteOfChiefAccountant,
            };

            return await _exportDataReport_SoChiTiet.ReportAsync(_model, _param, year);
        }
        catch
        {
            return string.Empty;
        }
    }

    private async Task<LedgerReportCalculatorIO> CalculatorFollowMonth_ThuChi(LedgerReportParam _param, int year)
    {
        LedgerReportCalculatorIO p = new LedgerReportCalculatorIO();
        try
        {
            DateTime _fromDt, _toDt;
            _fromDt = _param.FromMonth > 0 ? new DateTime(DateTime.Now.Year, (int)_param.FromMonth, 1) : (DateTime)_param.FromDate;
            _toDt = _param.ToMonth > 0 ? new DateTime(DateTime.Now.Year, (int)_param.ToMonth, 1) : (DateTime)_param.ToDate;

            var _chart = _context.GetChartOfAccount(year).Where(x =>
                string.IsNullOrEmpty(_param.AccountCodeDetail1)
                ? x.Code == _param.AccountCode
                : x.Code == _param.AccountCodeDetail1 && x.ParentRef == _param.AccountCode
                ).FirstOrDefault();

            if (_param.FromDate.Value.Year != year)
            {
                var accountOld = await _context.ChartOfAccounts.FirstOrDefaultAsync(x => x.Code == _chart.Code && x.ParentRef == _chart.ParentRef
                                && (string.IsNullOrEmpty(_chart.WarehouseCode) || x.WarehouseCode == _chart.WarehouseCode) && x.Year == _param.FromDate.Value.Year);
                _chart.OpeningDebit = accountOld?.OpeningDebit;
                _chart.OpeningCredit = accountOld?.OpeningCredit;
                _chart.OpeningStockQuantity = accountOld?.OpeningStockQuantity;

                _chart.OpeningDebitNB = accountOld?.OpeningDebitNB;
                _chart.OpeningCreditNB = accountOld?.OpeningCreditNB;
                _chart.OpeningStockQuantityNB = accountOld?.OpeningStockQuantityNB;
            }

            if (_chart != null)
            {
                p.OpeningDebit = _chart.OpeningDebit ?? 0;
                p.OpeningCredit = _chart.OpeningCredit ?? 0;
                p.OpeningAmountLeft = p.OpeningDebit - p.OpeningCredit;

                p.OriginalCurrency = _chart.OpeningForeignDebit ?? 0;
                p.ExchangeRate = (_chart?.OpeningDebit ?? 0) - (_chart?.OpeningCredit ?? 0);
                //4
                p.StockUnitPrice = (_chart.StockUnitPrice ?? 0);
                p.OpeningStockQuantity = (_chart.OpeningStockQuantity ?? 0);

                p.OpeningDebitNB = _chart.OpeningDebitNB ?? 0;
                p.OpeningCreditNB = _chart.OpeningCreditNB ?? 0;
                p.OpeningAmountLeftNB = p.OpeningDebitNB - p.OpeningCreditNB;
                p.OriginalCurrencyNB = _chart.OpeningForeignDebitNB ?? 0;
                p.ExchangeRateNB = (_chart?.OpeningDebitNB ?? 0) - (_chart?.OpeningCreditNB ?? 0);
                p.StockUnitPriceNB = _chart.StockUnitPriceNB ?? 0;
                p.OpeningStockQuantityNB = _chart.OpeningStockQuantityNB ?? 0;
            }

            return p;
        }
        catch
        {
            return null;
        }
    }

    private static double GetLuyKePSTon(bool isNoiBo, double _dauKy_Thu, double _dauKy_Chi, LedgerReportCalculatorIO _OpeningBackLog_2)
    {
        if (!isNoiBo)
            return _OpeningBackLog_2.OpeningAmountLeft + _dauKy_Thu - _dauKy_Chi;
        return _OpeningBackLog_2.OpeningAmountLeftNB + _dauKy_Thu - _dauKy_Chi;
    }

    private async Task<ChartOfAccount> GetChartOfAccount(LedgerReportParamDetail _param, int year, bool isNoiBo)
    {
        var chartAcc = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(y => y.Code == _param.AccountCodeDetail1 && y.ParentRef == _param.AccountCode);

        if (chartAcc == null) 
        {
            return null;
        }

        if(_param.FromDate.Value.Year == year)
        {
            return chartAcc;
        }

        var accountOld = await _context.ChartOfAccounts.FirstOrDefaultAsync(x => x.Code == chartAcc.Code
           && x.ParentRef == chartAcc.ParentRef 
           && (string.IsNullOrEmpty(chartAcc.WarehouseCode) 
            || x.WarehouseCode == chartAcc.WarehouseCode) 
           && x.Year == _param.FromDate.Value.Year);

        if (accountOld == null) 
        {
            return chartAcc;
        }

        chartAcc.OpeningDebit = accountOld.OpeningDebit;
        chartAcc.OpeningDebitNB = accountOld.OpeningDebitNB;
        chartAcc.OpeningCredit = accountOld.OpeningCredit;
        chartAcc.OpeningCreditNB = accountOld.OpeningCreditNB;
        chartAcc.StockUnitPrice = accountOld.StockUnitPrice;
        chartAcc.StockUnitPriceNB = accountOld.StockUnitPriceNB;
        chartAcc.OpeningForeignDebit = accountOld.OpeningForeignDebit;
        chartAcc.OpeningForeignDebitNB = accountOld.OpeningForeignDebitNB;
        chartAcc.OpeningStockQuantity = accountOld.OpeningStockQuantity;
        chartAcc.OpeningStockQuantityNB = accountOld.OpeningStockQuantityNB;

        return chartAcc;
    }

    private static double GetDauKyCoSoLuong(LedgerReportParamDetail _param, List<SumSoChiTietViewModel> listLedgerBefore)
    {
        Expression<Func<SumSoChiTietViewModel, bool>> expression = x => x.CreditCode == _param.AccountCode;

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail1))
        {
            expression = expression.And(x => x.CreditDetailCodeFirst == _param.AccountCodeDetail1);
        }

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail2))
        {
            expression = expression.And(x => x.CreditDetailCodeSecond == _param.AccountCodeDetail2);
        }

        return listLedgerBefore.Where(expression.Compile()).Sum(q => q.Quantity);
    }

    private static double GetDauKyCoVND(LedgerReportParamDetail _param, List<SumSoChiTietViewModel> listLedgerBefore)
    {
        Expression<Func<SumSoChiTietViewModel, bool>> expression = x => x.CreditCode == _param.AccountCode;

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail1))
        {
            expression = expression.And(x => x.CreditDetailCodeFirst == _param.AccountCodeDetail1);
        }

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail2))
        {
            expression = expression.And(x => x.CreditDetailCodeSecond == _param.AccountCodeDetail2);
        }

        return listLedgerBefore.Where(expression.Compile()).Sum(q => q.Amount);
    }

    private static double GetDauKyCoNgoaiTe(LedgerReportParamDetail _param, List<SumSoChiTietViewModel> listLedgerBefore)
    {
        Expression<Func<SumSoChiTietViewModel, bool>> expression = x => x.CreditCode == _param.AccountCode;

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail1))
        {
            expression = expression.And(x => x.CreditDetailCodeFirst == _param.AccountCodeDetail1);
        }

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail2))
        {
            expression = expression.And(x => x.CreditDetailCodeSecond == _param.AccountCodeDetail2);
        }

        return listLedgerBefore.Where(expression.Compile()).Sum(q => q.OrginalCurrency);
    }

    private static double GetDauKyNoSoLuong(LedgerReportParamDetail _param, List<SumSoChiTietViewModel> listLedgerBefore)
    {
        Expression<Func<SumSoChiTietViewModel, bool>> expression = x => x.DebitCode == _param.AccountCode;

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail1))
        {
            expression = expression.And(x => x.DebitDetailCodeFirst == _param.AccountCodeDetail1);
        }

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail2))
        {
            expression = expression.And(x => x.DebitDetailCodeSecond == _param.AccountCodeDetail2);
        }

        return listLedgerBefore.Where(expression.Compile()).Sum(q => q.Quantity);
    }

    private static double GetDauKyNoVND(LedgerReportParamDetail _param, List<SumSoChiTietViewModel> listLedgerBefore)
    {
        Expression<Func<SumSoChiTietViewModel, bool>> expression = x => x.DebitCode == _param.AccountCode;

        if(!string.IsNullOrEmpty(_param.AccountCodeDetail1))
        {
            expression = expression.And(x => x.DebitDetailCodeFirst == _param.AccountCodeDetail1);
        }

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail2))
        {
            expression = expression.And(x => x.DebitDetailCodeSecond == _param.AccountCodeDetail2);
        }

        return listLedgerBefore.Where(expression.Compile()).Sum(q => q.Amount);
    }

    private static double GetDauKyNoNgoaiTe(LedgerReportParamDetail _param, List<SumSoChiTietViewModel> listLedgerBefore)
    {
        Expression<Func<SumSoChiTietViewModel, bool>> expression = x => x.DebitCode == _param.AccountCode;

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail1))
        {
            expression = expression.And(x => x.DebitDetailCodeFirst == _param.AccountCodeDetail1);
        }

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail2))
        {
            expression = expression.And(x => x.DebitDetailCodeSecond == _param.AccountCodeDetail2);
        }

        return listLedgerBefore.Where(expression.Compile()).Sum(q => q.OrginalCurrency);
    }

    private static double GetSamplOpeningBackLog(bool isNoiBo, LedgerReportCalculatorIO _OpeningBackLog, double _dauKy_Thu, double _dauKy_Chi)
    {
        if (!isNoiBo)
        {
            _OpeningBackLog.OpeningAmountLeft = _OpeningBackLog.OpeningAmountLeft + _dauKy_Thu - _dauKy_Chi;
            return _OpeningBackLog.OpeningDebit - _OpeningBackLog.OpeningCredit;
        }

        _OpeningBackLog.OpeningAmountLeftNB = _OpeningBackLog.OpeningAmountLeftNB + _dauKy_Thu - _dauKy_Chi;
        return _OpeningBackLog.OpeningDebitNB - _OpeningBackLog.OpeningCreditNB;
    }

    private static double DauKyChi(LedgerReportParamDetail _param, List<SumSoChiTietViewModel> listLedgerBefore)
    {
        Expression<Func<SumSoChiTietViewModel, bool>> expression = x => x.CreditCode == _param.AccountCode;

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail1))
        {
            expression = expression.And(x => x.CreditDetailCodeFirst == _param.AccountCodeDetail1);
        }

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail2))
        {
            expression = expression.And(x => x.CreditDetailCodeSecond == _param.AccountCodeDetail2);
        }

        return listLedgerBefore.Where(expression.Compile())
            .Sum(q => q.Amount);
    }

    private static double GetDauKyThu(LedgerReportParamDetail _param, List<SumSoChiTietViewModel> listLedgerBefore)
    {
        Expression<Func<SumSoChiTietViewModel, bool>> expression = x => x.DebitCode == _param.AccountCode;

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail1))
        {
            expression = expression.And(x => x.DebitDetailCodeFirst == _param.AccountCodeDetail1);
        }

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail2))
        {
            expression = expression.And(x => x.DebitDetailCodeSecond == _param.AccountCodeDetail2);
        }

        return listLedgerBefore.Where(expression.Compile())
            .Sum(q => q.Amount);
    }

    private async Task<List<SumSoChiTietViewModel>> GetLedgersBefore(LedgerReportParamDetail _param, bool isNoiBo, DateTime dtFrom, DateTime dtFromBefore)
    {
        Expression<Func<Ledger, bool>> expression = x => x.OrginalBookDate >= dtFromBefore
                                                     && x.OrginalBookDate.Value < dtFrom
                                                     && (x.DebitCode == _param.AccountCode
                                                        || x.CreditCode == _param.AccountCode);

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail1))
        {
            expression = expression.And(x => x.DebitDetailCodeFirst == _param.AccountCodeDetail1 || x.CreditDetailCodeFirst == _param.AccountCodeDetail1);
        }

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail2))
        {
            expression = expression.And(x => x.DebitDetailCodeSecond == _param.AccountCodeDetail2 || x.CreditDetailCodeSecond == _param.AccountCodeDetail2);
        }

        return await _context.GetLedgerNotForYear(isNoiBo 
                                                    ? (int)LedgerInternal.Internal 
                                                    : (int)LedgerInternal.Accounting
            ).Where(expression)
            .Select(k => new SumSoChiTietViewModel
            {
                DebitCode = k.DebitCode,
                CreditCode = k.CreditCode,
                CreditDetailCodeFirst = k.CreditDetailCodeFirst,
                DebitDetailCodeSecond = k.DebitDetailCodeSecond,
                CreditDetailCodeSecond = k.CreditDetailCodeSecond,
                DebitDetailCodeFirst = k.DebitDetailCodeFirst,
                Amount = k.Amount,
                OrginalCurrency = k.OrginalCurrency,
                Quantity = k.Quantity,
            })
            .ToListAsync();
    }

    private async Task<List<SoChiTietViewModel>> GetRelations(LedgerReportParamDetail _param, bool isNoiBo, DateTime dtFrom, DateTime dtTo)
    {
        Expression<Func<Ledger, bool>> expression = x => x.OrginalBookDate >= dtFrom
                                                      && x.OrginalBookDate < dtTo;

        if (!string.IsNullOrEmpty(_param.AccountCode))
        {
            expression = expression.And(x => x.DebitCode == _param.AccountCode
                                          || x.CreditCode == _param.AccountCode);
        }

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail1))
        {
            expression = expression.And(x => x.DebitDetailCodeFirst == _param.AccountCodeDetail1
                                          || x.CreditDetailCodeFirst == _param.AccountCodeDetail1);
        }

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail2))
        {
            expression = expression.And(x => x.DebitDetailCodeSecond == _param.AccountCodeDetail2
                                          || x.CreditDetailCodeSecond == _param.AccountCodeDetail2);
        }

        if (!string.IsNullOrEmpty(_param.AccountCodeReciprocal))
        {
            expression = expression.And(x => x.DebitCode == _param.AccountCodeReciprocal
                                          || x.CreditCode == _param.AccountCodeReciprocal);
        }

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail1Reciprocal))
        {
            expression = expression.And(x => x.DebitDetailCodeFirst == _param.AccountCodeDetail1Reciprocal
                                          || x.CreditDetailCodeFirst == _param.AccountCodeDetail1Reciprocal);
        }

        if (!string.IsNullOrEmpty(_param.AccountCodeDetail2Reciprocal))
        {
            expression = expression.And(x => x.DebitDetailCodeSecond == _param.AccountCodeDetail2Reciprocal
                                          || x.CreditDetailCodeSecond == _param.AccountCodeDetail2Reciprocal);
        }

        var ledgers = await _context.GetLedgerNotForYear(isNoiBo ? 3 : 2)
            .Where(expression)
            .OrderBy(x => x.OrginalBookDate).ToListAsync();

        return ledgers
                .Select(k => new SoChiTietViewModel
                {
                    BookDate = k.BookDate,
                    OrginalBookDate = k.OrginalBookDate,
                    DebitCode = k.DebitCode,
                    CreditCode = k.CreditCode,
                    Description = k.OrginalDescription,
                    TakeNote = string.Empty,
                    VoucherNumber = k.VoucherNumber,
                    OrginalVoucherNumber = k.OrginalVoucherNumber,
                    IsDebit = k.DebitCode.Equals(_param.AccountCode),
                    DebitAmount = k.DebitCode.Equals(_param.AccountCode) ? k.Amount : 0,
                    CreditAmount = k.CreditCode.Equals(_param.AccountCode) ? k.Amount : 0,
                    DetailCode = k.DebitCode.Equals(_param.AccountCode) ? k.CreditDetailCodeFirst : k.DebitDetailCodeFirst,
                    ArisingDebit = k.DebitCode.Equals(_param.AccountCode) ? k.Amount : 0,
                    ArisingCredit = k.CreditCode.Equals(_param.AccountCode) ? k.Amount : 0,
                    Month = k.Month,
                    Year = k.Year ?? 0,
                    ExchangeRate = k.ExchangeRate,
                    OrginalCurrency = k.OrginalCurrency,
                    NameOfPerson = k.OrginalCompanyName,
                    UnitPrice = k.UnitPrice,
                    Amount = k.Amount,
                    Quantity = k.Quantity,
                    DebitDetailCodeFirst = k.DebitDetailCodeFirst,
                    DebitDetailCodeSecond = k.DebitDetailCodeSecond,
                    CreditDetailCodeFirst = k.CreditDetailCodeFirst,
                    CreditDetailCodeSecond = k.CreditDetailCodeSecond,
                    InvoiceNumber = k.InvoiceNumber,
                }).ToList();
    }

    private async Task<ChartOfAccount> GetAccountChild(LedgerReportParamDetail _param, int year)
    {
        if (!string.IsNullOrEmpty(_param.AccountCodeDetail1))
        {
            return await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == _param.AccountCodeDetail1
                                    && x.ParentRef == _param.AccountCode);
        }
        if (!string.IsNullOrEmpty(_param.AccountCodeDetail2))
        {
            var parentRefCode = _param.AccountCode + ":" + _param.AccountCodeDetail1;
            return await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == _param.AccountCodeDetail2
                                    && x.ParentRef == parentRefCode);
        }

        return null;
    }

}