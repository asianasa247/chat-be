using System.Linq.Expressions;
using ManageEmployee.DataTransferObject.AddressModels;
using ManageEmployee.Entities.AddressEntities;

namespace ManageEmployee.Services.Interfaces.Addresses;

public interface IDistrictService
{
    Task<List<DistrictGetListModel>> GetAll(Expression<Func<DistrictGetListModel, bool>> where);

    Task<IEnumerable<District>> GetAllByProvinceId(int provinceId);

    Task<List<District>> GetAll(int currentPage, int pageSize);

    IEnumerable<District> GetMany(Expression<Func<District, bool>> where, int currentPage, int pageSize);

    IEnumerable<District> GetMany(Expression<Func<District, bool>> where);

    District GetById(int id);

    District Create(District param);

    void Update(District param);

    void Delete(int id);

    int Count(Expression<Func<District, bool>> where = null);
}
