using ManageEmployee.DataTransferObject.UserModels;

namespace ManageEmployee.Services.Interfaces.Users;

public interface IUserSalaryService
{
    Task<string> ExportSalary(int month, int isInternal);
    Task<List<User_SalaryModel>> GetListUserSalary(int month, int isInternal);
    Task UpdateSalaryToAccountant(int month, int isInternal, int year);
}
