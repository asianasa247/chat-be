using DinkToPdf.Contracts;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Text;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using ManageEmployee.Services.Interfaces.Ledgers.V2;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Entities.PocoSelections;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.CompanyEntities;

namespace ManageEmployee.Services.LedgerServices.V2;

public class AccountBalanceSheetV2Service : IAccountBalanceSheetV2Service
{
    private readonly ApplicationDbContext _context;
    private readonly ICompanyService _companyService;
    private readonly IConverter _converterPDF;
    private readonly IChartOfAccountService _chartOfAccountService;

    private Company _company;
    private List<AccountBalanceSheetPocoData> _listData = new();
    private AccountBalanceSheetPocoData _sumRow;
    public AccountBalanceSheetV2Service(
        ApplicationDbContext context,
        ICompanyService companyService,
        IConverter converterPDF,
        IChartOfAccountService chartOfAccountService
    )
    {
        _context = context;
        _companyService = companyService;
        _converterPDF = converterPDF;
        _chartOfAccountService = chartOfAccountService;
    }

    public async Task<string> GenerateReport(AccountBalanceReportParam param, int year)
    {
        
        await InitDataAsync(param, year);
        _company = await _companyService.GetCompany();
        if (_company is null)
            _company = new Company();
        var _filePath = "";

        switch (param.FileType)
        {
            case "html":
                _filePath = ConvertToHtml(param, year);
                break;
            case "excel":
                _filePath = ExportExcel_Report(param);
                break;
            case "pdf":
                _filePath = ConvertToPdf(param, year);
                break;
        }

        return _filePath;
    }

