using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Cars;

public interface IPetrolConsumptionService
{
    Task<PagingResult<PetrolConsumptionGetterModel>> GetPaging(PagingRequestModel param);

    Task Create(PetrolConsumptionModel param);

    Task Update(PetrolConsumptionModel param);

    Task Delete(int id);

    Task<PetrolConsumptionModel> GetById(int id);
    Task<IEnumerable<PetrolConsumptionReportModel>> ReportAsync(PetrolConsumptionReportRequestModel param);
}
