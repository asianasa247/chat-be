using ManageEmployee.Entities.AddressEntities;

namespace ManageEmployee.Services.Interfaces.Addresses;

public interface IProvinceService
{
    Task<IEnumerable<Province>> GetAll();

    IEnumerable<Province> GetAll(int currentPage, int pageSize);

    Province GetById(int id);

    Task Create(Province param);

    Task Update(Province param);

    Task Delete(int id);

    int Count();
}