    public async Task<(List<AccountBalanceSheetPocoData>, AccountBalanceSheetPocoData)> InitDataAsync(AccountBalanceReportParam param, int year)
    {
        string conditionDebit = "";
        string conditionCredit = "";
        DateTime filterFrom;
        DateTime filterTo;

        if (param.FromDate.HasValue && param.ToDate.HasValue)
        {
            if (param.FromDate.Value > param.ToDate.Value)
            {
                throw new("Từ ngày phải nhỏ hơn hoặc bằng tới ngày !");
            }

            filterFrom = new DateTime(param.FromDate.Value.Year, param.FromDate.Value.Month, param.FromDate.Value.Day, 0, 0, 0);
            filterTo = new DateTime(param.ToDate.Value.Year, param.ToDate.Value.Month, param.ToDate.Value.Day, 0, 0, 0).AddDays(1);
            param.FromMonth = filterFrom.Month;
            param.ToMonth = filterTo.Month;
        }
        else
        {
            filterFrom = new DateTime(year, param.FromMonth.Value, 1, 0, 0, 0);
            filterTo = new DateTime(year, param.ToMonth.Value, 1, 0, 0, 0).AddMonths(1);
            param.FromDate = filterFrom;
            param.ToDate = filterTo;
        }
        var pocoResultArsing = new List<AccountBalanceSheetPoco>();
        var pocoResultLK = new List<AccountBalanceSheetPoco>();
        string conditionAccount = " where coa.year = " + year;

        if (!string.IsNullOrEmpty(param.AccountCode))
        {
            conditionAccount += " and coa.Code = '" + param.AccountCode + "' or coa.parentRef like '" + param.AccountCode + "%'";
            var account = await _chartOfAccountService.GetAccountByCode(param.AccountCode, year);
            param.AccountName = account.Name;
        }


        string ledgerInternal = " and l.IsInternal = " + (param.IsNoiBo ? 3 : 1);
        if (!param.IsNoiBo)
        {
            ledgerInternal = " and l.IsInternal in (1,2)";
        }

        var queryAccount = "coa.OpeningDebit as OpenDebitAmount , coa.OpeningCredit as OpenCreditAmount, ";
        if (param.IsNoiBo)
            queryAccount = "coa.OpeningDebitNb as OpenDebitAmount , coa.OpeningCreditNb as OpenCreditAmount, ";

        for (int i = 0; i < 2; i++)
        {
            conditionDebit = " ";
            conditionCredit = " ";
            if (!string.IsNullOrEmpty(param.AccountCode))
            {
                conditionDebit = " and DebitCode = '" + param.AccountCode + "'";
                conditionCredit = " and CreditCode = '" + param.AccountCode + "'";
            }

            if (!string.IsNullOrEmpty(param.StartAccount))
            {
                conditionDebit = " and DebitCode like '" + param.StartAccount + "%'";
                conditionCredit = " and CreditCode like '" + param.StartAccount + "%'";
            }

            if (i == 0)
            {
                conditionDebit += " and l.OrginalBookDate >= '" + filterFrom.ToString("yyyy/MM/dd") + "' and l.OrginalBookDate < '" + filterTo.ToString("yyyy/MM/dd") + "'";
                conditionCredit += " and l.OrginalBookDate >= '" + filterFrom.ToString("yyyy/MM/dd") + "' and l.OrginalBookDate < '" + filterTo.ToString("yyyy/MM/dd") + "'";
            }
            else if (i == 1)
            {
                conditionDebit += " and l.Year = " + filterFrom.Year + " and l.OrginalBookDate < '" + filterTo.ToString("yyyy/MM/dd") + "'";
                conditionCredit += " and l.Year = " + filterFrom.Year + " and l.OrginalBookDate < '" + filterTo.ToString("yyyy/MM/dd") + "'";
            }

            conditionDebit = conditionDebit + ledgerInternal;
            conditionCredit = conditionCredit + ledgerInternal;

            var debitCodeSecondArising = @"select sum(Amount) debitAmount, 0 as creditAmount, DebitDetailCodeSecond as accountCode, CONCAT(DebitCode, ':', DebitDetailCodeFirst) as parentRef, 
                                                    CASE WHEN  DebitWarehouse is null THEN '' ELSE DebitWarehouse END as wareHouseCode
	                                from Ledgers l
	                                where DebitDetailCodeSecond is not null and DebitDetailCodeSecond != '' " + conditionDebit +
                                    "group by DebitDetailCodeSecond, DebitDetailCodeFirst, DebitCode, DebitWarehouse";
            var debitCodeFirstArising = @"select sum(Amount) debitAmount, 0 as creditAmount, DebitDetailCodeFirst as accountCode, DebitCode as parentRef, 
                                                    CASE WHEN  DebitWarehouse is null THEN '' ELSE DebitWarehouse END as wareHouseCode
                                    from Ledgers l
                                    where DebitDetailCodeFirst is not null and DebitDetailCodeFirst != ''  " + conditionDebit +
                                    "group by DebitDetailCodeFirst, DebitCode, DebitWarehouse  ";

            var debitCodeArising = @"select sum(Amount) debitAmount, 0 as creditAmount, DebitCode as accountCode, 
                                    CASE WHEN LEN(DebitCode) > 3 THEN  SUBSTRING(DebitCode, 0, LEN(DebitCode)) ELSE '' END  as parentRef, 
                                        CASE WHEN  DebitWarehouse is null THEN '' ELSE DebitWarehouse END as wareHouseCode
                                    from Ledgers l
                                    where DebitCode is not null and DebitCode != ''" + conditionDebit +
                                    "group by DebitCode, DebitWarehouse  ";
            var creditCodeSecondArising = @"select 0 as debitAmount, sum(Amount) creditAmount, CreditDetailCodeSecond as accountCode, CONCAT(CreditCode , ':', CreditDetailCodeFirst) as parentRef, 
                                                        CASE WHEN  CreditWarehouse is null THEN '' ELSE CreditWarehouse END as wareHouseCode 
                                            from  Ledgers l
                                            where CreditDetailCodeSecond is not null and CreditDetailCodeSecond != ''  " + conditionCredit +
                                            "group by CreditDetailCodeSecond, CreditDetailCodeFirst , CreditCode, CreditWarehouse ";
            var creditCodeFirstArising = @"select 0 as debitAmount,  sum(Amount) creditAmount, CreditDetailCodeFirst as accountCode, CreditCode as parentRef, 
                                                    CASE WHEN  CreditWarehouse is null THEN '' ELSE CreditWarehouse END as wareHouseCode 
                                            from  Ledgers l
                                            where CreditDetailCodeFirst is not null and CreditDetailCodeFirst != ''  " + conditionCredit +
                                            "group by CreditDetailCodeFirst, CreditCode, CreditWarehouse";
            var creditCodeArising = @"select 0 as debitAmount, sum(Amount) creditAmount, CreditCode as accountCode, 
                                CASE WHEN  LEN(CreditCode) > 3 THEN SUBSTRING(CreditCode, 0, LEN(CreditCode)) ELSE '' END  as parentRef, 
                                    CASE WHEN  CreditWarehouse is null THEN '' ELSE CreditWarehouse END as wareHouseCode 
                                        from Ledgers l
                                        where CreditCode is not null and CreditCode != '' " + conditionCredit +
                                        "group by CreditCode, CreditWarehouse  ";

            var queryArising = @"select coa.Id as Id, coa.Code as Code, coa.Name as Name,coa.ParentRef as ParentRef, coa.Type as AccountType, 
                                        CASE WHEN  coa.wareHouseCode is null THEN '' ELSE coa.wareHouseCode END as wareHouseCode 
                                    , coa.WareHouseName as wareHouseName, "
                            + queryAccount
                            + " le.debitAmount as ArisingDebitAmount, le.creditAmount as ArisingCreditAmount"
                            + " from ChartOfAccounts coa"
                            + " left join (select sum(lk.debitAmount) as debitAmount , sum(lk.creditAmount) as creditAmount,lk.accountCode as accountCode, lk.parentRef as parentRef, lk.wareHouseCode  from (( " + debitCodeSecondArising
                                    + ") UNION (" + debitCodeFirstArising
                                    + ") UNION (" + debitCodeArising
                                    + ") UNION (" + creditCodeSecondArising
                                    + ") UNION (" + creditCodeFirstArising
                                    + ") UNION (" + creditCodeArising
                                    + ") ) as lk"
                                    + " group by lk.accountCode, lk.parentRef, lk.wareHouseCode)" +
                                    "as le on coa.Code = le.accountCode and coa.ParentRef = le.parentRef and coa.wareHouseCode = le.wareHouseCode "
                                    + conditionAccount;
            if (i == 0)
            {
                pocoResultArsing = await _context
                .Set<AccountBalanceSheetPoco>()
                .FromSqlRaw(queryArising)
                .ToListAsync();
            }
            else if (i == 1)
            {
                pocoResultLK = await _context
                .Set<AccountBalanceSheetPoco>()
                .FromSqlRaw(queryArising)
                .ToListAsync();
            }
        }
        _sumRow = new AccountBalanceSheetPocoData
        {
            OpenDebitAmount = 0,
            OpenCreditAmount = 0,
            ArisingDebitAmount = 0,
            ArisingCreditAmount = 0,
            CumulativeCreditAmount = 0,
            CumulativeDebitAmount = 0,
            EndDebitAmount = 0,
        };
        List<AccountBalanceSheetPocoData> listOut = new();

        foreach (var item in pocoResultArsing)
        {
            var itemOut = new AccountBalanceSheetPocoData
            {
                Id = item.Id,
                Code = item.Code,
                ParentRef = item.ParentRef,
                Name = item.Name,
                AccountType = item.AccountType,
                OpenDebitAmount = item.OpenDebitAmount ?? 0,
                OpenCreditAmount = item.OpenCreditAmount ?? 0,
                ArisingDebitAmount = item.ArisingDebitAmount ?? 0,
                ArisingCreditAmount = item.ArisingCreditAmount ?? 0,
                WareHouseCode = item.WareHouseCode,
                WareHouseName = item.WareHouseName,
            };
            var cumulativeItem = pocoResultLK.Find(x => x.Id == item.Id);
            if (cumulativeItem is null)
                continue;
            itemOut.CumulativeDebitAmount = (cumulativeItem.OpenDebitAmount ?? 0) + (cumulativeItem.ArisingDebitAmount ?? 0);
            itemOut.CumulativeCreditAmount = (cumulativeItem.OpenCreditAmount ?? 0) + (cumulativeItem.ArisingCreditAmount ?? 0);

            itemOut.EndDebitAmount = (item.OpenDebitAmount ?? 0) + (itemOut.ArisingDebitAmount ?? 0) - (item.OpenCreditAmount ?? 0) - (item.ArisingCreditAmount ?? 0);
            listOut.Add(itemOut);
        }

        // de quy tinh tong cha
        if (listOut.Count > 0)
        {
            var minAccountType = listOut.Min(x => x.AccountType);
            var listParent = listOut.Where(x => x.AccountType == minAccountType).ToList();
            foreach (var parent in listParent)
            {
                _listData.Add(parent);
                DeQuy(parent, listOut);
            }

            _sumRow.OpenDebitAmount = listParent.Sum(x => x.OpenDebitAmount ?? 0);
            _sumRow.OpenCreditAmount = listParent.Sum(x => x.OpenCreditAmount ?? 0);
            _sumRow.ArisingDebitAmount = listParent.Sum(x => x.ArisingDebitAmount ?? 0);
            _sumRow.ArisingCreditAmount = listParent.Sum(x => x.ArisingCreditAmount ?? 0);
            _sumRow.CumulativeDebitAmount = listParent.Sum(x => x.CumulativeDebitAmount ?? 0);
            _sumRow.CumulativeCreditAmount = listParent.Sum(x => x.CumulativeCreditAmount ?? 0);
            _sumRow.EndCreditAmount = listParent.Where(x => x.EndDebitAmount < 0).Sum(x => x.EndDebitAmount ?? 0);
            _sumRow.EndDebitAmount = listParent.Where(x => x.EndDebitAmount > 0).Sum(x => x.EndDebitAmount ?? 0);
        }

        if (!param.PrintType.Contains((int)AccountBalanceReportTypeEnum.inCaChiTietCap2))
        {
            _listData = _listData.Where(x =>
                !x.AccountType.Equals(6)
                ).ToList();
        }
        if (!param.PrintType.Contains((int)AccountBalanceReportTypeEnum.inCaChiTietCap1))
        {
            _listData = _listData.Where(x =>
                !x.AccountType.Equals(5) || !x.AccountType.Equals(6)
                ).ToList();
        }

        if (param.PrintType.Contains((int)AccountBalanceReportTypeEnum.khongInNhungDongKhongSoLieu))
        {
            _listData = _listData.Where(x =>
                            x.OpenDebitAmount + x.OpenCreditAmount + x.ArisingDebitAmount + x.ArisingCreditAmount + x.CumulativeDebitAmount + x.CumulativeCreditAmount + x.EndDebitAmount > 0
                             ).ToList();
        }

        return (_listData, _sumRow);
    }
    private void DeQuy(AccountBalanceSheetPocoData item, List<AccountBalanceSheetPocoData> listOut)
    {
        var parentRef = item.Code;
        if (item.AccountType == 5)
        {
            parentRef = $"{item.ParentRef}:{item.Code}";
        }

        item.Code = item.AccountType == 5 ? "+" + item.Code :
           item.AccountType == 6 ? "-" + item.Code : item.Code;

        List<AccountBalanceSheetPocoData> children = listOut.Where(x => x.ParentRef == parentRef).ToList();

        if (children.Count == 0)
        {
            _listData.AddRange(children);
            return;
        }

        foreach (var child in children)
        {
            _listData.Add(child);
            DeQuy(child, listOut);
        }
        item.OpenDebitAmount = children.Sum(x => x.OpenDebitAmount ?? 0);
        item.OpenCreditAmount = children.Sum(x => x.OpenCreditAmount ?? 0);
        item.ArisingDebitAmount = children.Sum(x => x.ArisingDebitAmount ?? 0);
        item.ArisingCreditAmount = children.Sum(x => x.ArisingCreditAmount ?? 0);
        item.CumulativeDebitAmount = children.Sum(x => x.CumulativeDebitAmount ?? 0);
        item.CumulativeCreditAmount = children.Sum(x => x.CumulativeCreditAmount ?? 0);
        item.EndDebitAmount = children.Sum(x => x.EndDebitAmount ?? 0);
    }
    string ConvertToHtml(AccountBalanceReportParam param, int year)
    {
        try
        {
            bool _hasLuyKe = false;
            if (param.PrintType.Contains((int)AccountBalanceReportTypeEnum.inCotLuyKePhatSinh))
                _hasLuyKe = true;

            string _template = _hasLuyKe ? "BangCanDoiTaiKhoanTemplate.html" : "BangCanDoiTaiKhoanNoLKTemplate.html",
                _folderPath = @"Uploads\Html", resultStr = string.Empty,
                path = Path.Combine(Directory.GetCurrentDirectory(), _folderPath, _template),
                _allText = File.ReadAllText(path),
                _loopReplace = @"<tr class='#SPECIAL_ROW#'>
                        <td class='txt-left #TK_ACCOUNT_5_6#'>{{{MATK}}}</td>
                        <td class='txt-left'>{{{TENTK}}}</td>
                        <td class='txt-right'>{{{NO_SDDK}}}</td>
                        <td class='txt-right'>{{{CO_SDDK}}}</td>
                        <td class='txt-right'>{{{NO_PSTK}}}</td>
                        <td class='txt-right'>{{{CO_PSTK}}}</td>
                        #THEM_COT_PSLK#
                        <td class='txt-right'>{{{NO_SDCK}}}</td>
                        <td class='txt-right'>{{{CO_SDCK}}}</td>
                    </tr>", _psLK_Row = @"
                        <td class='txt-right'>{{{NO_PSLK}}}</td>
                        <td class='txt-right'>{{{CO_PSLK}}}</td>";

            if (_hasLuyKe)
                _loopReplace = _loopReplace.Replace("#THEM_COT_PSLK#", _psLK_Row);
            else
                _loopReplace = _loopReplace.Replace("#THEM_COT_PSLK#", string.Empty);

            IDictionary<string, string> v_dicFixed = new Dictionary<string, string>
            {
                { "TenCongTy", _company.Name },
                { "DiaChi", _company.Address },
                { "MST", _company.MST },
                { "TuThang", param.FromDate.Value.Month.ToString("D2")},
                { "DenThang",  param.ToDate.Value.AddDays(-1).Month.ToString("D2") },
                { "Nam", year.ToString() },
                { "NguoiLap", !string.IsNullOrEmpty(param.PreparedBy) ? param.PreparedBy : string.Empty },
                { "KeToanTruong", param.FillFullName ? _company.NameOfChiefAccountant : string.Empty  },
                { "GiamDoc", param.FillFullName ? _company.NameOfCEO : string.Empty },
                { "KeToanTruong_CV",  _company.NoteOfChiefAccountant   },
                { "GiamDoc_CV", _company.NoteOfCEO  },
                { "TIEU_DE", string.IsNullOrEmpty(param.AccountCode) ? "BẢNG CÂN ĐỐI TÀI KHOẢN" : param.AccountCode.ToUpper()}

            };

            v_dicFixed.Keys.ToList().ForEach(x => _allText = _allText.Replace("{{{" + x + "}}}", v_dicFixed[x]));


            _listData.ForEach(x =>
            {
                string _tmp = _loopReplace;
                _tmp = _tmp
                .Replace("#SPECIAL_ROW#", x.AccountType > 0 ? x.AccountType.Equals(1) ? x.Code.Length == 3 ? "r-nm-bold" : "r-nm-normal" : x.AccountType.Equals(5) || x.AccountType.Equals(6) ? "r-nm-italic" : "r-unknow" : "normal-r")
                .Replace("{{{MATK}}}", x.Code + (string.IsNullOrEmpty(x.WareHouseCode) ? "" : " - " + x.WareHouseCode))
                .Replace("#TK_ACCOUNT_5_6#", x.AccountType == 5 ? "account_5" : x.AccountType == 6 ? "account_6" : "")
                .Replace("{{{TENTK}}}", x.Name + (string.IsNullOrEmpty(x.WareHouseCode) ? "" : " - " + x.WareHouseName))

                .Replace("{{{NO_PSTK}}}", string.Format("{0:N0}", x.ArisingDebitAmount > 0 ? x.ArisingDebitAmount : string.Empty))
                .Replace("{{{CO_PSTK}}}", string.Format("{0:N0}", x.ArisingCreditAmount > 0 ? x.ArisingCreditAmount : string.Empty))
                .Replace("{{{NO_PSLK}}}", string.Format("{0:N0}", x.CumulativeDebitAmount > 0 ? x.CumulativeDebitAmount : string.Empty))
                .Replace("{{{CO_PSLK}}}", string.Format("{0:N0}", x.CumulativeCreditAmount > 0 ? x.CumulativeCreditAmount : string.Empty))
                    .Replace("{{{NO_SDDK}}}", string.Format("{0:N0}", x.OpenDebitAmount > 0 ? x.OpenDebitAmount : string.Empty))
                    .Replace("{{{CO_SDDK}}}", string.Format("{0:N0}", x.OpenCreditAmount > 0 ? x.OpenCreditAmount : string.Empty))

                    .Replace("{{{NO_SDCK}}}", string.Format("{0:N0}", x.EndDebitAmount > 0 ? x.EndDebitAmount : string.Empty))
                .Replace("{{{CO_SDCK}}}", string.Format("{0:N0}", x.EndDebitAmount < 0 ? Math.Abs(x.EndDebitAmount ?? 0) : string.Empty));

                resultStr += _tmp;
            });

            _loopReplace = _loopReplace.Replace("#SPECIAL_ROW#", "r-nm-bold")
                .Replace("{{{MATK}}}", string.Empty)
                .Replace("<td class='txt-left'>{{{TENTK}}}</td>", "<td class='txt-center'>CỘNG</td>")

                .Replace("{{{NO_PSTK}}}", string.Format("{0:N0}", _sumRow.ArisingDebitAmount))
                .Replace("{{{CO_PSTK}}}", string.Format("{0:N0}", _sumRow.ArisingCreditAmount))
                .Replace("{{{NO_PSLK}}}", string.Format("{0:N0}", _sumRow.CumulativeDebitAmount))
                .Replace("{{{CO_PSLK}}}", string.Format("{0:N0}", _sumRow.CumulativeCreditAmount))
                .Replace("{{{NO_SDCK}}}", string.Format("{0:N0}", Math.Abs(_sumRow.EndDebitAmount ?? 0)))
                .Replace("{{{CO_SDCK}}}", string.Format("{0:N0}", Math.Abs(_sumRow.EndCreditAmount ?? 0)))
            .Replace("{{{NO_SDDK}}}", string.Format("{0:N0}", _sumRow.OpenDebitAmount))
            .Replace("{{{CO_SDDK}}}", string.Format("{0:N0}", Math.Abs(_sumRow.OpenCreditAmount ?? 0)));

            resultStr += _loopReplace;

            _allText = _allText.Replace("##REPLACE_PLACE##", resultStr);
            return _allText;
        }
        catch
        {
            return string.Empty;
        }
    }
    string ConvertToPdf(AccountBalanceReportParam param, int year)
    {
        try
        {
            string _allText = ConvertToHtml(param, year);
            return ExcelHelpers.ConvertUseDinkLandscape(_allText, _converterPDF, Directory.GetCurrentDirectory(), "BangCanDoiTaiKhoan");
        }
        catch
        {
            return string.Empty;
        }
    }
    string ExportExcel_Report(AccountBalanceReportParam param)
    {
        try
        {
            bool _hasLuyKe = false;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using ExcelPackage package = new ExcelPackage();
            using ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheetName");

            if (param.PrintType.Contains((int)AccountBalanceReportTypeEnum.inCotLuyKePhatSinh))
                _hasLuyKe = true;


            //A => J
            worksheet.Cells[_hasLuyKe ? "A1:J1" : "A1:H1"].Merge = true;
            worksheet.Cells[_hasLuyKe ? "A2:J2" : "A2:H2"].Merge = true;
            worksheet.Cells[_hasLuyKe ? "A3:J3" : "A3:H3"].Merge = true;

            worksheet.Cells[_hasLuyKe ? "A1:J1" : "A1:H1"].Value = _company.Name;
            worksheet.Cells[_hasLuyKe ? "A2:J2" : "A2:H2"].Value = _company.Address;
            worksheet.Cells[_hasLuyKe ? "A3:J3" : "A3:H3"].Value = _company.MST;


            worksheet.Cells[_hasLuyKe ? "A5:J5" : "A5:H5"].Merge = true;
            worksheet.Cells[_hasLuyKe ? "A5:J5" : "A5:H5"].Value = "BẢNG CÂN ĐỐI TÀI KHOẢN";

            worksheet.Cells[_hasLuyKe ? "A6:J6" : "A6:H6"].Merge = true;
            worksheet.Cells[_hasLuyKe ? "A6:J6" : "A6:H6"].Value = "Từ tháng " + param.FromMonth.Value.ToString("D2") + " đến tháng " + param.FromMonth.Value.ToString("D2") + " năm " + param.FromDate.Value.Year.ToString("D4");

            worksheet.Cells[_hasLuyKe ? "I8:J8" : "G8:H8"].Merge = true;
            worksheet.Cells[_hasLuyKe ? "I8:J8" : "G8:H8"].Value = "Đơn vị tính :  Đồng";


            //table
            worksheet.Cells["A9:A10"].Merge = true;
            worksheet.Cells["A9:A10"].Value = "MÃ TK";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["A9:A10"]);

            worksheet.Cells["B9:B10"].Merge = true;
            worksheet.Cells["B9:B10"].Value = "TÊN TÀI KHOẢN";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["B9:B10"]);

