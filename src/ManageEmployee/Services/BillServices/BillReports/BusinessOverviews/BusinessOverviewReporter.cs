using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.BillReportModels.BusinessOverviews;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Accounts;
using ManageEmployee.Services.Interfaces.Bills.BillReports.BusinessOverviews;
using ManageEmployee.Services.Interfaces.Reports;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.BillServices.BillReports.BusinessOverviews
{
    public class BusinessOverviewReporter : IBusinessOverviewReporter
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountBalanceSheetService _accountBalanceSheetService;
        private readonly ISavedMovedMoneyReportService _savedMovedMoneyReportService;
        public BusinessOverviewReporter(ApplicationDbContext context,
            IAccountBalanceSheetService accountBalanceSheetService,
            ISavedMovedMoneyReportService savedMovedMoneyReportService)
        {
            _context = context;
            _accountBalanceSheetService = accountBalanceSheetService;
            _savedMovedMoneyReportService = savedMovedMoneyReportService;
        }

        public async Task<BusinessOverviewReportModel> ReportOverviewAsync(BillReportBranchRequestModel param, int year)
        {
            if (param.FromAt > param.ToAt)
            {
                throw new ErrorException(ErrorMessages.IncorrectFormat);
            }
            List<AccountBalanceItemModel> _lstAccount = await _accountBalanceSheetService
                       .GenerateReport(param.FromAt, param.ToAt, year, isNoiBo: true);
            List<SaveMovedModelBase> _lstModelBaseReport = _savedMovedMoneyReportService.GetModelOnjectReport();
            var saveMovedCodes = new List<string>() { "01", "02" };
            _lstModelBaseReport = _lstModelBaseReport.Where(x => saveMovedCodes.Contains(x.code)).ToList();
            var dayAmount = (param.ToAt - param.FromAt).Days + 1;

            var billQuery = _context.Bills.Where(x => x.CreatedDate >= param.FromAt && x.CreatedDate <= param.ToAt
                            && (param.BranchId == null || x.BranchId == param.BranchId));
            var billIds = await billQuery.Select(x => x.Id).ToListAsync();
            var itemOut = new BusinessOverviewReportModel();
            itemOut.TotalBill = await billQuery.CountAsync();
            itemOut.RevenueAmount = await billQuery.SumAsync(x => x.TotalAmount + x.DiscountPrice);
            itemOut.RefundAmount = await _context.BillDetailRefunds.Where(x => billIds.Contains(x.BillId ?? 0)).SumAsync(x => x.Quantity * x.UnitPrice);

            foreach (SaveMovedModelBase _modeBase in _lstModelBaseReport)
            {
                _savedMovedMoneyReportService.Calculator_Report(_modeBase, _lstAccount, true);
                if (_modeBase.code == "01")
                {
                    itemOut.SaleAmount = _modeBase.this_year;

                }
                else if (_modeBase.code == "02")
                {
                    itemOut.TotalCost = _modeBase.this_year;
                }
            }

            itemOut.GrossProfit = itemOut.SaleAmount - itemOut.TotalCost;
            //Average
            itemOut.TotalCostAverage = itemOut.TotalCost / dayAmount;
            itemOut.GrossProfitAverage = itemOut.GrossProfit / dayAmount;
            itemOut.RefundAmountAverage = itemOut.RefundAmount / dayAmount;
            itemOut.RevenueAmountAverage = itemOut.RevenueAmount / dayAmount;
            itemOut.SaleAmountAverage = itemOut.SaleAmount / dayAmount;
            itemOut.TotalBillAverage = itemOut.TotalBill / dayAmount;

            //PreviousPeriod
            param.ToAt = param.FromAt.AddDays(-1);
            param.FromAt = param.FromAt.AddDays(-dayAmount);
            _lstAccount = await _accountBalanceSheetService
                      .GenerateReport(param.FromAt, param.ToAt, year, isNoiBo: true);

            billQuery = _context.Bills.Where(x => x.CreatedDate >= param.FromAt && x.CreatedDate <= param.ToAt);
            billIds = await billQuery.Select(x => x.Id).ToListAsync();

            var totalBillPreviousPeriod = await billQuery.CountAsync();
            if (totalBillPreviousPeriod > 0)
            {
                itemOut.TotalBillPreviousPeriod = (itemOut.TotalBill - totalBillPreviousPeriod) / totalBillPreviousPeriod;
            }

            var revenueAmountPreviousPeriod = await billQuery.SumAsync(x => x.TotalAmount + x.DiscountPrice);
            if (revenueAmountPreviousPeriod > 0)
            {
                itemOut.RevenueAmountPreviousPeriod = (itemOut.RevenueAmount - revenueAmountPreviousPeriod) / revenueAmountPreviousPeriod;
            }

            var refundAmountPreviousPeriod = await _context.BillDetailRefunds.Where(x => billIds.Contains(x.BillId ?? 0)).SumAsync(x => x.Quantity * x.UnitPrice);
            if (refundAmountPreviousPeriod > 0)
            {
                itemOut.RefundAmountPreviousPeriod = (itemOut.RefundAmount - refundAmountPreviousPeriod) / refundAmountPreviousPeriod;
            }

            double saleAmountPreviousPeriod = 0;
            double totalCostPreviousPeriod = 0;
            foreach (SaveMovedModelBase _modeBase in _lstModelBaseReport)
            {
                _savedMovedMoneyReportService.Calculator_Report(_modeBase, _lstAccount, true);
                if (_modeBase.code == "01" && _modeBase.this_year > 0)
                {
                    saleAmountPreviousPeriod = _modeBase.this_year;
                    itemOut.SaleAmountPreviousPeriod = (itemOut.SaleAmount - itemOut.SaleAmountPreviousPeriod) / itemOut.SaleAmountPreviousPeriod;
                }
                else if (_modeBase.code == "02" && _modeBase.this_year > 0)
                {
                    totalCostPreviousPeriod = _modeBase.this_year;
                    itemOut.TotalCostPreviousPeriod = (itemOut.TotalCost - totalCostPreviousPeriod) / totalCostPreviousPeriod;
                }
            }
            var GrossProfitPreviousPeriod = saleAmountPreviousPeriod - totalCostPreviousPeriod;
            if (GrossProfitPreviousPeriod > 0)
            {
                itemOut.GrossProfitPreviousPeriod = (itemOut.GrossProfit - GrossProfitPreviousPeriod) / GrossProfitPreviousPeriod;
            }

            return itemOut;
        }
        public async Task<IEnumerable<BusinessReportForBranchModel>> ReportForBranchAsync(BillReportTimeRequestModel param, int year)
        {
            var listOut = new List<BusinessReportForBranchModel>();
            var branchs = await _context.Branchs.Where(x => !x.IsDelete).ToListAsync();
            List<AccountBalanceItemModel> _lstAccount = await _accountBalanceSheetService
                          .GenerateReport(param.FromAt, param.ToAt, year, isNoiBo: true);
            List<SaveMovedModelBase> _lstModelBaseReport = _savedMovedMoneyReportService.GetModelOnjectReport();
            var saveMovedCodes = new List<string>() { "01", "02" };
            _lstModelBaseReport = _lstModelBaseReport.Where(x => saveMovedCodes.Contains(x.code)).ToList();

            foreach (var branch in branchs)
            {
                var billQuery = _context.Bills.Where(x => x.CreatedDate >= param.FromAt && x.CreatedDate <= param.ToAt && x.BranchId == branch.Id);
                var billIds = await billQuery.Select(x => x.Id).ToListAsync();
                var itemOut = new BusinessReportForBranchModel
                {
                    BranchId = branch.Id,
                    BranchName = branch.Name,
                };

                itemOut.RevenueAmount = await billQuery.SumAsync(x => x.TotalAmount + x.DiscountPrice);
                itemOut.RefundAmount = await _context.BillDetailRefunds.Where(x => billIds.Contains(x.BillId ?? 0)).SumAsync(x => x.Quantity * x.UnitPrice);

                foreach (SaveMovedModelBase _modeBase in _lstModelBaseReport)
                {
                    _savedMovedMoneyReportService.Calculator_Report(_modeBase, _lstAccount, true);
                    if (_modeBase.code == "01")
                    {
                        itemOut.SaleAmount = _modeBase.this_year;

                    }
                    else if (_modeBase.code == "02")
                    {
                        itemOut.TotalCost = _modeBase.this_year;
                    }
                }

                itemOut.GrossProfit = itemOut.SaleAmount - itemOut.TotalCost;
                listOut.Add(itemOut);
            }
            return listOut;
        }
    }
}