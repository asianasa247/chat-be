using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.Majors;

public interface IMajorService
{
    Task<IEnumerable<Major>> GetAll();

    Task<PagingResult<Major>> GetAll(int currentPage, int pageSize, string keyword);

    Task<Major> GetById(int id);

    Task Create(Major param);

    Task Update(Major param);

    Task Delete(int id);
}
