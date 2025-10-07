using Common.Constants;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.Reports;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.Reports
{
    public class SoChiTiet_FourReporter: ISoChiTiet_FourReporter
    {
        private readonly ApplicationDbContext _context;
        private readonly ICompanyService _companyService;
        private readonly IExportDataReport_SoChiTiet _exportDataReport_SoChiTiet;

        public SoChiTiet_FourReporter(ApplicationDbContext context,
            ICompanyService companyService, IExportDataReport_SoChiTiet exportDataReport_SoChiTiet)
        {
            _context = context;
            _companyService = companyService;
            _exportDataReport_SoChiTiet = exportDataReport_SoChiTiet;
        }
        public async Task<string> ReportAsync(LedgerReportParamDetail _param, int year)
        {
            bool isNoiBo = _param.IsNoiBo;
            try
            {
                DateTime dtFrom, dtTo;

                if (string.IsNullOrEmpty(_param.AccountCodeDetail1)) _param.AccountCodeDetail1 = string.Empty;
                _param.AccountCodeDetail1 = _param.AccountCodeDetail1.Trim();


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
                
                var query = await _context.GetLedgerNotForYear(isNoiBo ? 3 : 2).ToListAsync();
                //List<SoChiTietViewModel> relations =
                var tquery = query.Where(x => x.IsInternal != LedgerInternalConst.LedgerTemporary && x.OrginalBookDate >= dtFrom && x.OrginalBookDate < dtTo);
                tquery = tquery.Where(x =>
                    (string.IsNullOrEmpty(_param.AccountCode) || (x.DebitCode == _param.AccountCode || x.CreditCode == _param.AccountCode)));
                tquery = tquery.Where(x => (string.IsNullOrEmpty(_param.AccountCodeDetail1)
                    || ((x.DebitDetailCodeFirst == _param.AccountCodeDetail1 && x.DebitCode == _param.AccountCode)
                    || (x.CreditDetailCodeFirst == _param.AccountCodeDetail1 && x.CreditCode == _param.AccountCode))));

                tquery = tquery.Where(x => string.IsNullOrEmpty(_param.AccountCodeDetail2)
                   || ((x.DebitDetailCodeSecond == _param.AccountCodeDetail2
                       && x.DebitDetailCodeFirst == _param.AccountCodeDetail1 && x.DebitCode == _param.AccountCode)
                       || x.CreditDetailCodeSecond == _param.AccountCodeDetail2 && x.CreditDetailCodeFirst == _param.AccountCodeDetail1
                           && x.CreditCode == _param.AccountCode));

                tquery = tquery.Where(x => string.IsNullOrEmpty(_param.AccountCodeReciprocal)
                    || (x.DebitCode == _param.AccountCodeReciprocal || x.CreditCode == _param.AccountCodeReciprocal));

                tquery = tquery.Where(x => string.IsNullOrEmpty(_param.AccountCodeDetail1Reciprocal)
                    || (x.DebitDetailCodeFirst == _param.AccountCodeDetail1Reciprocal || x.CreditDetailCodeFirst == _param.AccountCodeDetail1Reciprocal));

                tquery = tquery.Where(x => string.IsNullOrEmpty(_param.AccountCodeDetail2Reciprocal)
                    || (x.DebitDetailCodeSecond == _param.AccountCodeDetail2Reciprocal
                        || x.CreditDetailCodeSecond == _param.AccountCodeDetail2Reciprocal));
                    
                List<SoChiTietViewModel> relations = tquery.Select(k => new SoChiTietViewModel
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
                    Year = k.OrginalBookDate.Value.Year,
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
                    InvoiceNumber = k.InvoiceNumber
                })
                    .OrderBy(x => x.OrginalBookDate)
                .ToList();

                List<SumSoChiTietViewModel> listLedgerBefore = await _context.GetLedgerNotForYear(isNoiBo ? 3 : 2).Where(y => y.IsInternal != LedgerInternalConst.LedgerTemporary && y.OrginalBookDate.Value >= dtFromBefore
                               && y.OrginalBookDate.Value < dtFrom && (y.DebitCode == _param.AccountCode || y.CreditCode == _param.AccountCode)
                    && (string.IsNullOrEmpty(_param.AccountCodeDetail1) || (y.DebitDetailCodeFirst == _param.AccountCodeDetail1 || y.CreditDetailCodeFirst == _param.AccountCodeDetail1))
                    && (string.IsNullOrEmpty(_param.AccountCodeDetail2) || (y.DebitDetailCodeSecond == _param.AccountCodeDetail2 || y.CreditDetailCodeSecond == _param.AccountCodeDetail2))
                    )
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
                        CreditWarehouse = k.CreditWarehouse,
                        DebitWarehouse = k.DebitWarehouse,
                    })
                    .ToListAsync();

                IDictionary<string, List<SoChiTietThuChiViewModel>> v_dicTotal = new Dictionary<string, List<SoChiTietThuChiViewModel>>();

                var listAccount = await _context.GetChartOfAccount(year).Where(x => x.ParentRef.Contains(_param.AccountCode) && !x.HasChild && !x.HasDetails).ToListAsync();

                
                if (!string.IsNullOrEmpty(_param.AccountCodeDetail2))
                {
                    var parentRef = $"{_param.AccountCode}:{_param.AccountCodeDetail1}";
                    listAccount = listAccount.Where(x => x.Code == _param.AccountCodeDetail2 && x.ParentRef == parentRef).ToList();
                }
                else if (!string.IsNullOrEmpty(_param.AccountCodeDetail1))
                {
                    var parentRef = $"{_param.AccountCode}";
                    listAccount = listAccount.Where(x => x.ParentRef == parentRef).ToList();
                }

                var listLedgerAll = await _context.GetLedgerNotForYear(_param.IsNoiBo ? 3 : 2).Where(x => x.OrginalBookDate >= dtFromBefore && x.OrginalBookDate < dtTo)
                                .Where(y => y.DebitCode == _param.AccountCode || y.CreditCode == _param.AccountCode).ToListAsync();
                foreach (var account in listAccount)
                {
                    string detail1 = "";
                    string detail2 = "";
                    string wareHouse = account.WarehouseCode;
                    var relations_new = new List<SoChiTietViewModel>();
                    if (account.Type == 5)
                    {
                        relations_new = relations.Where(x => (x.DebitDetailCodeFirst == account.Code || x.CreditDetailCodeFirst == account.Code)
                        && (string.IsNullOrEmpty(wareHouse) || x.DebitWarehouseCode == wareHouse || x.CreditWarehouseCode == wareHouse)).ToList();
                        if (relations_new.Count == 0)
                            continue;
                        detail1 = account.Code;
                    }
                    else if (account.Type == 6)
                    {
                        relations_new = relations.Where(x => x.DebitDetailCodeSecond == account.Code || x.CreditDetailCodeSecond == account.Code
                        && (string.IsNullOrEmpty(wareHouse) || x.DebitWarehouseCode == wareHouse || x.CreditWarehouseCode == wareHouse)).ToList();
                        if (relations_new.Count == 0)
                            continue;
                        detail1 = account.ParentRef.Split(':')[1];
                        detail2 = account.Code;
                    }
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

                    double _dauKy_Thu = listLedgerBefore.Where(y => y.DebitCode == _param.AccountCode
                    && (string.IsNullOrEmpty(_param.AccountCodeDetail1) || y.DebitDetailCodeFirst == _param.AccountCodeDetail1)
                    && (string.IsNullOrEmpty(_param.AccountCodeDetail2) || y.DebitDetailCodeSecond == _param.AccountCodeDetail2)
                    && (string.IsNullOrEmpty(wareHouse) || y.CreditWarehouse == wareHouse)
                    )
                    .Sum(q => q.Amount);
                    double _dauKy_Chi = listLedgerBefore.Where(y => y.CreditCode == _param.AccountCode
                        && (string.IsNullOrEmpty(_param.AccountCodeDetail1) || y.CreditDetailCodeFirst == _param.AccountCodeDetail1)
                        && (string.IsNullOrEmpty(_param.AccountCodeDetail2) || y.CreditDetailCodeSecond == _param.AccountCodeDetail2)
                        && (string.IsNullOrEmpty(wareHouse) || y.DebitWarehouse == wareHouse)
                        )
                        .Sum(q => q.Amount);

                    double _dauKyNo_SoLuong = listLedgerBefore.Where(y => y.DebitCode == _param.AccountCode
                    && (string.IsNullOrEmpty(detail1) || y.DebitDetailCodeFirst == detail1)
                        && (string.IsNullOrEmpty(detail2) || y.DebitDetailCodeSecond == detail2)).Sum(q => q.Quantity);

                    double _dauKyCo_SoLuong = listLedgerBefore.Where(y => y.CreditCode == _param.AccountCode
                    && (string.IsNullOrEmpty(detail1) || y.CreditDetailCodeFirst == detail1)
                        && (string.IsNullOrEmpty(detail2) || y.CreditDetailCodeSecond == detail2)).Sum(q => q.Quantity);

                    relations_new.ForEach(x =>
                    {
                        x.Thu_Amount = x.IsDebit ? x.Amount : 0;// ArisingDebit_OrginalCur
                        x.Chi_Amount = !x.IsDebit ? x.Amount : 0;//ArisingCredit_OrginalCur
                        x.Input_Quantity = x.IsDebit ? x.Quantity : 0;
                        x.Output_Quantity = !x.IsDebit ? x.Quantity : 0;
                        x.Temp = long.Parse(x.Month + "" + x.Year);

                        if (!isNoiBo)
                        {
                            x.Residual_Amount = (account.OpeningDebit ?? 0) - (account.OpeningCredit ?? 0) + _dauKy_Thu - _dauKy_Chi + x.Thu_Amount - x.Chi_Amount;

                            x.Residual_Quantity = ((account.OpeningStockQuantity ?? 0) + _dauKyNo_SoLuong - _dauKyCo_SoLuong) + x.Input_Quantity - x.Output_Quantity;
                        }
                        else
                        {
                            x.Residual_Amount = (account.OpeningDebitNB ?? 0) - (account.OpeningCreditNB ?? 0) + _dauKy_Thu - _dauKy_Chi + x.Thu_Amount - x.Chi_Amount;
                            x.Residual_Quantity = (account.OpeningStockQuantityNB ?? 0) + _dauKyNo_SoLuong - _dauKyCo_SoLuong + x.Input_Quantity - x.Output_Quantity;
                        }
                        _dauKy_Thu += x.Thu_Amount;
                        _dauKy_Chi += x.Chi_Amount;
                        _dauKyNo_SoLuong += x.Input_Quantity;
                        _dauKyCo_SoLuong += x.Output_Quantity;
                    });

                    double _luyKe_PS_Thu = 0, _luyKe_PS_Chi = 0, _luyKe_PS_Ton = 0;
                    double _luyKe_PS_No_SoLuong = 0, _luyKe_PS_Co_SoLuong = 0;
                    double _quantityDK = 0;
                    double _donGiaDK = 0;
                    double _tienDK = 0;
                    double _quantityLK = 0;
                    double _tienLK = 0;

                    List<long> _months1 = relations_new.Select(x => x.Temp).Distinct().ToList();
                    if (!isNoiBo)
                    {
                        _luyKe_PS_Ton = (account.OpeningDebit ?? 0) - (account.OpeningCredit ?? 0) + _dauKy_Thu - _dauKy_Chi;
                        _quantityDK = account.OpeningStockQuantity ?? 0;
                        _donGiaDK = account.StockUnitPrice ?? 0;
                        _tienDK = (account.OpeningDebit ?? 0) - (account.OpeningCredit ?? 0);
                        _quantityLK = account.OpeningStockQuantity ?? 0;
                        _tienLK = (account.OpeningDebit ?? 0) - (account.OpeningCredit ?? 0);

                    }    
                       
                    else
                    {
                        _luyKe_PS_Ton = (account.OpeningDebitNB ?? 0) - (account.OpeningCreditNB ?? 0) + _dauKy_Thu - _dauKy_Chi;
                        _quantityDK = account.OpeningStockQuantityNB ?? 0;
                        _donGiaDK = account.StockUnitPriceNB ?? 0;
                        _tienDK = (account.OpeningDebitNB ?? 0) - (account.OpeningCreditNB ?? 0);
                        _quantityLK = account.OpeningStockQuantityNB ?? 0;
                        _tienLK = (account.OpeningDebitNB ?? 0) - (account.OpeningCreditNB ?? 0);
                    }    
                                      

                    if (dtFrom.Day > 1 || dtFrom.Month > 1)
                    {
                        var dauKyCo = listLedgerAll.Where(y => y.CreditCode == _param.AccountCode && y.CreditDetailCodeFirst == detail1
                                                    && (string.IsNullOrEmpty(detail2) || y.CreditDetailCodeSecond == detail2)
                                                    && (string.IsNullOrEmpty(account.WarehouseCode) || y.CreditWarehouse == account.WarehouseCode)).ToList();
                        double _dauKyCo_SoLuongLK = dauKyCo.Sum(q => q.Quantity);
                        double _dauKyCo_Amount = dauKyCo.Sum(q => q.Amount);
                        var dauKyNo = listLedgerAll.Where(y => y.DebitCode == _param.AccountCode && y.DebitDetailCodeFirst == detail1
                                                    && (string.IsNullOrEmpty(detail2) || y.DebitDetailCodeSecond == detail2)
                                                    && (string.IsNullOrEmpty(account.WarehouseCode) || y.DebitWarehouse == account.WarehouseCode)).ToList();
                        double _dauKyNo_SoLuongLK = dauKyNo.Sum(q => q.Quantity);
                        double _dauKyNo_Amount = dauKyNo.Sum(q => q.Amount);
                        if (!isNoiBo)
                        {
                            _quantityLK = (account.OpeningStockQuantity ?? 0) + _dauKyNo_SoLuongLK - _dauKyCo_SoLuongLK;
                            _tienLK = (account.OpeningDebit ?? 0) - (account.OpeningCredit ?? 0) + _dauKyNo_Amount - _dauKyCo_Amount;
                        }
                        else
                        {
                            _quantityLK = (account.OpeningStockQuantityNB ?? 0) + _dauKyNo_SoLuongLK - _dauKyCo_SoLuongLK;
                            _tienLK = (account.OpeningDebitNB ?? 0) - (account.OpeningCreditNB ?? 0) + _dauKyNo_Amount - _dauKyCo_Amount;
                        }    
                           
                    }

                    var listThuChi = new List<SoChiTietThuChiViewModel>();
                    _months1.ForEach(x =>
                    {
                        int _ms = (int)x / 10000;
                        int _year = (int)x % 10000;

                        SoChiTietThuChiViewModel _congPS = new SoChiTietThuChiViewModel();
                        _congPS.Thu_Amount = relations_new.Where(y => y.Month == _ms && y.Year == _year).Sum(q => q.Thu_Amount);
                        _congPS.Chi_Amount = relations_new.Where(y => y.Month == _ms && y.Year == _year).Sum(q => q.Chi_Amount);
                        _congPS.Residual_Amount = _luyKe_PS_Ton + _congPS.Thu_Amount - _congPS.Chi_Amount;
                        _congPS.Type = 1;
                        _congPS.Month = _ms;//cộng phát sinh

                        _congPS.Input_Quantity = relations_new.Where(y => y.Month == _ms && y.Year == _year).Sum(q => q.Input_Quantity);
                        _congPS.Output_Quantity = relations_new.Where(y => y.Month == _ms && y.Year == _year).Sum(q => q.Output_Quantity);
                        _congPS.Residual_Quantity = relations_new.Where(y => y.Month == _ms && y.Year == _year).Sum(q => q.Residual_Quantity);

                        SoChiTietThuChiViewModel _LuyKeNam = new SoChiTietThuChiViewModel();
                        _LuyKeNam.Thu_Amount = relations_new.Where(y => y.Month == _ms && y.Year == _year).Sum(q => q.Thu_Amount);
                        _LuyKeNam.Chi_Amount = relations_new.Where(y => y.Month == _ms && y.Year == _year).Sum(q => q.Chi_Amount);
                        _LuyKeNam.Residual_Amount = _congPS.Residual_Amount;
                        _LuyKeNam.Type = 2;//cộng lũy kế
                        _LuyKeNam.Month = _ms;//cộng lũy kế

                        _LuyKeNam.Input_Quantity = relations_new.Where(y => y.Month == _ms && y.Year == _year).Sum(q => q.Input_Quantity);
                        _LuyKeNam.Output_Quantity = relations_new.Where(y => y.Month == _ms && y.Year == _year).Sum(q => q.Output_Quantity);
                        _LuyKeNam.Residual_Quantity = relations_new.Where(y => y.Month == _ms && y.Year == _year).Sum(q => q.Residual_Quantity);

                        if (_ms > 1)
                        {
                            double _thuDK = 0, _chiDK = 0;
                            if (!isNoiBo)
                            {
                                _thuDK = account.OpeningDebit ?? 0;
                                _chiDK = account.OpeningCredit ?? 0;
                            }
                            else
                            {
                                _thuDK = account.OpeningDebitNB ?? 0;
                                _chiDK = account.OpeningCreditNB ?? 0;
                            }
                            _luyKe_PS_Thu = listLedgerAll.Where(y => y.Month < _ms && y.Year == dtFrom.Year)
                            .Where(y => y.DebitCode == _param.AccountCode
                            && (string.IsNullOrEmpty(detail1) || y.DebitDetailCodeFirst == detail1)
                            && (string.IsNullOrEmpty(detail2) || y.DebitDetailCodeSecond == detail2)
                            && (string.IsNullOrEmpty(account.WarehouseCode) || y.DebitWarehouse == account.WarehouseCode)
                            ).Sum(q => q.Amount);

                            _luyKe_PS_Chi = listLedgerAll.Where(y => y.Month < _ms && y.Year == dtFrom.Year)
                            .Where(y => y.CreditCode == _param.AccountCode
                            && (string.IsNullOrEmpty(detail1) || y.CreditDetailCodeFirst == detail1)
                            && (string.IsNullOrEmpty(detail2) || y.CreditDetailCodeSecond == detail2)
                            && (string.IsNullOrEmpty(account.WarehouseCode) || y.CreditWarehouse == account.WarehouseCode)
                            )
                            .Sum(q => q.Amount);

                            _LuyKeNam.Thu_Amount += _luyKe_PS_Thu;
                            _LuyKeNam.Chi_Amount += _luyKe_PS_Chi;

                            _thuDK += _LuyKeNam.Thu_Amount;
                            _chiDK += _LuyKeNam.Chi_Amount;
                            _LuyKeNam.Residual_Amount = _thuDK - _chiDK;
                            var listLuyKeNo = listLedgerAll.Where(y => y.Month < _ms && y.Year == dtFrom.Year).Where(y => y.DebitCode == _param.AccountCode
                            && y.DebitDetailCodeFirst == detail1
                            && (string.IsNullOrEmpty(detail2) || y.DebitDetailCodeSecond == detail2)
                            && (string.IsNullOrEmpty(wareHouse) || y.DebitWarehouseName == wareHouse)).ToList();

                            var listLuyKeCo = listLedgerAll.Where(y => y.Month < _ms && y.Year == dtFrom.Year)
                            .Where(y => y.CreditCode == _param.AccountCode
                            && y.CreditDetailCodeFirst == _param.AccountCodeDetail1
                            && (string.IsNullOrEmpty(detail2) || y.CreditDetailCodeSecond == detail2)
                            && (string.IsNullOrEmpty(wareHouse) || y.CreditWarehouse == wareHouse)).ToList();

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

                            _LuyKeNam.Residual_Quantity = relations_new.Where(y => y.Month == _ms && y.Year == _year).Sum(q => q.Residual_Quantity);
                            _LuyKeNam.Residual_Amount = relations_new.Where(y => y.Month == _ms && y.Year == _year).Sum(q => q.Residual_Amount);
                        }
                        _luyKe_PS_Ton = _congPS.Residual_Amount;
                        SoChiTietThuChiViewModel _du = new SoChiTietThuChiViewModel();
                        _du.Month = _ms;//dư
                        _du.Type = 3;//dư
                        _du.ArisingDebit_Foreign = 0;
                        _du.ArisingCredit_Foreign = 0;
                        _du.Input_Quantity = 0;
                        _du.Output_Quantity = 0;

                        if (!isNoiBo)
                        {
                            _du.Thu_Amount = (account.OpeningDebit ?? 0) + _LuyKeNam.Thu_Amount;
                            _du.Chi_Amount = (account.OpeningCredit ?? 0) + _LuyKeNam.Chi_Amount;
                            _du.Residual_Amount = (account.OpeningDebit ?? 0) - (account.OpeningCredit ?? 0) + _LuyKeNam.Thu_Amount - _LuyKeNam.Chi_Amount;
                            _du.Residual_Quantity = (account.OpeningStockQuantity ?? 0) + _LuyKeNam.Input_Quantity - _LuyKeNam.Output_Quantity;
                        }
                        else
                        {
                            _du.Thu_Amount = (account.OpeningDebitNB ?? 0) + _LuyKeNam.Thu_Amount;
                            _du.Chi_Amount = (account.OpeningCreditNB ?? 0) + _LuyKeNam.Chi_Amount;
                            _du.Residual_Amount = (account.OpeningDebitNB ?? 0) - (account.OpeningCreditNB ?? 0) + _LuyKeNam.Thu_Amount - _LuyKeNam.Chi_Amount;
                            _du.Residual_Quantity = (account.OpeningStockQuantityNB ?? 0) + _LuyKeNam.Input_Quantity - _LuyKeNam.Output_Quantity;
                        }

                        listThuChi.Add(_congPS);
                        listThuChi.Add(_LuyKeNam);
                        listThuChi.Add(_du);
                    });

                    v_dicTotal.Add(detail1 + "/" + detail2 + "/" + account.WarehouseCode + "/" + String.Format("{0:N0}", _quantityLK) + "/" + String.Format("{0:N0}", _tienLK), listThuChi);
                }

                LedgerReportModel _model = new()
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
                    listAccoutCodeThuChi = v_dicTotal,
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

    }
}
