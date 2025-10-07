using ManageEmployee.DataTransferObject.UserModels;

namespace ManageEmployee.Services.Interfaces.Allowances;

public interface IAllowanceUserService
{
    IEnumerable<AllowanceUserViewModel> GetAll(int currentPage, int pageSize, string keyword);
    string Update(AllowanceUserViewModel user);
}
