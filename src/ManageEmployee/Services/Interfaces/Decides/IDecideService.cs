using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.DecideEntities;

namespace ManageEmployee.Services.Interfaces.Decides;

public interface IDecideService
{
    Task<PagingResult<Decide>> GetAll(int currentPage, int pageSize, string keyword);

    Task Create(Decide param);

    Task Update(Decide param, IFormFile file);

    Task Delete(int id);

    Task<Decide> GetById(int id);
}
