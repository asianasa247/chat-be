using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Cars;

public interface IRoadRouteService
{
    Task Create(RoadRouteModel form);
    Task Delete(int id);
    Task<RoadRouteModel> GetDetail(int id);
    Task<PagingResult<RoadRoutePagingModel>> GetPaging(PagingRequestModel searchRequest);
    Task Update(RoadRouteModel form);
}
