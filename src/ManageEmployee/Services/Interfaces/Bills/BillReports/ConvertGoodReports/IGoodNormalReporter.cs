using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.BillReportModels.ConvertGoodReports;

namespace ManageEmployee.Services.Interfaces.Bills.BillReports.ConvertGoodReports
{
    public interface IGoodNormalReporter
    {
        Task<IEnumerable<GoodNormalReporterModel>> ReporterAsync(BillReportGoodRequestModel param, int year);
    }
}
