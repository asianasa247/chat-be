using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.Relatives;

public interface IRelativeService
{
    Task<IEnumerable<Relative>> GetAllUserActive();

    IEnumerable<Relative> Filter(RelativeMapper.FilterParams param);

    Task<Relative> GetById(int id);

    Relative Create(Relative relative);

    Relative Update(Relative relative);

    Task Delete(int id);

    IEnumerable<Relative> CountFilter(RelativeMapper.FilterParams param);

    Task<IEnumerable<Relative>> GetForExcel();
    Task<string> ExportRelationShip(int userId);
}
