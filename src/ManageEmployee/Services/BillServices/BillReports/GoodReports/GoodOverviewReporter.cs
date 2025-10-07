using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.BillReportModels.GoodReports;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Services.Interfaces.Accounts;
using ManageEmployee.Services.Interfaces.Bills.BillReports.BusinessOverviews;
using ManageEmployee.Services.Interfaces.Reports;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.BillServices.BillReports.GoodReports
{
    public class GoodOverviewReporter: IGoodOverviewReporter
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountBalanceSheetService _accountBalanceSheetService;
        private readonly ISavedMovedMoneyReportService _savedMovedMoneyReportService;

        public GoodOverviewReporter(ApplicationDbContext context,
            IAccountBalanceSheetService accountBalanceSheetService, 
            ISavedMovedMoneyReportService savedMovedMoneyReportService)
        {
            _context = context;
            _accountBalanceSheetService = accountBalanceSheetService;
            _savedMovedMoneyReportService = savedMovedMoneyReportService;
        }
        public async Task<GoodOverviewReporterModel> ReportAsync(BillReportBranchRequestModel param, int year)
        {
            var billQuery = _context.Bills.Where(x => x.CreatedDate >= param.FromAt && x.CreatedDate <= param.ToAt
                            && (param.BranchId == null || x.BranchId == param.BranchId));
            var billIds = await billQuery.Select(x => x.Id).ToListAsync();
            var billDetailQuery = _context.BillDetails.Where(x => billIds.Contains(x.Id));
            var itemOut = new GoodOverviewReporterModel();
            itemOut.Overview = await OverviewAsync(param, billDetailQuery, year);
            itemOut.GroupGoods = await GroupGoodAsync(param, billIds);
            itemOut.Goods = await GoodAsync(param, billIds);
            return itemOut;
        }
        private async Task<GoodOverviewReporterOverviewModel> OverviewAsync(BillReportBranchRequestModel param, IQueryable<BillDetail> billDetailQuery, int year)
        {
            List<AccountBalanceItemModel> _lstAccount = await _accountBalanceSheetService
                       .GenerateReport(param.FromAt, param.ToAt, year, isNoiBo: true);
            List<SaveMovedModelBase> _lstModelBaseReport = _savedMovedMoneyReportService.GetModelOnjectReport();
            var saveMovedCodes = new List<string>() { "01", "02" };
            _lstModelBaseReport = _lstModelBaseReport.Where(x => saveMovedCodes.Contains(x.code)).ToList();

            var itemOut = new GoodOverviewReporterOverviewModel();
            itemOut.TotalGood = await billDetailQuery.Select(x => x.GoodsId).Distinct().CountAsync();
            itemOut.TotalQuantity = await billDetailQuery.SumAsync(x => x.Quantity);
            double totalCost = 0;
            foreach (SaveMovedModelBase _modeBase in _lstModelBaseReport)
            {
                _savedMovedMoneyReportService.Calculator_Report(_modeBase, _lstAccount, true);
                if (_modeBase.code == "01")
                {
                    itemOut.SaleAmount = _modeBase.this_year;

                }
                else if (_modeBase.code == "02")
                {
                    totalCost = _modeBase.this_year;
                }
            }

            itemOut.GrossProfit = itemOut.SaleAmount - totalCost;
            return itemOut;
        }

        private async Task<IEnumerable<GoodOverviewReporterDetailModel>> GroupGoodAsync(BillReportBranchRequestModel param, List<int> billIds)
        {
            var data = await _context.BillDetails
                .Where(x => x.CreatedDate >= param.FromAt && x.CreatedDate <= param.ToAt && billIds.Contains(x.BillId))
                .Join(_context.Goods,
                b => b.GoodsId,
                g => g.Id,
                (b, g) => new
                {
                    bill = b,
                    good = g,
                })
                .GroupBy(x => x.good.Detail1)
                .Select(x => new 
                {
                    Code = x.Key,
                    Name = x.FirstOrDefault().good.DetailName1,
                    TotalQuantity = x.Select(x => x.bill.Quantity).Distinct().Count(),
                    GoodIds = x.Select(x => x.good.Id).Distinct().ToList(),
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(10)
                .ToListAsync();

            var listOut = new List<GoodOverviewReporterDetailModel>();
            foreach (var item in data)
            {
                var itemOut = new GoodOverviewReporterDetailModel
                {
                    Code = item.Code,
                    Name = item.Name,
                    TotalQuantity = item.TotalQuantity,
                };
                itemOut.TotalRefund = await _context.BillDetailRefunds.Where(x => billIds.Contains(x.BillId ?? 0) && item.GoodIds.Contains(x.GoodsId ?? 0)).SumAsync(x => x.Quantity);
                listOut.Add(itemOut);
            }

            return listOut;
        }
        private async Task<IEnumerable<GoodOverviewReporterDetailModel>> GoodAsync(BillReportBranchRequestModel param, List<int> billIds)
        {
            var data = await _context.BillDetails
                .Where(x => x.CreatedDate >= param.FromAt && x.CreatedDate <= param.ToAt && billIds.Contains(x.BillId))
                .Join(_context.Goods,
                b => b.GoodsId,
                g => g.Id,
                (b, g) => new
                {
                    bill = b,
                    good = g,
                })
                .GroupBy(x => x.good.Detail2)
                .Select(x => new
                {
                    Code = x.Key,
                    Name = x.FirstOrDefault().good.DetailName2,
                    TotalQuantity = x.Select(x => x.bill.Quantity).Distinct().Count(),
                    BillIds = x.Select(x => x.bill.BillId).Distinct().ToList(),
                    GoodIds = x.Select(x => x.good.Id).Distinct().ToList(),
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(10)
                .ToListAsync();


            var listOut = new List<GoodOverviewReporterDetailModel>();
            foreach (var item in data)
            {
                var itemOut = new GoodOverviewReporterDetailModel
                {
                    Code = item.Code,
                    Name = item.Name,
                    TotalQuantity = item.TotalQuantity,
                };
                itemOut.TotalRefund = await _context.BillDetailRefunds.Where(x =>  billIds.Contains(x.BillId ?? 0) && item.GoodIds.Contains(x.GoodsId ?? 0)).SumAsync(x => x.Quantity);
                listOut.Add(itemOut);
            }

            return listOut;
        }

    }
}


