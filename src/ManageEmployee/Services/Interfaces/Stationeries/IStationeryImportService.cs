using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Stationery;

namespace ManageEmployee.Services.Interfaces.Stationeries;

public interface IStationeryImportService
{
    Task<PagingResult<StationeryImportGetterModel>> GetPaging(PagingRequestModel param);
    Task Create(StationeryImportModel param);
    Task Update(StationeryImportModel param);
    Task Delete(int id);
    Task<StationeryImportModel> GetById(int id);
    Task<string> GetProcedureNumber();
}
