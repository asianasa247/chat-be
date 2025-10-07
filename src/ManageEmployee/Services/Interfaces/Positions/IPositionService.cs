using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.CompanyEntities;

namespace ManageEmployee.Services.Interfaces.Positions;

public interface IPositionService
{
    IEnumerable<Position> GetAll();

    Task<PagingResult<Position>> GetAll(int currentPage, int pageSize, string keyword);

    Position GetById(int id);

    Position Create(Position param);

    void Update(Position param);

    void Delete(int id);
}
