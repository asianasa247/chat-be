using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.Services.Interfaces.Reports
{
    public interface ISoChiTiet_FourReporter
    {
        Task<string> ReportAsync(LedgerReportParamDetail _param, int year);
    }
}
