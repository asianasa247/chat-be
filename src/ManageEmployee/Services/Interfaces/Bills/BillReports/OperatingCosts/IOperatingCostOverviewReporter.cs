using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.BillReportModels.OperatingCosts;

namespace ManageEmployee.Services.Interfaces.Bills.BillReports.OperatingCosts
{
    public interface IOperatingCostOverviewReporter
    {
        Task<OperatingCostOverviewReportModel> ReportAsync(BillReportTimeRequestModel param, int year);
    }
}
