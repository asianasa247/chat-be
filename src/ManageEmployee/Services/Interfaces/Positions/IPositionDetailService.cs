using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.CompanyEntities;

namespace ManageEmployee.Services.Interfaces.Positions;

public interface IPositionDetailService
{
    Task<IEnumerable<PositionDetail>> GetAll();

    Task<PagingResult<PositionDetailModel>> GetAll(int currentPage, int pageSize, string keyword);

    Task Create(PositionDetail param);

    Task Update(PositionDetail param);

    Task Delete(int id);

    Task<PositionDetail> GetById(int id);
}
