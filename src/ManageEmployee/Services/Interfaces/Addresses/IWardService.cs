using System.Linq.Expressions;
using ManageEmployee.Entities.AddressEntities;

namespace ManageEmployee.Services.Interfaces.Addresses;

public interface IWardService
{
    Task<List<Ward>> GetAll(Expression<Func<Ward, bool>> where);

    IEnumerable<Ward> GetAllByDistrictId(int districtId);

    IEnumerable<Ward> GetAll(int currentPage, int pageSize);

    Ward GetById(int id);

    Ward Create(Ward param);

    void Update(Ward param);

    void Delete(int id);

    int Count(Expression<Func<Ward, bool>> where = null);
}
