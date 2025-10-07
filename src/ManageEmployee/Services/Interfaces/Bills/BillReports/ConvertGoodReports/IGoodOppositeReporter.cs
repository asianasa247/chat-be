using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.BillReportModels.ConvertGoodReports;

namespace ManageEmployee.Services.Interfaces.Bills.BillReports.ConvertGoodReports
{
    public interface IGoodOppositeReporter
    {
        Task<IEnumerable<GoodConvertReporterModel>> ReporterAsync(BillReportGoodRequestModel param, int year);
    }
}
