using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.WarehouseModel;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.WareHouses;

public interface IWarehouseService
{
    IEnumerable<Warehouse> GetAll();

    Task<PagingResult<WarehousePaging>> GetAll(DepartmentRequest param);

    Task<WarehouseSetterModel> GetById(int id);

    Task Create(WarehouseSetterModel param, int userId, int yearFilter);

    Task Update(WarehouseSetterModel param, int userId, int yearFilter);

    Task Delete(int id);
}
