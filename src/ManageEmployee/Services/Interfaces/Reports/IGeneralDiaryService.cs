using ManageEmployee.DataTransferObject.Reports;

namespace ManageEmployee.Services.Interfaces.Reports;

public interface IGeneralDiaryService
{
    Task<string> GenerateGeneralDiaryReport(GeneralDiaryReportParam param, int year);
}
