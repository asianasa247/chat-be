using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SalaryModels;

namespace ManageEmployee.Services.Interfaces.Users.Salaries;

public interface ICalculateSalaryService
{
    Task<PagingResult<SalaryReportModel>> GetPaging(PagingRequestFilterByMonthModel param);
    Task CalculateSalaryByMonth(int month, int year);
    Task<string> ExportToExcel(int month, int year);
    Task<string> ExportToPdf(int month, int year);
}
