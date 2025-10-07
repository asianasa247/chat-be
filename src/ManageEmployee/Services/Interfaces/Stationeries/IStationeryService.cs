using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Stationery;

namespace ManageEmployee.Services.Interfaces.Stationeries;

public interface IStationeryService
{
    Task<IEnumerable<StationeryModel>> GetList();
    Task<PagingResult<StationeryModel>> GetPaging(PagingRequestModel param);
    Task Create(StationeryModel param);
    Task Update(StationeryModel param);
    Task<StationeryModel> GetById(int id);
    Task Delete(int id);
}
