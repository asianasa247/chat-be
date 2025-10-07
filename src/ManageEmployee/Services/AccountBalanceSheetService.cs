using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Extends;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Services.Interfaces.Accounts;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Entities.LedgerEntities;

namespace ManageEmployee.Services;


public class AccountBalanceSheetv2Service : IAccountBalanceSheetService
{
    private readonly ApplicationDbContext _context;
    public AccountBalanceSheetv2Service(ApplicationDbContext context)
    {
        _context = context;
    }

    private async Task<List<AccountBalanceSheetTempModel>> CalculateBalanceSheet(DateTime from, DateTime to, bool isNoiBo, int year, string filterAccount = "")
    {
        var filterFrom = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0);
        var filterTo = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);
        var filterAcc = filterAccount?.Trim() ?? string.Empty;
        // 1. Lấy toàn bộ nội dung bảng tài khoản, chỉ lấy giá trị đầu kỳ, giá trị phát sinh và cuối
        // kỳ để mặc định

        var chartOfAccounts = _context.GetChartOfAccount(year)
            .Select(x => new AccountBalanceSheetTempModel
            {
                Code = x.Code,
                Name = x.Name,
                Type = x.Type,
                Parent = x.ParentRef,
                OpeningDebit = x.OpeningDebit ?? 0,
                OpeningCredit = x.OpeningCredit ?? 0,
                OpeningForeignDebit = x.OpeningForeignDebit ?? 0,
                OpeningForeignCredit = x.OpeningForeignCredit ?? 0,
                IsForeignCurrency = false, // x.IsForeignCurrency #ToDO: For debug only
                WarehouseCode = x.WarehouseCode ?? string.Empty,
                Duration = x.Duration,
                ParentRef = x.ParentRef,
                OpeningDebitNB = x.OpeningDebitNB ?? 0,
                OpeningCreditNB = x.OpeningCreditNB ?? 0,
                OpeningForeignDebitNB = x.OpeningForeignDebitNB ?? 0,
                OpeningForeignCreditNB = x.OpeningForeignCreditNB ?? 0,
            });

        // Lấy ra danh sách tài khoản
        var filter = chartOfAccounts
            .Where(x => x.Type <= 4);

        if (!string.IsNullOrEmpty(filterAcc)
           && filterAcc.Length >= 3
           && filterAcc.Length <= 8
           )
        {
            filter = filter
                .Where(x => x.Code.StartsWith(filterAcc));
        };

        var accountOnly =await filter
            .OrderBy(x => x.Code)
            .ToListAsync();

        // Lấy ra danh sách chi tiết
        var detailOnly = await chartOfAccounts
            .Where(x => x.Type > 4 && ( string.IsNullOrEmpty(filterAcc) || x.Parent.StartsWith(filterAcc)))
            .ToListAsync();

        // Duyệt qua danh sách tài khoản, gắn chi tiết thành con của tài khoản
        accountOnly
            .ForEach(x =>
            {
                var parents = new List<string>();

                var code = x.Code;

                while (code.Length >= 3)
                {
                    parents.Prepend(code);
                    code = code.Substring(0, code.Length - 1);
                }

                // Tham chiếu đến tài khoản cha
                var parentCode = x.Code.Substring(0, x.Code.Length - 1);
                var parentRef = accountOnly
                    .FirstOrDefault(c => c.Code == parentCode);

                x.Parent = parentCode;
                x.Ref = parentRef;

                x.Hash = x.Code; // string.Join("/", parents);
                                 // Chi tiết 1
                x.Childs = detailOnly
                        .Where(y => y.Type == 5 && y.Parent == x.Code)
                        .Select(y =>
                        {
                            var parent = string.Join(":", new[] { x.Code, y.Code });
                            var detailLevelOne = new AccountBalanceSheetTempModel();
                            detailLevelOne.Code = y.Code;
                            detailLevelOne.Name = y.Name;
                            detailLevelOne.Parent = x.Code;
                            detailLevelOne.Type = y.Type;
                            detailLevelOne.Duration = y.Duration;
                            detailLevelOne.IsForeignCurrency = x.IsForeignCurrency; // Lấy loại ghi chép của cha
                        detailLevelOne.Ref = x;
                            detailLevelOne.ParentRef = y.ParentRef;
                            detailLevelOne.OpeningDebit = y.OpeningDebit;
                            detailLevelOne.OpeningCredit = y.OpeningCredit;
                            detailLevelOne.OpeningForeignDebit = y.OpeningForeignDebit;
                            detailLevelOne.OpeningForeignCredit = y.OpeningForeignCredit;
                            detailLevelOne.WarehouseCode = y.WarehouseCode;
                        // Phát sinh và cuối kỳ mặc định
                        detailLevelOne.Hash = string.Join("/",
                            new[] {
                            x.Code,
                            y.Code,
                                // y.WarehouseCode
                            }.Where(s => !string.IsNullOrEmpty(s)));
                            detailLevelOne.Childs = detailOnly // chi tiết 2
                                .Where(z => z.Type == 6 && z.Parent == parent)
                                  .Select(z =>
                                      new AccountBalanceSheetTempModel
                                      {
                                          Code = z.Code,
                                          Name = z.Name,
                                          Parent = y.Code,
                                          Type = z.Type,
                                          IsForeignCurrency = y.IsForeignCurrency, // Lấy loại ghi chép của cha
                                      Ref = detailLevelOne,
                                          ParentRef = z.ParentRef,
                                          OpeningDebit = z.OpeningDebit,
                                          OpeningCredit = z.OpeningCredit,
                                          OpeningForeignDebit = z.OpeningForeignDebit,
                                          OpeningForeignCredit = z.OpeningForeignCredit,
                                          //
                                          OpeningDebitNB = z.OpeningDebitNB,
                                          OpeningCreditNB = z.OpeningCreditNB,
                                          OpeningForeignDebitNB = z.OpeningForeignDebitNB,
                                          OpeningForeignCreditNB = z.OpeningForeignCreditNB,
                                          //
                                          WarehouseCode = z.WarehouseCode,
                                          Hash = string.Join("/",
                                          new[] {
                                          x.Code,
                                          y.Code,
                                          z.Code,
                                              //    z.WarehouseCode
                                          }
                                          .Where(s => !string.IsNullOrEmpty(s)))
                                      })
                                  .ToList();
                            return detailLevelOne;
                        }).ToList();
            });


        // Làm phẳng giá trị bảng
        var flatten = accountOnly
            .TraverseX(x => x.Childs).ToList();
        // Lấy theo ngày ghi sổ
        List<Ledger>  arisingLedgers = await _context.GetLedger(year, isNoiBo ? 3 : 2)
        .Where(x => x.OrginalBookDate.Value >= filterFrom && x.OrginalBookDate <= filterTo)
        .ToListAsync();
        // Lấy phát sinh có từ bảng phát sinh
        var araisingCredits = arisingLedgers
        .Select(x => new
        {
            x.CreditCode,
            CreditDetailCodeFirst = x.CreditDetailCodeFirst ?? string.Empty,
            CreditDetailCodeSecond = x.CreditDetailCodeSecond ?? string.Empty,
            CreditWarehouse = x.CreditWarehouse ?? string.Empty,
            Amount = x.Amount // Tổng tiền
        })
        .GroupBy(x => new
        {
            x.CreditCode,
            x.CreditDetailCodeFirst,
            x.CreditDetailCodeSecond,
            x.CreditWarehouse
        })
        .Select(x => new
        {
            x.First().CreditCode,
            x.First().CreditDetailCodeFirst,
            x.First().CreditDetailCodeSecond,
            x.First().CreditWarehouse,
            Amount = x.Sum(y => y.Amount)
        })
        .Select(x => new
        {
            CreditCode = x.CreditCode,
            CreditDetailCodeFirst = x.CreditDetailCodeFirst,
            CreditDetailCodeSecond = x.CreditDetailCodeSecond,
            Amount = x.Amount,
            CreditWarehouse = x.CreditWarehouse,
            Hash = string.Join("/", new[] {
                x.CreditCode,
                x.CreditDetailCodeFirst,
                x.CreditDetailCodeSecond,
                //x.CreditWarehouse
            }.Where(s => !string.IsNullOrEmpty(s)))
        })
        .OrderBy(x => x.CreditCode)
        .ToList();

        // Tổng hợp có từ bảng Phát sinh vào bảng tài khoản
        flatten.ForEach(x =>
        {
            araisingCredits.ForEach(y =>
            {
                if (x.Hash == y.Hash && x.WarehouseCode == y.CreditWarehouse)
                {
                    if (!x.IsForeignCurrency)
                    {
                        x.ArisingCredit = y.Amount;
                    }
                    else
                    {
                        x.ArisingForeignCredit = y.Amount;
                    }

                }
            });
        });


        // Danh sách phát sinh nợ
        var araisingDebits = arisingLedgers
        .Select(x => new
        {
            x.DebitCode,
            DebitDetailCodeFirst = x.DebitDetailCodeFirst ?? string.Empty,
            DebitDetailCodeSecond = x.DebitDetailCodeSecond ?? string.Empty,
            DebitWarehouse = x.DebitWarehouse ?? string.Empty,
            Amount = x.Amount
        })
        .GroupBy(x => new
        {
            x.DebitCode,
            x.DebitDetailCodeFirst,
            x.DebitDetailCodeSecond,
            x.DebitWarehouse
        }).Select(x => new
        {
            x.First().DebitCode,
            x.First().DebitDetailCodeFirst,
            x.First().DebitDetailCodeSecond,
            x.First().DebitWarehouse,
            Amount = x.Sum(y => y.Amount)
        })
        .Select(x => new
        {
            DebitCode = x.DebitCode,
            DebitDetailCodeFirst = x.DebitDetailCodeFirst,
            DebitDetailCodeSecond = x.DebitDetailCodeSecond,
            Amount = x.Amount,
            DebitWarehouse = x.DebitWarehouse,
            Hash = string.Join("/", new[] {
                    x.DebitCode,
                    x.DebitDetailCodeFirst,
                    x.DebitDetailCodeSecond,
                    //x.DebitWarehouse
                }.Where(s => !string.IsNullOrEmpty(s)))
        })
        .OrderBy(x => x.DebitCode)
        .ToList();

        // Tổng hợp Nợ vào bảng tài khoản
        flatten.ForEach(x =>
        {
            araisingDebits.ForEach(y =>
            {
                if (x.Hash == y.Hash && x.WarehouseCode == y.DebitWarehouse)
                {
                    if (!x.IsForeignCurrency)
                    {
                        x.ArisingDebit = y.Amount;
                    }
                    else
                    {
                        x.ArisingForeignDebit = y.Amount;
                    }

                }
            });
        });

        // Tính ngược lên tài khoản cha
        double closing = 0;
        double closingForeign = 0;

        // Xoá trống giá trị của parent -> để tính lại
        flatten.ForEach(x =>
        {
            var currentRef = x.Ref;
            while (currentRef != null)
            {
                currentRef.HasChild = true;
                currentRef.ArisingCredit = 0;
                currentRef.ArisingDebit = 0;
                currentRef.ArisingForeignCredit = 0;
                currentRef.ArisingForeignDebit = 0;
                currentRef.CumulativeArisingCredit = 0;
                currentRef.CumulativeArisingDebit = 0;
                currentRef.CumulativeArisingForeignCredit = 0;
                currentRef.CumulativeArisingForeignDebit = 0;
                currentRef.ClosingCredit = 0;
                currentRef.ClosingDebit = 0;
                currentRef.ClosingForeignCredit = 0;
                currentRef.ClosingForeignDebit = 0;
                currentRef = currentRef.Ref;
            }
        });

        flatten.ForEach(x =>
        {
            if (isNoiBo)
            {
                closing = (x.OpeningCreditNB + x.ArisingCredit) - (x.OpeningDebitNB + x.ArisingDebit);
                closingForeign = (x.OpeningForeignCreditNB + x.ArisingForeignCredit) - (x.OpeningForeignDebitNB + x.ArisingForeignDebit);

            }
            else
            {
                closing = (x.OpeningCredit + x.ArisingCredit) - (x.OpeningDebit + x.ArisingDebit);
                closingForeign = (x.OpeningForeignCredit + x.ArisingForeignCredit) - (x.OpeningForeignDebit + x.ArisingForeignDebit);

            }

            x.ClosingCredit = 0;
            x.ClosingDebit = 0;
            if (closing >= 0)
            {
                x.ClosingCredit = closing;
            }
            else
            {
                x.ClosingDebit = Math.Abs(closing);

            }

            x.ClosingForeignCredit = 0;
            x.ClosingForeignDebit = 0;
            if (closingForeign >= 0)
            {
                x.ClosingForeignCredit = closingForeign;

            }
            else
            {
                x.ClosingForeignDebit = Math.Abs(closingForeign);

            }

            var currentRef = x.Ref;

            while (currentRef != null)
            {
                currentRef.ArisingDebit = currentRef.ArisingDebit + x.ArisingDebit;
                currentRef.ArisingCredit = currentRef.ArisingCredit + x.ArisingCredit;
                currentRef.ArisingForeignDebit = currentRef.ArisingForeignDebit + x.ArisingForeignDebit;
                currentRef.ArisingForeignCredit = currentRef.ArisingForeignCredit + x.ArisingForeignCredit;
                currentRef.CumulativeArisingCredit = currentRef.ArisingCredit;
                currentRef.CumulativeArisingDebit = currentRef.ArisingDebit;
                currentRef.CumulativeArisingForeignCredit = currentRef.ArisingForeignCredit;
                currentRef.CumulativeArisingForeignDebit = currentRef.ArisingForeignDebit;
                if (isNoiBo)
                {
                    closing = (currentRef.OpeningCreditNB + currentRef.ArisingCredit) - (currentRef.OpeningDebitNB + currentRef.ArisingDebit);
                    closingForeign = (currentRef.OpeningForeignCreditNB + currentRef.ArisingForeignCredit) - (currentRef.OpeningForeignDebitNB + currentRef.ArisingForeignDebit);
                }
                else
                {
                    closing = (currentRef.OpeningCredit + currentRef.ArisingCredit) - (currentRef.OpeningDebit + currentRef.ArisingDebit);
                    closingForeign = (currentRef.OpeningForeignCredit + currentRef.ArisingForeignCredit) - (currentRef.OpeningForeignDebit + currentRef.ArisingForeignDebit);
                }


                currentRef.ClosingCredit = 0;
                currentRef.ClosingDebit = 0;
                if (closing >= 0)
                {
                    currentRef.ClosingCredit = closing;

                }
                else
                {
                    currentRef.ClosingDebit = Math.Abs(closing);

                }

                currentRef.ClosingForeignCredit = 0;
                currentRef.ClosingForeignDebit = 0;
                if (closingForeign >= 0)
                {
                    currentRef.ClosingForeignCredit = closingForeign;

                }
                else
                {
                    currentRef.ClosingForeignDebit = Math.Abs(closingForeign);

                }

                currentRef = currentRef.Ref;
            }
        });
        return flatten;
    }

    public async Task<AccrualAccountingViewModel> GenerateAccrualAccounting(
        string type, DateTime? fromDate, DateTime? toDate,
        string accountCode, string detail1Code, string detail2Code, bool isNoiBo
    )
    {
        DateTime filterFrom, filterTo;
        if (type == "month")
        {
            filterFrom = new DateTime(fromDate.Value.Year, fromDate.Value.Month, 1, 0, 0, 0);
            filterTo = new DateTime(toDate.Value.Year, toDate.Value.Month, DateTime.DaysInMonth(fromDate.Value.Year, fromDate.Value.Month), 00, 00, 00);
            filterTo = filterTo.AddHours(23);
            filterTo = filterTo.AddMinutes(59);
            filterTo = filterTo.AddSeconds(59);
        }
        else
        {
            filterFrom = new DateTime(fromDate.Value.Year, fromDate.Value.Month, fromDate.Value.Day, 0, 0, 0);
            filterTo = new DateTime(toDate.Value.Year, toDate.Value.Month, toDate.Value.Day, 23, 59, 59);
        }
        var _res = new AccrualAccountingViewModel();
        var charOfAccount1 = await _context.ChartOfAccounts.Where(x => x.Year == filterFrom.Year).ToListAsync();
        // Lấy theo ngày ghi sổ
        var arisingLedgers = await _context.GetLedgerNotForYear(isNoiBo ? 3 : 2).Where(x => x.Year == filterFrom.Year && x.DebitCode != null).ToListAsync();
        double sumDebit = 0, sumCredit = 0;
        double sumOpeningStockFixed = 0;
        if (fromDate.Value.Day == 1 && fromDate.Value.Month == 1)
        {
            var charOfAccounts = charOfAccount1.Where(x => x.Code == accountCode)
                    .Select(x => new { x.Code, TDK = ((isNoiBo ? x.OpeningDebitNB : x.OpeningDebit) ?? 0) - ((isNoiBo ? x.OpeningCreditNB : x.OpeningCredit) ?? 0) }).FirstOrDefault();
            // có cả chi tiết 1 và chi tiết 2
            if (!String.IsNullOrEmpty(detail1Code) && !String.IsNullOrEmpty(detail2Code))
            {
                charOfAccounts = charOfAccount1.Where(x => x.Code == detail2Code && x.ParentRef == accountCode + ":" + detail1Code)
                        .Select(x => new { x.Code, TDK = ((isNoiBo ? x.OpeningDebitNB : x.OpeningDebit) ?? 0) - ((isNoiBo ? x.OpeningCreditNB : x.OpeningCredit) ?? 0) }).FirstOrDefault();
            }
            //có chi tiết 1 và không có chi tiết 2
            else if (!String.IsNullOrEmpty(detail1Code) && String.IsNullOrEmpty(detail2Code))
            {
                charOfAccounts = charOfAccount1.Where(x => x.Code == detail1Code && x.ParentRef == accountCode)
                    .Select(x => new { x.Code, TDK = ((isNoiBo ? x.OpeningDebitNB : x.OpeningDebit) ?? 0) - ((isNoiBo ? x.OpeningCreditNB : x.OpeningCredit) ?? 0) }).FirstOrDefault();
            }
            
            if (charOfAccounts != null)
            {
                _res.OpeningStock = (double)charOfAccounts.TDK;
                sumOpeningStockFixed = _res.OpeningStock;
            }
        }
        else
        {
            var charOfAccounts = charOfAccount1.Where(x => x.Code == accountCode)
                    .Select(x => new { x.Code, TDK = ((isNoiBo ? x.OpeningDebitNB : x.OpeningDebit) ?? 0) - ((isNoiBo ? x.OpeningCreditNB : x.OpeningCredit) ?? 0) }).FirstOrDefault();
            // có mã tài khoản, không có chi tiết 1 và chi tiết 2
            if (!String.IsNullOrEmpty(accountCode) && String.IsNullOrEmpty(detail1Code) && String.IsNullOrEmpty(detail2Code))
            {
                int accountLength = accountCode.Length;
                sumDebit = arisingLedgers.Where(x => x.DebitCode.Length >= accountLength && x.DebitCode.Substring(0, accountLength) == accountCode
                    && x.OrginalBookDate.Value < filterFrom).Sum(x => x.Amount);
                sumCredit = arisingLedgers.Where(x => x.CreditCode.Length >= accountLength && x.CreditCode.Substring(0, accountLength) == accountCode
                    && x.OrginalBookDate.Value < filterFrom).Sum(x => x.Amount);
            }
            // có chi tiết 1 và không có chi tiết 2
            else if (!String.IsNullOrEmpty(detail1Code) && String.IsNullOrEmpty(detail2Code))
            {
                charOfAccounts = charOfAccount1.Where(x => x.Code == detail1Code && x.ParentRef == accountCode)
                    .Select(x => new { x.Code, TDK = ((isNoiBo ? x.OpeningDebitNB : x.OpeningDebit) ?? 0) - ((isNoiBo ? x.OpeningCreditNB : x.OpeningCredit) ?? 0) }).FirstOrDefault();
                sumDebit = arisingLedgers.Where(x => x.DebitCode == accountCode
                    && x.OrginalBookDate.Value < filterFrom && x.DebitDetailCodeFirst == detail1Code).Sum(x => x.Amount);
                sumCredit = arisingLedgers.Where(x => x.CreditCode == accountCode
                    && x.OrginalBookDate.Value < filterFrom && x.CreditDetailCodeFirst == detail1Code).Sum(x => x.Amount);
            }
            // có cả chi tiết 1 và chi tiết 2
            else
            {
                charOfAccounts = charOfAccount1.Where(x => x.Code == detail2Code && x.ParentRef == accountCode + ":" + detail1Code)
                        .Select(x => new { x.Code, TDK = ((isNoiBo ? x.OpeningDebitNB : x.OpeningDebit) ?? 0) - ((isNoiBo ? x.OpeningCreditNB : x.OpeningCredit) ?? 0) }).FirstOrDefault();
                sumDebit = arisingLedgers.Where(x => x.DebitCode == accountCode
                    && x.OrginalBookDate.Value < filterFrom && x.DebitDetailCodeFirst == detail1Code && x.DebitDetailCodeSecond == detail2Code)
                    .Sum(x => x.Amount);
                sumCredit = arisingLedgers.Where(x => x.CreditCode == accountCode
                    && x.OrginalBookDate.Value < filterFrom && x.CreditDetailCodeFirst == detail1Code && x.CreditDetailCodeSecond == detail2Code)
                    .Sum(x => x.Amount);
            }
            
            if (charOfAccounts != null)
            {
                _res.OpeningStock = charOfAccounts.TDK + (sumDebit - sumCredit);
                sumOpeningStockFixed = (double)charOfAccounts.TDK;
            }
        }

        // Tính phát sinh lũy kế
        var noPSDK = (fromDate.Value.Month == 1 && fromDate.Value.Day == 1) ? 0: sumDebit;
        var coPSDK = (fromDate.Value.Month == 1 && fromDate.Value.Day == 1) ? 0 : sumCredit;

        // Tính phát sinh từng đợt theo tháng/ngày
        List<IncurExpense> incurExpenses = new List<IncurExpense>();
        double sumExpense = 0, sumOpeningStock = _res.OpeningStock;
        for (int i = fromDate.Value.Month; i <= toDate.Value.Month; i++)
        {
            // có mã tài khoản, không có chi tiết 1 và chi tiết 2
            if (!String.IsNullOrEmpty(accountCode) && String.IsNullOrEmpty(detail1Code) && String.IsNullOrEmpty(detail2Code))
            {
                if(accountCode.Length == 3)
                {
                    sumDebit = arisingLedgers.Where(x => x.DebitCode.StartsWith(accountCode)
                    && x.OrginalBookDate.Value >= filterFrom && x.OrginalBookDate.Value < filterTo
                    && x.Month == i)
                .Sum(x => x.Amount);
                    sumCredit = arisingLedgers.Where(x => x.CreditCode.StartsWith(accountCode)
                    && x.OrginalBookDate.Value >= filterFrom && x.OrginalBookDate.Value < filterTo
                        && x.Month == i)
                    .Sum(x => x.Amount);
                }
                else
                {
                    sumDebit = arisingLedgers.Where(x => x.DebitCode == accountCode
                        && x.OrginalBookDate.Value >= filterFrom && x.OrginalBookDate.Value < filterTo
                        && x.Month == i)
                    .Sum(x => x.Amount);
                    sumCredit = arisingLedgers.Where(x => x.CreditCode == accountCode
                    && x.OrginalBookDate.Value >= filterFrom && x.OrginalBookDate.Value < filterTo
                        && x.Month == i)
                    .Sum(x => x.Amount);
                }
            }
            // có chi tiết 1 và không có chi tiết 2
            else if (!String.IsNullOrEmpty(detail1Code) && String.IsNullOrEmpty(detail2Code))
            {
                sumDebit = arisingLedgers.Where(x => x.DebitCode == accountCode
                    && x.OrginalBookDate.Value >= filterFrom && x.OrginalBookDate.Value < filterTo
                    && x.DebitDetailCodeFirst == detail1Code
                    && x.Month == i)
                .Sum(x => x.Amount);
                sumCredit = arisingLedgers.Where(x => x.CreditCode == accountCode
                    && x.OrginalBookDate.Value >= filterFrom && x.OrginalBookDate.Value < filterTo
                    && x.CreditDetailCodeFirst == detail1Code
                    && x.Month == i)
                .Sum(x => x.Amount);
            }
            // có cả chi tiết 1 và chi tiết 2
            else
            {
                sumDebit = arisingLedgers.Where(x => x.DebitCode == accountCode
                    && x.OrginalBookDate.Value >= filterFrom && x.OrginalBookDate.Value < filterTo
                    && x.DebitDetailCodeFirst == detail1Code && x.DebitDetailCodeSecond == detail2Code
                    && x.Month == i)
                .Sum(x => x.Amount);
                sumCredit = arisingLedgers.Where(x => x.CreditCode == accountCode
                    && x.OrginalBookDate.Value >= filterFrom && x.OrginalBookDate.Value < filterTo
                    && x.CreditDetailCodeFirst == detail1Code && x.CreditDetailCodeSecond == detail2Code
                    && x.Month == i)
                .Sum(x => x.Amount);
            }
            IncurExpense incurExpense = new IncurExpense
            {
                Name = "T" + i,
                SumDebit = sumDebit,
                SumCredit = sumCredit,
                AccumulatedDebit = noPSDK + sumDebit,
                AccumulatedCredit = coPSDK + sumCredit,
                Expense = sumDebit - sumCredit,
                OpeningStock = sumOpeningStockFixed
            };
            incurExpense.Balance = sumOpeningStockFixed + incurExpense.AccumulatedDebit - incurExpense.AccumulatedCredit;
            sumExpense += incurExpense.Expense;
            // Cộng dần phí phát sinh hằng tháng
            noPSDK += sumDebit;
            coPSDK += sumCredit;
            incurExpenses.Add(incurExpense);
        }
        _res.IncurExpenses = incurExpenses;

        //Tồn cuối kỳ (TCK) = Tồn đầu kỳ + Tổng phát sinh
        _res.ClosingStock = _res.OpeningStock + sumExpense;
        return _res;
    }

    public async Task<List<AccountBalanceItemModel>> GenerateReport(DateTime from, DateTime to, int year, string filterAccount = "", bool isNoiBo = false)
    {
        var filterBeginingTime = new DateTime(from.Year, 1, 1, 0, 0, 0);
        //
        var filterFrom = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0);
        var filterTo = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);
        var previousBalanceSheet = await CalculateBalanceSheet(filterBeginingTime, filterFrom.AddSeconds(-1), isNoiBo, year, filterAccount);

        // Nếu bắt đầu từ ngày 1/1
        if (from.Month == 1 && from.Day == 1)
        {
            //return previousBalanceSheet.Select(x => new AccountBalanceItemModel
            //{
            //    AccountCode = x.Code,
            //    AccountName = x.Name,
            //    AccountType = x.Type,
            //    OpeningDebit = x.IsForeignCurrency ? x.OpeningForeignDebit : x.OpeningDebit,
            //    OpeningCredit = x.IsForeignCurrency ? x.OpeningForeignCredit : x.OpeningCredit,
            //    ArisingDebit = x.IsForeignCurrency ? x.ArisingForeignDebit : x.ArisingDebit,
            //    ArisingCredit = x.IsForeignCurrency ? x.ArisingForeignCredit : x.ArisingCredit,
            //    ClosingDebit = x.IsForeignCurrency ? x.ClosingForeignDebit : x.ClosingDebit,
            //    ClosingCredit = x.IsForeignCurrency ? x.ClosingForeignCredit : x.ClosingCredit,
            //    CumulativeCredit = x.IsForeignCurrency ? x.CumulativeArisingForeignCredit : x.CumulativeArisingCredit,
            //    CumulativeDebit = x.IsForeignCurrency ? x.CumulativeArisingForeignDebit : x.CumulativeArisingDebit
            //}).ToList();
        }
        else
        {
            // Lấy cuối kỳ trước gán vào đầu kỳ sau
            previousBalanceSheet.ForEach(item =>
            {
                item.OpeningCredit = item.ClosingCredit;
                item.OpeningDebit = item.ClosingDebit;
                item.OpeningForeignCredit = item.ClosingForeignCredit;
                item.OpeningForeignDebit = item.ClosingForeignDebit;
                // nội bộ
                item.OpeningCreditNB = item.ClosingCredit;
                item.OpeningDebitNB = item.ClosingDebit;
                item.OpeningForeignCreditNB = item.ClosingForeignCredit;
                item.OpeningForeignDebitNB = item.ClosingForeignDebit;

                // Cuối kỳ = 0;
                item.ClosingCredit = 0;
                item.ClosingDebit = 0;
                item.ClosingForeignCredit = 0;
                item.ClosingForeignDebit = 0;
                // Luỹ kế bằng với phát sinh kì trước
                item.CumulativeArisingCredit = item.ArisingCredit;
                item.CumulativeArisingDebit = item.ArisingDebit;
                item.CumulativeArisingForeignCredit = item.ArisingForeignCredit;
                item.CumulativeArisingForeignDebit = item.ArisingForeignDebit;
                // Phát sinh = 0;
                item.ArisingCredit = 0;
                item.ArisingDebit = 0;
                item.ArisingForeignCredit = 0;
                item.ArisingForeignDebit = 0;

            });
        }
            // Lấy theo ngày ghi sổ
            var arisingLedgers = await _context.GetLedger(year, isNoiBo ? 3 : 2)
            .Where(x => x.OrginalBookDate.Value >= filterFrom && x.OrginalBookDate <= filterTo)
            .ToListAsync();

            // Lấy phát sinh có từ bảng phát sinh
            var araisingCredits = arisingLedgers
            .Select(x => new
            {
                x.CreditCode,
                CreditDetailCodeFirst = x.CreditDetailCodeFirst ?? string.Empty,
                CreditDetailCodeSecond = x.CreditDetailCodeSecond ?? string.Empty,
                CreditWarehouse = x.CreditWarehouse ?? string.Empty,
                Amount = x.Amount // Tổng tiền
            })
            .GroupBy(x => new
            {
                x.CreditCode,
                x.CreditDetailCodeFirst,
                x.CreditDetailCodeSecond,
                x.CreditWarehouse
            })
            .Select(x => new
            {
                x.First().CreditCode,
                x.First().CreditDetailCodeFirst,
                x.First().CreditDetailCodeSecond,
                x.First().CreditWarehouse,
                Amount = x.Sum(y => y.Amount)
            })
            .Select(x => new
            {
                CreditCode = x.CreditCode,
                CreditDetailCodeFirst = x.CreditDetailCodeFirst,
                CreditDetailCodeSecond = x.CreditDetailCodeSecond,
                Amount = x.Amount,
                CreditWarehouse = x.CreditWarehouse,
                Hash = string.Join("/", new[] {
                        x.CreditCode,
                        x.CreditDetailCodeFirst,
                        x.CreditDetailCodeSecond,
                        //x.CreditWarehouse
                }.Where(s => !string.IsNullOrEmpty(s)))
            })
            .OrderBy(x => x.CreditCode)
            .ToList();

            // Tổng hợp có từ bảng Phát sinh vào bảng tài khoản
            previousBalanceSheet.ForEach(x =>
            {
                araisingCredits.ForEach(y =>
                {
                    if (x.Hash == y.Hash && x.WarehouseCode == y.CreditWarehouse)
                    {
                        if (!x.IsForeignCurrency)
                        {
                            x.ArisingCredit = y.Amount;
                            x.CumulativeArisingCredit = x.CumulativeArisingCredit + y.Amount;
                        }
                        else
                        {
                            x.ArisingForeignCredit = y.Amount;
                            x.CumulativeArisingForeignCredit = x.CumulativeArisingForeignCredit + y.Amount;
                        }

                    }
                });
            });


            // Danh sách phát sinh nợ
            var araisingDebits = arisingLedgers
            .Select(x => new
            {
                x.DebitCode,
                DebitDetailCodeFirst = x.DebitDetailCodeFirst ?? string.Empty,
                DebitDetailCodeSecond = x.DebitDetailCodeSecond ?? string.Empty,
                DebitWarehouse = x.DebitWarehouse ?? string.Empty,
                Amount = x.Amount
            })
            .GroupBy(x => new
            {
                x.DebitCode,
                x.DebitDetailCodeFirst,
                x.DebitDetailCodeSecond,
                x.DebitWarehouse
            }).Select(x => new
            {
                x.First().DebitCode,
                x.First().DebitDetailCodeFirst,
                x.First().DebitDetailCodeSecond,
                x.First().DebitWarehouse,
                Amount = x.Sum(y => y.Amount)
            })
            .Select(x => new
            {
                DebitCode = x.DebitCode,
                DebitDetailCodeFirst = x.DebitDetailCodeFirst,
                DebitDetailCodeSecond = x.DebitDetailCodeSecond,
                Amount = x.Amount,
                DebitWarehouse = x.DebitWarehouse,
                Hash = string.Join("/", new[] {
                    x.DebitCode,
                    x.DebitDetailCodeFirst,
                    x.DebitDetailCodeSecond,
                    //x.DebitWarehouse
                }.Where(s => !string.IsNullOrEmpty(s)))
            })
            .OrderBy(x => x.DebitCode)
            .ToList();

            // Tổng hợp Nợ vào bảng tài khoản
            previousBalanceSheet.ForEach(x =>
            {
                araisingDebits.ForEach(y =>
                {
                    if (x.Hash == y.Hash && x.WarehouseCode == y.DebitWarehouse)
                    {
                        if (!x.IsForeignCurrency)
                        {
                            x.ArisingDebit = y.Amount;
                            x.CumulativeArisingDebit = x.CumulativeArisingDebit + y.Amount;
                        }
                        else
                        {
                            x.ArisingForeignDebit = y.Amount;
                            x.CumulativeArisingForeignDebit = x.CumulativeArisingForeignDebit + y.Amount;
                        }

                    }
                });
            });

            // Tính ngược lên tài khoản cha
            double closing = 0;
            double closingForeign = 0;
            // Xoá trống giá trị của parent -> để tính lại
            previousBalanceSheet.ForEach(x =>
            {
                var currentRef = x.Ref;
                while (currentRef != null)
                {
                    currentRef.HasChild = true;
                    //currentRef.OpeningCredit = 0;
                    //currentRef.OpeningDebit = 0;
                    //currentRef.OpeningForeignCredit = 0;
                    //currentRef.OpeningForeignDebit = 0;
                    currentRef.ArisingCredit = 0;
                    currentRef.ArisingDebit = 0;
                    currentRef.ArisingForeignCredit = 0;
                    currentRef.ArisingForeignDebit = 0;
                    currentRef.CumulativeArisingCredit = 0;
                    currentRef.CumulativeArisingDebit = 0;
                    currentRef.CumulativeArisingForeignCredit = 0;
                    currentRef.CumulativeArisingForeignDebit = 0;
                    currentRef.ClosingCredit = 0;
                    currentRef.ClosingDebit = 0;
                    currentRef.ClosingForeignCredit = 0;
                    currentRef.ClosingForeignDebit = 0;
                    currentRef = currentRef.Ref;
                }
            });

            previousBalanceSheet.ForEach(x =>
            {
                if (isNoiBo)
                {
                    closing = (x.OpeningCreditNB + x.ArisingCredit) - (x.OpeningDebitNB + x.ArisingDebit);
                    closingForeign = (x.OpeningForeignCreditNB + x.ArisingForeignCredit) - (x.OpeningForeignDebitNB + x.ArisingForeignDebit);
                }
                else
                {
                    closing = (x.OpeningCredit + x.ArisingCredit) - (x.OpeningDebit + x.ArisingDebit);
                    closingForeign = (x.OpeningForeignCredit + x.ArisingForeignCredit) - (x.OpeningForeignDebit + x.ArisingForeignDebit);
                }
                x.ClosingCredit = 0;
                x.ClosingDebit = 0;
                if (closing >= 0)
                {
                    x.ClosingCredit = closing;
                }
                else
                {
                    x.ClosingDebit = Math.Abs(closing);
                }

                x.ClosingForeignCredit = 0;
                x.ClosingForeignDebit = 0;
                if (closingForeign >= 0)
                {
                    x.ClosingForeignCredit = closingForeign;
                }
                else
                {
                    x.ClosingForeignDebit = Math.Abs(closingForeign);
                }

                var currentRef = x.Ref;

                while (currentRef != null)
                {
                    currentRef.ArisingDebit = currentRef.ArisingDebit + x.ArisingDebit;
                    currentRef.ArisingCredit = currentRef.ArisingCredit + x.ArisingCredit;
                    currentRef.ArisingForeignDebit = currentRef.ArisingForeignDebit + x.ArisingForeignDebit;
                    currentRef.ArisingForeignCredit = currentRef.ArisingForeignCredit + x.ArisingForeignCredit;
                    currentRef.CumulativeArisingCredit = currentRef.CumulativeArisingCredit + x.CumulativeArisingCredit;
                    currentRef.CumulativeArisingDebit = currentRef.CumulativeArisingDebit + x.CumulativeArisingDebit;
                    currentRef.CumulativeArisingForeignCredit = currentRef.CumulativeArisingForeignCredit + x.CumulativeArisingForeignCredit;
                    currentRef.CumulativeArisingForeignDebit = currentRef.CumulativeArisingForeignDebit + x.CumulativeArisingForeignDebit;

                    if (isNoiBo)
                    {
                        closing = (currentRef.OpeningCreditNB + currentRef.ArisingCredit) - (currentRef.OpeningDebitNB + currentRef.ArisingDebit);
                        closingForeign = (currentRef.OpeningForeignCreditNB + currentRef.ArisingForeignCredit) - (currentRef.OpeningForeignDebitNB + currentRef.ArisingForeignDebit);
                    }
                    else
                    {
                        closing = (currentRef.OpeningCredit + currentRef.ArisingCredit) - (currentRef.OpeningDebit + currentRef.ArisingDebit);
                        closingForeign = (currentRef.OpeningForeignCredit + currentRef.ArisingForeignCredit) - (currentRef.OpeningForeignDebit + currentRef.ArisingForeignDebit);
                    }
                    currentRef.ClosingCredit = 0;
                    currentRef.ClosingDebit = 0;
                    if (closing >= 0)
                    {
                        currentRef.ClosingCredit = closing;
                    }
                    else
                    {
                        currentRef.ClosingDebit = Math.Abs(closing);
                    }

                    currentRef.ClosingForeignCredit = 0;
                    currentRef.ClosingForeignDebit = 0;
                    if (closingForeign >= 0)
                    {
                        currentRef.ClosingForeignCredit = closingForeign;
                    }
                    else
                    {
                        currentRef.ClosingForeignDebit = Math.Abs(closingForeign);
                    }

                    currentRef = currentRef.Ref;
                }
            });
        
        var ret = previousBalanceSheet.Select(x => new AccountBalanceItemModel
        {
            IsForeign = x.IsForeignCurrency,
            AccountCode = x.Code,
            AccountName = x.Name,
            AccountType = x.Type,
            OpeningDebit = x.IsForeignCurrency ? x.OpeningForeignDebit : x.OpeningDebit,
            OpeningCredit = x.IsForeignCurrency ? x.OpeningForeignCredit : x.OpeningCredit,
            ArisingDebit = x.IsForeignCurrency ? x.ArisingForeignDebit : x.ArisingDebit,
            ArisingCredit = x.IsForeignCurrency ? x.ArisingForeignCredit : x.ArisingCredit,
            ClosingDebit = x.IsForeignCurrency ? x.ClosingForeignDebit : x.ClosingDebit,
            ClosingCredit = x.IsForeignCurrency ? x.ClosingForeignCredit : x.ClosingCredit,
            CumulativeCredit = x.IsForeignCurrency ? x.CumulativeArisingForeignCredit : x.CumulativeArisingCredit,
            CumulativeDebit = x.IsForeignCurrency ? x.CumulativeArisingForeignDebit : x.CumulativeArisingDebit,
            Hash = x.Hash,
            ParentRef = x.ParentRef,
            Duration = x.Duration,
            HasChild = x.HasChild,
            // nội bộ
            OpeningDebitNB = x.IsForeignCurrency ? x.OpeningForeignDebitNB : x.OpeningDebitNB,
            OpeningCreditNB = x.IsForeignCurrency ? x.OpeningForeignCreditNB : x.OpeningCreditNB,
        }).ToList();

        int accountType = 1;
        if (ret.Count > 0)
            accountType = ret.Min(x => x.AccountType);
        var accountsLevel1 = ret.Where(x => x.AccountType == accountType)// && x.AccountCode.Length == 3
            .ToList();

        ret = ret.Append(new AccountBalanceItemModel()
        {
            IsForeign = false,
            AccountCode = "-1",
            AccountName = "Tổng",
            AccountType = 1,
            OpeningDebit = accountsLevel1.Sum(x => x.OpeningDebit),
            OpeningCredit = accountsLevel1.Sum(x => x.OpeningCredit),
            ArisingDebit = accountsLevel1.Sum(x => x.ArisingDebit),
            ArisingCredit = accountsLevel1.Sum(x => x.ArisingCredit),
            ClosingDebit = accountsLevel1.Sum(x => x.ClosingDebit),
            ClosingCredit = accountsLevel1.Sum(x => x.ClosingCredit),
            CumulativeDebit = accountsLevel1.Sum(x => x.CumulativeDebit),
            CumulativeCredit = accountsLevel1.Sum(x => x.CumulativeCredit),
            Hash = "-1",
            // nội bộ
            OpeningDebitNB = accountsLevel1.Sum(x => x.OpeningDebitNB),
            OpeningCreditNB = accountsLevel1.Sum(x => x.OpeningCreditNB),
        }).ToList();

        return ret;
    }

    private AccountByPeriodDetail GetAccountInfoFromTheBeginingTime(DateTime end, string accountCode, string detailOne, string detailTwo, int year)
    {
        var begining = new DateTime(end.Year, 1, 1, 0, 0, 0);
        // debit
        var arisingDebitQuery = _context.GetLedger(year, 2)
        .Where(x => x.OrginalBookDate.Value >= begining && x.OrginalBookDate.Value <= end && x.DebitCode.StartsWith(accountCode));

        if (!string.IsNullOrWhiteSpace(detailOne))
        {
            arisingDebitQuery = arisingDebitQuery
            .Where(x => x.DebitDetailCodeFirst == detailOne);
        }

        if (!string.IsNullOrWhiteSpace(detailTwo))
        {
            arisingDebitQuery = arisingDebitQuery
            .Where(x => x.DebitDetailCodeSecond == detailTwo);
        }

        var arisingDebit = arisingDebitQuery
            .Sum(x => (double?)x.Amount) ?? 0;

        // credit
        var arisingCreditQuery = _context.GetLedger(year, 2)
        .Where(x => x.OrginalBookDate.Value >= begining && x.OrginalBookDate.Value <= end && x.CreditCode.StartsWith(accountCode));


        if (!string.IsNullOrWhiteSpace(detailOne))
        {
            arisingCreditQuery = arisingCreditQuery
            .Where(x => x.CreditDetailCodeFirst == detailOne);
        }

        if (!string.IsNullOrWhiteSpace(detailTwo))
        {
            arisingCreditQuery = arisingCreditQuery
            .Where(x => x.CreditDetailCodeSecond == detailTwo);
        };

        var arisingCredit = arisingCreditQuery
            .Sum(x => (double?)x.Amount) ?? 0;

        // Lay tai khoan
        var accountQuery = _context.GetChartOfAccount(year).AsQueryable();

        if (!string.IsNullOrWhiteSpace(detailTwo))
        {
            string parentRef = string.Format("{0}:{1}", accountCode, detailOne);

            accountQuery = accountQuery
                .Where(x => x.Code == detailTwo && x.Type == 6 && x.ParentRef == parentRef);
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(detailOne))
            {
                accountQuery = accountQuery
                .Where(x => x.Code == detailOne && x.Type == 5 && x.ParentRef == accountCode);
            }
            else
            {
                accountQuery = accountQuery
                .Where(x => x.Code == accountCode);
            }
        }

        var result = accountQuery
        .Select(x => new AccountByPeriodDetail
        {
            OpeningDebit = x.OpeningDebit ?? 0,
            OpeningCredit = x.OpeningCredit ?? 0,
            ArisingDebit = arisingDebit,
            ArisingCredit = arisingCredit,
            OpenAccumulatingDebit = 0,
            OpenAccumulatingCredit = 0,
            AcumulatingDebit = arisingDebit,
            AcumulatingCredit = arisingCredit,
            ClosingDebit = 0,
            ClosingCredit = 0
        }).FirstOrDefault();


        var closing = (result.OpeningDebit + result.ArisingDebit) - (result.OpeningCredit + result.ArisingCredit);

        if (closing > 0)
            result.ClosingDebit = closing;
        else
            result.ClosingCredit = Math.Abs(closing);
        return result;
    }

    public AccountByPeriod GetAccountInfoByPeriod(DateTime from, DateTime to, string accountCode, int year, string detailOne = "", string detailTwo = "")
    {
        from = new DateTime(from.Year, from.Month, from.Day, 0, 0, 0);
        to = new DateTime(to.Year, to.Month, to.Day, 23, 59, 59);

        var previous = GetAccountInfoFromTheBeginingTime(from.Date.AddSeconds(-1), accountCode, detailOne, detailTwo, year);

        // debit
        var arisingDebitQuery = _context.GetLedger(year, 2)
        .Where(x => x.OrginalBookDate.Value >= from && x.OrginalBookDate.Value <= to && x.DebitCode.StartsWith(accountCode));

        if (!string.IsNullOrWhiteSpace(detailOne))
        {
            arisingDebitQuery = arisingDebitQuery
            .Where(x => x.DebitDetailCodeFirst == detailOne);
        }

        if (!string.IsNullOrWhiteSpace(detailTwo))
        {
            arisingDebitQuery = arisingDebitQuery
            .Where(x => x.DebitDetailCodeSecond == detailTwo);
        };

        var arisingDebit = arisingDebitQuery
            .ToList()
            .GroupBy(x => x.OrginalBookDate.Value.Month)
            .Select(x => new MonthSummary {
                Month = x.Key,
                LedgerDetails = x.Select(c => new LedgerDetail {
                    RecordDate = c.BookDate.Value,
                    VoucherNumber = c.VoucherNumber,
                    VoucherDate = c.OrginalBookDate.Value,
                    Description = c.OrginalDescription,
                    CorrespondingAccount = c.CreditCode,
                    CorrespondingDetail = c.CreditDetailCodeFirst,
                    DebitArisingAmount = c.Amount,
                    DebitBalance = c.Amount
                }),
                ArisingAmount = x.Sum(x => (double?)x.Amount) ?? 0
            }).ToDictionary(x => x.Month);


        // credit
        var arisingCreditQuery = _context.GetLedger(year, 2)
        .Where(x => x.OrginalBookDate.Value >= from && x.OrginalBookDate.Value <= to && x.CreditCode.StartsWith(accountCode));


        if (!string.IsNullOrWhiteSpace(detailOne))
        {
            arisingCreditQuery = arisingCreditQuery
            .Where(x => x.CreditDetailCodeFirst == detailOne);
        }

        if (!string.IsNullOrWhiteSpace(detailTwo))
        {
            arisingCreditQuery = arisingCreditQuery
            .Where(x => x.CreditDetailCodeSecond == detailTwo);
        };

        var arisingCredit = arisingCreditQuery
            .ToList()
            .GroupBy(x => x.OrginalBookDate.Value.Month)
            .Select(x => new MonthSummary {
                Month = x.Key,
                LedgerDetails = x.Select(c => new LedgerDetail {
                    RecordDate = c.BookDate.Value,
                    VoucherNumber = c.VoucherNumber,
                    VoucherDate = c.OrginalBookDate.Value,
                    Description = c.OrginalDescription,
                    CorrespondingAccount = c.DebitCode,
                    CorrespondingDetail = c.DebitDetailCodeFirst,
                    CreditArisingAmount = c.Amount,
                    CreditBalance = c.Amount
                }),
                ArisingAmount = x.Sum(x => (double?)x.Amount) ?? 0
            }).ToDictionary(x => x.Month);

        AccountByPeriod result = new AccountByPeriod()
        {
            StartDate = from,
            EndDate = to,
            AccountCode = accountCode,
            DetailOneCode = detailOne,
            DetailTwoCode = detailTwo
        };

        for (int i = from.Month; i <= to.Month; i++)
        {
            if (!arisingDebit.ContainsKey(i))
            {
                arisingDebit.Add(i, new MonthSummary
                {
                    Month = i,
                    ArisingAmount = 0
                });
            }

            if (!arisingCredit.ContainsKey(i))
            {
                arisingCredit.Add(i, new MonthSummary
                {
                    Month = i,
                    ArisingAmount = 0
                });
            }
        }

        int idx = 0;
        for (int i = from.Month; i <= to.Month; i++)
        {
            var r0 = from.AddMonths(idx);
            var r1 = new DateTime(r0.Year, r0.Month , DateTime.DaysInMonth(r0.Year, r0.Month),  23, 59, 59);
            var current = new AccountByPeriodDetail()
            {
                From = r0,
                To = r1,
                Month = r0.Month,
                OpeningDebit = previous.ClosingDebit,
                OpeningCredit = previous.ClosingCredit,
                ArisingDebit = arisingDebit[i].ArisingAmount,
                ArisingCredit = arisingCredit[i].ArisingAmount,
                OpenAccumulatingDebit = previous.AcumulatingDebit,
                OpenAccumulatingCredit = previous.AcumulatingCredit,
                AcumulatingDebit = previous.AcumulatingDebit + arisingDebit[i].ArisingAmount,
                AcumulatingCredit = previous.AcumulatingCredit + arisingCredit[i].ArisingAmount
            };

            current.LedgerDetails.AddRange(arisingDebit[i].LedgerDetails);
            current.LedgerDetails.AddRange(arisingCredit[i].LedgerDetails);
            current.LedgerDetails =  current.LedgerDetails.OrderBy(x => x.VoucherDate)
                .ToList();

            double oDebit = current.OpeningDebit;
            double oCredit = current.OpeningCredit;

            for (int j = 0; j < current.LedgerDetails.Count; j++)
            {
                current.LedgerDetails[j].DebitBalance = oDebit + current.LedgerDetails[j].DebitArisingAmount;
                current.LedgerDetails[j].CreditBalance = oCredit + current.LedgerDetails[j].CreditArisingAmount;
                current.LedgerDetails[j].TotalBalance = current.LedgerDetails[j].DebitBalance  - current.LedgerDetails[j].CreditBalance;
                oDebit = current.LedgerDetails[j].DebitBalance;
                oCredit = current.LedgerDetails[j].CreditBalance;
            }

            var closing = (current.OpeningDebit + current.ArisingDebit) - (current.OpeningCredit + current.ArisingCredit);

            if (closing > 0)
                current.ClosingDebit = closing;
            else
                current.ClosingCredit = Math.Abs(closing);

            previous = current;
            idx++;
            result.accountByPeriodDetails.Add(current);
        }

        return result;
    }
}
