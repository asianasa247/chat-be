using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.BillReportModels.BusinessOverviews;

namespace ManageEmployee.Services.Interfaces.Bills.BillReports.BusinessOverviews
{
    public interface IBusinessOverviewReporter
    {
        Task<IEnumerable<BusinessReportForBranchModel>> ReportForBranchAsync(BillReportTimeRequestModel param, int year);
        Task<BusinessOverviewReportModel> ReportOverviewAsync(BillReportBranchRequestModel param, int year);
    }
}
