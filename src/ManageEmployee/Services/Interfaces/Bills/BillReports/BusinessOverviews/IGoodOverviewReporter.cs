using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.BillReportModels.GoodReports;

namespace ManageEmployee.Services.Interfaces.Bills.BillReports.BusinessOverviews
{
    public interface IGoodOverviewReporter
    {
        Task<GoodOverviewReporterModel> ReportAsync(BillReportBranchRequestModel param, int year);
    }
}
