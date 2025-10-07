using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.BillReportModels.OperatingCosts;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Services.Interfaces.Bills.BillReports.OperatingCosts;
using ManageEmployee.Services.Interfaces.Ledgers.V2;

namespace ManageEmployee.Services.BillServices.BillReports.OperatingCosts
{
    public class OperatingCostOverviewReporter : IOperatingCostOverviewReporter
    {
        private readonly IAccountBalanceSheetV2Service _accountBalanceSheetV2Service;

        public OperatingCostOverviewReporter(IAccountBalanceSheetV2Service accountBalanceSheetV2Service)
        {
            _accountBalanceSheetV2Service = accountBalanceSheetV2Service;
        }

        public async Task<OperatingCostOverviewReportModel> ReportAsync(BillReportTimeRequestModel param, int year)
        {
            var paramReport = new AccountBalanceReportParam
            {
                FromDate = param.FromAt,
                ToDate = param.ToAt,
                IsNoiBo = true,
                StartAccount = "6",
                PrintType = new List<int>
                            {
                                (int)AccountBalanceReportTypeEnum.khongInNhungDongKhongSoLieu,
                                (int)AccountBalanceReportTypeEnum.inCaChiTietCap1,
                                (int)AccountBalanceReportTypeEnum.inCaChiTietCap2,
                            }
            };
            var (accounts, itemTotal) = await _accountBalanceSheetV2Service.InitDataAsync(paramReport, year);
            var accountParents = accounts.Where(x => x.AccountType < 5).Select(x => x.Code);
            return new OperatingCostOverviewReportModel
            {
                Details = accounts.Where(x => accountParents.Contains(x.ParentRef) && accountParents.Contains(x.Code)),
                ItemTotal = itemTotal
            };
        }
    }
}