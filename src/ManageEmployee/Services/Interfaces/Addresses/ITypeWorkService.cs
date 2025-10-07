using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.Addresses;

public interface ITypeWorkService
{
    IEnumerable<TypeWork> GetAll();

    Task<PagingResult<TypeWorkModel>> GetAll(int currentPage, int pageSize);

    TypeWork GetById(int id);

    Task Create(TypeWorkModel param);

    Task Update(TypeWorkModel param);

    void Delete(int id);
}
