using ManageEmployee.DataTransferObject.AllowanceModels;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Allowances;

public interface IAllowanceService
{
    Task<IEnumerable<AllowanceSelectList>> GetAll();

    Task<PagingResult<AllowanceViewModel>> GetAll(int currentPage, int pageSize, string keyword);

    Task<AllowanceViewModel> GetById(int id);

    Task Create(AllowanceViewModel param, int userId);

    Task Update(AllowanceViewModel param, int userId);

    Task Delete(int id);
}
