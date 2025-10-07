
using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.BillReportModels.BusinessOverviews;

namespace ManageEmployee.Services.Interfaces.Bills.BillReports.BusinessOverviews
{
    public interface IBusinessDetailForRevenueReporter
    {
        Task<BusinessDetailForGroupGoodReporterModel> ReportAsync(BillReportBranchRequestModel param);
    }
}
