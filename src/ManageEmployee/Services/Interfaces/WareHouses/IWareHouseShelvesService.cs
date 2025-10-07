using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.WarehouseModel;

namespace ManageEmployee.Services.Interfaces.WareHouses;

public interface IWareHouseShelvesService
{
    Task<PagingResult<WareHouseShelvesPaging>> GetAll(PagingRequestModel param);
    Task<IEnumerable<WareHouseShelvesGetAllModel>> GetAll();

    Task<WarehouseShelveSetterModel> GetById(int id);

    Task Create(WarehouseShelveSetterModel param);

    Task Update(WarehouseShelveSetterModel param);

    Task Delete(int id);
}
