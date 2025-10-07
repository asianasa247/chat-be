using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Reports;

namespace ManageEmployee.Services.Interfaces.Reports;

public interface ISavedMovedMoneyReportService
{
    void Calculator_Report(SaveMovedModelBase _modelBase, List<AccountBalanceItemModel> _accBalance, bool isNoiBo);
    Task<string> ExportDataReport(SavedMoneyReportParam request, int year, bool isNoiBo = false);
    List<SaveMovedModelBase> GetModelOnjectReport();
}
