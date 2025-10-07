using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.WarehouseModel;

namespace ManageEmployee.Services.Interfaces.WareHouses;

public interface IWareHouseFloorService
{
    Task<PagingResult<WarehouseFloorPaging>> GetAll(PagingRequestModel param);
    Task<IEnumerable<WareHouseFloorGetAllModel>> GetAll();

    Task<WarehouseFloorSetterModel> GetById(int id);

    Task Create(WarehouseFloorSetterModel param);

    Task Update(WarehouseFloorSetterModel param);

    Task Delete(int id);
}
