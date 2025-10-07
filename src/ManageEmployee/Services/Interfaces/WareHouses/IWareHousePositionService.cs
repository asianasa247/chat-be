using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.WarehouseModel;
using ManageEmployee.Entities.WareHouseEntities;

namespace ManageEmployee.Services.Interfaces.WareHouses;

public interface IWareHousePositionService
{
    Task<PagingResult<WareHousePosition>> GetAll(PagingRequestModel param);
    Task<IEnumerable<WareHousePositionGetAllModel>> GetAll();

    Task<WareHousePosition> GetById(int id);

    Task<WareHousePosition> Create(WareHousePosition param);

    Task Update(WareHousePosition param);

    Task Delete(int id);
}
