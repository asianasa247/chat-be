using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.Relatives;

public interface IRelationShipService
{
    bool Create(RelationShip relative);

    bool Update(RelationShip relative);

    void Delete(int id);

    IEnumerable<RelationShip> GetListPaging(int _pageSize, int page, int employeeId);

    RelationShip GetById(int id);

    int Count(int employeeId);
}
