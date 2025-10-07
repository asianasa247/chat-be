using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.Services.Interfaces.Reports;

public interface IBalanceSheetService
{
    Task<string> ExportDataAccountantBalance(LedgerReportParam _param, int year, bool isNoiBo = false);
}
