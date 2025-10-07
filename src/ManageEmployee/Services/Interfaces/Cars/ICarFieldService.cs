using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Cars;

public interface ICarFieldService
{
    Task Create(CarFieldModel param);
    Task Delete(int id);
    Task<CarFieldModel> GetById(int id);
    Task<PagingResult<CarFieldPagingModel>> GetPaging(PagingRequestModel param);
    Task Update(CarFieldModel param);
}
