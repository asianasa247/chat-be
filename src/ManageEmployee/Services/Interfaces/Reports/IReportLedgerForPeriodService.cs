using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.Services.Interfaces.Reports;

public interface IReportLedgerForPeriodService
{
    Task PerformAsync(LedgerReportParam param);
}
