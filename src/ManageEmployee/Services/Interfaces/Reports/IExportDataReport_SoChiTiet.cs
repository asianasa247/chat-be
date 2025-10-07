using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Reports;

namespace ManageEmployee.Services.Interfaces.Reports
{
    public interface IExportDataReport_SoChiTiet
    {
        Task<string> ReportAsync(LedgerReportModel ledgers, LedgerReportParamDetail param, int year);
    }
}
