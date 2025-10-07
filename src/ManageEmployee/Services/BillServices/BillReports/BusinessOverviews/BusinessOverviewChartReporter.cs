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
    public class BusinessOverviewChartReporter : IBusinessOverviewChartReporter
    {
        private readonly ApplicationDbContext _context;
        private readonly IAccountBalanceSheetService _accountBalanceSheetService;
        private readonly ISavedMovedMoneyReportService _savedMovedMoneyReportService;
        public BusinessOverviewChartReporter(ApplicationDbContext context,
            IAccountBalanceSheetService accountBalanceSheetService,
            ISavedMovedMoneyReportService savedMovedMoneyReportService)
        {
            _context = context;
            _accountBalanceSheetService = accountBalanceSheetService;
            _savedMovedMoneyReportService = savedMovedMoneyReportService;
        }

        public async Task<IEnumerable<BusinessOverviewReportModel>> ReportAsync(BillReportBranchRequestModel param, int year)
        {
            if (param.FromAt > param.ToAt)
            {
                throw new ErrorException(ErrorMessages.IncorrectFormat);
            }
            decimal day = (param.ToAt - param.FromAt).Days + 1;
            int dayAdd = (int)Math.Round(day / 5);
            int i = 0;
            List<SaveMovedModelBase> _lstModelBaseReport = _savedMovedMoneyReportService.GetModelOnjectReport();
            var saveMovedCodes = new List<string>() { "01", "02" };
            _lstModelBaseReport = _lstModelBaseReport.Where(x => saveMovedCodes.Contains(x.code)).ToList();

            var listOut = new List<BusinessOverviewReportModel>();
            while (true)
            {
                DateTime dateCheck = param.FromAt.AddDays(dayAdd * i);
                if (dateCheck > param.ToAt)
                {
                    break;
                }
                List<AccountBalanceItemModel> _lstAccount = await _accountBalanceSheetService
                           .GenerateReport(dateCheck, dateCheck, year, isNoiBo: true);
                

                var billQuery = _context.Bills.Where(x => x.CreatedDate >= dateCheck && x.CreatedDate <= dateCheck
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
                
                i++;
                listOut.Add(itemOut);
            }

            return listOut;
        }
    }
}