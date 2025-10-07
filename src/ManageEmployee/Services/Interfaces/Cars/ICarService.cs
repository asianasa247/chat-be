using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Cars;

public interface ICarService
{
    Task<IEnumerable<CarGetterModel>> GetList();

    Task<PagingResult<CarGetterPagingModel>> GetPaging(PagingRequestModel param);

    Task Create(CarModel param);

    Task Update(CarModel param);

    Task Delete(int id);

    Task<CarGetterDetailModel> GetById(int id);

    Task<List<CarFieldSetupGetterModel>> GetCarFieldSetup(int carId);
    Task UpdateCarFieldSetup(int carId, List<CarFieldSetupModel> carFieldSetups);
}
