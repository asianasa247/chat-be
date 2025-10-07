using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.Services.Interfaces.Reports;

public interface IReportDebitCustomerService
{
    Task<string> ReportAsyn(LedgerReportParam _param, int year);
}
