using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.MainColors;

public interface IMainColorService
{
    Task<PagingResult<MainColor>> GetAll(int pageIndex, int pageSize, string keyword, int userId);

    Task<string> Create(MainColor request, int userId);

    Task<MainColor> GetById(int id);

    Task<string> Update(MainColor request);

    Task<string> Delete(int id);

    Task<MainColor> GetByUserId(int userId);
}
