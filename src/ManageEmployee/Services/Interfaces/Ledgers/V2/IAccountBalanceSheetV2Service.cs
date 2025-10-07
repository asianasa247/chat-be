using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Entities.PocoSelections;

namespace ManageEmployee.Services.Interfaces.Ledgers.V2;

public interface IAccountBalanceSheetV2Service
{
    Task<string> GenerateReport(AccountBalanceReportParam param, int year);
    Task<(List<AccountBalanceSheetPocoData>, AccountBalanceSheetPocoData)> InitDataAsync(AccountBalanceReportParam param, int year);
}