            worksheet.Cells["C9:D9"].Merge = true;
            worksheet.Cells["C9:D9"].Value = "SỐ DƯ ĐẦU KỲ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["C9:D9"]);

            worksheet.Cells["E9:F9"].Merge = true;
            worksheet.Cells["E9:F9"].Value = "PHÁT SINH TRONG KỲ";
            ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["E9:F9"]);

            if (_hasLuyKe)
            {
                worksheet.Cells["G9:H9"].Merge = true;
                worksheet.Cells["G9:H9"].Value = "PHÁT SINH LŨY KẾ";
                ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["G9:H9"]);

                worksheet.Cells["I9:J9"].Merge = true;
                worksheet.Cells["I9:J9"].Value = "SỐ DƯ CUỐI KỲ";
                ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["I9:J9"]);
            }
            else
            {
                worksheet.Cells["G9:H9"].Merge = true;
                worksheet.Cells["G9:H9"].Value = "SỐ DƯ CUỐI KỲ";
                ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["G9:H9"]);
            }

            worksheet.Cells["C10"].Value = "NỢ"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["C10"]);
            worksheet.Cells["D10"].Value = "CÓ"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["D10"]);

            worksheet.Cells["E10"].Value = "NỢ"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["E10"]);
            worksheet.Cells["F10"].Value = "CÓ"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["F10"]);

            worksheet.Cells["G10"].Value = "NỢ"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["G10"]);
            worksheet.Cells["H10"].Value = "CÓ"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["H10"]);

            if (_hasLuyKe)
            {
                worksheet.Cells["I10"].Value = "NỢ"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["I10"]);
                worksheet.Cells["J10"].Value = "CÓ"; ExcelHelpers.Format_Border_Excel_Range(worksheet.Cells["J10"]);
            }

            int currentRowNoBegin = 10;
            int nColEnd = 8;
            int currentRowNo = currentRowNoBegin;

            foreach (var dr in _listData)
            {

                currentRowNo++;
                worksheet.Cells[currentRowNo, 1].Value = dr.Code;
                worksheet.Cells[currentRowNo, 2].Value = dr.Name;
                worksheet.Cells[currentRowNo, 3].Value = dr.OpenDebitAmount;
                worksheet.Cells[currentRowNo, 4].Value = dr.OpenCreditAmount;

                worksheet.Cells[currentRowNo, 5].Value = dr.ArisingDebitAmount;
                worksheet.Cells[currentRowNo, 6].Value = dr.ArisingCreditAmount;

                if (_hasLuyKe)
                {
                    worksheet.Cells[currentRowNo, 7].Value = dr.CumulativeDebitAmount;
                    worksheet.Cells[currentRowNo, 8].Value = dr.CumulativeCreditAmount;
                    worksheet.Cells[currentRowNo, 9].Value = dr.EndDebitAmount > 0 ? dr.EndDebitAmount : 0;
                    worksheet.Cells[currentRowNo, 10].Value = dr.EndDebitAmount < 0 ? Math.Abs(dr.EndDebitAmount ?? 0) : 0;
                }
                else
                {
                    worksheet.Cells[currentRowNo, 7].Value = dr.EndDebitAmount > 0 ? dr.EndDebitAmount : 0;
                    worksheet.Cells[currentRowNo, 8].Value = dr.EndDebitAmount < 0 ? Math.Abs(dr.EndDebitAmount ?? 0) : 0;
                }

                if (dr.AccountType > 0)
                {
                    switch (dr.AccountType)
                    {
                        case 1:
                            worksheet.SelectedRange[currentRowNo, 1, currentRowNo, 10].Style.Font.Bold = dr.Code.Length.Equals(3);
                            break;
                        case 5:
                        case 6:
                            worksheet.SelectedRange[currentRowNo, 1, currentRowNo, 10].Style.Font.Italic = true;
                            break;
                    }
                }
            }

            currentRowNo++;
            worksheet.Cells[currentRowNo, 1].Value = string.Empty;
            worksheet.Cells[currentRowNo, 2].Value = "CỘNG";
            worksheet.Cells[currentRowNo, 3].Value = _sumRow.OpenDebitAmount;
            worksheet.Cells[currentRowNo, 4].Value = _sumRow.OpenCreditAmount;

            worksheet.Cells[currentRowNo, 5].Value = _sumRow.ArisingDebitAmount;
            worksheet.Cells[currentRowNo, 6].Value = _sumRow.ArisingCreditAmount;

            if (_hasLuyKe)
            {
                worksheet.Cells[currentRowNo, 7].Value = _sumRow.CumulativeDebitAmount;
                worksheet.Cells[currentRowNo, 8].Value = _sumRow.CumulativeCreditAmount;
                worksheet.Cells[currentRowNo, 9].Value = _sumRow.EndDebitAmount > 0 ? _sumRow.EndDebitAmount : 0;
                worksheet.Cells[currentRowNo, 10].Value = _sumRow.EndDebitAmount < 0 ? Math.Abs(_sumRow.EndDebitAmount ?? 0) : 0;
                nColEnd = 10;
            }
            else
            {
                worksheet.Cells[currentRowNo, 7].Value = _sumRow.EndDebitAmount > 0 ? _sumRow.EndDebitAmount : 0;
                worksheet.Cells[currentRowNo, 8].Value = _sumRow.EndDebitAmount < 0 ? Math.Abs(_sumRow.EndDebitAmount ?? 0) : 0;
            }

            worksheet.SelectedRange[currentRowNo, 1, currentRowNo, 10].Style.Font.Bold = true;
            worksheet.SelectedRange[currentRowNo, 1, currentRowNo, 10].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
            worksheet.SelectedRange[currentRowNo, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.Cells[currentRowNoBegin, 3, currentRowNo, nColEnd].Style.Numberformat.Format = "_(* #,##0_);_(* (#,##0);_(* \"-\"??_);_(@_)";

            currentRowNo += 2;
            worksheet.Cells[_hasLuyKe ? $"A{currentRowNo}:C{currentRowNo}" : $"A{currentRowNo}:C{currentRowNo}"].Merge = true;
            worksheet.Cells[_hasLuyKe ? $"A{currentRowNo}:C{currentRowNo}" : $"A{currentRowNo}:C{currentRowNo}"].Value = "Người lập biểu";

            worksheet.Cells[_hasLuyKe ? $"D{currentRowNo}:G{currentRowNo}" : $"D{currentRowNo}:E{currentRowNo}"].Merge = true;
            worksheet.Cells[_hasLuyKe ? $"D{currentRowNo}:G{currentRowNo}" : $"D{currentRowNo}:E{currentRowNo}"].Value = _company.NoteOfChiefAccountant;

            worksheet.Cells[_hasLuyKe ? $"H{currentRowNo}:J{currentRowNo}" : $"F{currentRowNo}:H{currentRowNo}"].Merge = true;
            worksheet.Cells[_hasLuyKe ? $"H{currentRowNo}:J{currentRowNo}" : $"F{currentRowNo}:H{currentRowNo}"].Value = _company.NoteOfCEO;

            currentRowNo += 6;
            currentRowNo += 2;
            if (!string.IsNullOrEmpty(param.PreparedBy))
            {
                worksheet.Cells[_hasLuyKe ? $"A{currentRowNo}:C{currentRowNo}" : $"A{currentRowNo}:C{currentRowNo}"].Merge = true;
                worksheet.Cells[_hasLuyKe ? $"A{currentRowNo}:C{currentRowNo}" : $"A{currentRowNo}:C{currentRowNo}"].Value = param.PreparedBy;
            }

            if (param.FillFullName)
            {
                worksheet.Cells[_hasLuyKe ? $"D{currentRowNo}:G{currentRowNo}" : $"D{currentRowNo}:E{currentRowNo}"].Merge = true;
                worksheet.Cells[_hasLuyKe ? $"D{currentRowNo}:G{currentRowNo}" : $"D{currentRowNo}:E{currentRowNo}"].Value = _company.NameOfChiefAccountant;

                worksheet.Cells[_hasLuyKe ? $"H{currentRowNo}:J{currentRowNo}" : $"F{currentRowNo}:H{currentRowNo}"].Merge = true;
                worksheet.Cells[_hasLuyKe ? $"H{currentRowNo}:J{currentRowNo}" : $"F{currentRowNo}:H{currentRowNo}"].Value = _company.NameOfCEO;
            }

            currentRowNo += 2;

            worksheet.Column(1).AutoFit(25);
            worksheet.Column(2).AutoFit(30);
            worksheet.Column(3).AutoFit(15);
            worksheet.Column(4).AutoFit(15);
            worksheet.Column(5).AutoFit(15);
            worksheet.Column(6).AutoFit(15);
            worksheet.Column(7).AutoFit(15);
            worksheet.Column(8).AutoFit(15);
            worksheet.Column(9).AutoFit(15);
            worksheet.Column(10).AutoFit(15);


            worksheet.SelectedRange["A1:A3"].Style.Font.Size = 12;
            worksheet.SelectedRange["A6:J6"].Style.Font.Size = 12;

            worksheet.SelectedRange[_hasLuyKe ? "A5:J5" : "A5:H5"].Style.Font.Size = 16;
            worksheet.SelectedRange[_hasLuyKe ? "A5:J5" : "A5:H5"].Style.Font.Bold = true;
            worksheet.Row(5).Height = 30;

            worksheet.SelectedRange[_hasLuyKe ? "A9:J10" : "A9:H10"].Style.Font.Bold = true;
            worksheet.SelectedRange[_hasLuyKe ? "A9:J10" : "A9:H10"].Style.Font.Size = 14;

            worksheet.Cells[_hasLuyKe ? "A9:J10" : "A9:H10"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.SelectedRange[_hasLuyKe ? "A9:J10" : "A9:H10"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(252, 213, 180));


            worksheet.SelectedRange["A11:J" + currentRowNo].Style.Font.Size = 12;
            worksheet.SelectedRange["A1:J" + currentRowNo].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            worksheet.SelectedRange["A1:J" + currentRowNo].Style.WrapText = true;

            worksheet.SelectedRange["A5:J10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            worksheet.SelectedRange["A1:B3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.SelectedRange["A11:B" + currentRowNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            worksheet.SelectedRange["A11:B" + currentRowNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

            int dataCount = _listData.Count;
            worksheet.SelectedRange["C11:J" + (dataCount + 10)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

            worksheet.SelectedRange[$"A{dataCount + 12}:J" + currentRowNo].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.SelectedRange[$"A{dataCount + 12}:J" + currentRowNo].Style.Font.Bold = true;

            return ExcelHelpers.SaveFileExcel(package, Directory.GetCurrentDirectory(), "BangCanDoiTaiKhoan");
        }
        catch
        {
            return string.Empty;
        }
    }

}