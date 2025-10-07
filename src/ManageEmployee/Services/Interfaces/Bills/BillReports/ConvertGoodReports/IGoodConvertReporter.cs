using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.BillReportModels.ConvertGoodReports;

namespace ManageEmployee.Services.Interfaces.Bills.BillReports.ConvertGoodReports
{
    public interface IGoodConvertReporter
    {
        Task<IEnumerable<GoodConvertBeforeReporterModel>> ReporterAsync(BillReportGoodRequestModel param, int year);
    }
}
