using ManageEmployee.DataTransferObject.LedgerModels;

namespace ManageEmployee.Services.Interfaces.Reports
{
    public interface ILedgerProjectReporter
    {
        Task<IEnumerable<LedgerProjectReporterModel>> ReportAsync(DateTime fromAt, DateTime toAt, string projectCode, int year, bool isNoiBo);
    }
}
