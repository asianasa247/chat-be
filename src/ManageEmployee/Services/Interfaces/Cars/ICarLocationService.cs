using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Cars;

public interface ICarLocationService
{
    Task Accept(int id, int userId);
    Task Create(CarLocationModel form, int userId);
    Task Delete(int id);
    Task<string> Export(int id);
    Task<CarLocationModel> GetDetail(int id);
    Task<PagingResult<CarLocationPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId);
    Task<string> GetProcedureNumber();
    Task NotAccept(int id, int userId);
    Task Update(CarLocationModel form, int userId);
}
