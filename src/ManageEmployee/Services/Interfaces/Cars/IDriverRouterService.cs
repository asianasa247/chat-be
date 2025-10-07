using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Cars;

public interface IDriverRouterService
{
    Task Delete(int id);
    Task Finish(int petrolConsumptionId);
    Task<DriverRouterModel> GetById(int id);
    Task<IEnumerable<PoliceCheckPointModel>> GetListPoliceCheckPoint(int id);
    Task<PagingResult<DriverRouterPagingModel>> GetPaging(PagingRequestModel param);
    Task Start(int petrolConsumptionId);
    Task Update(DriverRouterModel form);
}
