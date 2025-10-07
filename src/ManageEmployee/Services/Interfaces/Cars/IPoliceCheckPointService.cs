using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.CarEntities;

namespace ManageEmployee.Services.Interfaces.Cars;

public interface IPoliceCheckPointService
{
    Task Create(PoliceCheckPoint form);
    Task Delete(int id);
    Task<IEnumerable<PoliceCheckPoint>> GetAll();
    Task<PoliceCheckPoint> GetDetail(int id);
    Task<PagingResult<PoliceCheckPoint>> GetPaging(PagingRequestModel searchRequest);
    Task Update(PoliceCheckPoint form);
}
