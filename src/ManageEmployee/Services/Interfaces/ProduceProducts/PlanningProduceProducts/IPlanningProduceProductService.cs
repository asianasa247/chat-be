using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.Services.Interfaces.ProduceProducts.PlanningProduceProducts;

public interface IPlanningProduceProductService
{
    Task Accept(int id, int userId, int year);
    Task AddLedger(int id, int carId, string carName, int year);
    Task Canceled(int id);

    Task CancelPlanningDetail(int id, List<int> detailIds);

    Task Create(PlanningProduceProductModel form, int userId);

    Task Delete(int id);

    Task<CarDeliveryModel> GetCarDelivery(int? carId, string carName, int id);

    Task<PlanningProduceProductGetDetailModel> GetDetail(int id, int userId);

    Task<IEnumerable<PlanningProduceProductListGetterModel>> GetList(ProcedureForCreatePlanningProduct? procedureCode);

    Task<IEnumerable<CarGetterModel>> GetListCar(int id);

    Task<PagingResult<PlanningProduceProductPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId);

    Task<PaymentProposalModel> GetPaymentProposal(int? carId, string carName, int id);

    Task NotAccept(int id, int userId);

    Task SetCarDelivery(CarDeliveryModel carDelivery, int id);

    Task SetPaymentProposal(PaymentProposalModel model, int? carId, string carName, int id, int userId);

    Task Update(PlanningProduceProductModel form, int userId);

    Task UpdatePlanning(PlanningProduceProductGetDetailModel form, int userId);
    Task UpDateShouldExportDetail(int id, int? carId, string carName, int userId);
}
