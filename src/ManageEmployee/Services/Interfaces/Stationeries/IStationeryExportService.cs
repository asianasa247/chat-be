using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Stationery;

namespace ManageEmployee.Services.Interfaces.Stationeries;

public interface IStationeryExportService
{
    Task<PagingResult<StationeryExportGetterModel>> GetPaging(PagingRequestModel param);
    Task Create(StationeryExportModel param);
    Task Update(StationeryExportModel param);
    Task Delete(int id);
    Task<StationeryExportModel> GetById(int id);
    Task<string> GetProcedureNumber();
}
