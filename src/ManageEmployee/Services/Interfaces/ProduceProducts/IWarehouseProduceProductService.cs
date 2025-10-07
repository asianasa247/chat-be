using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.DataTransferObject.SupplyModels;

namespace ManageEmployee.Services.Interfaces.ProduceProducts;

public interface IWarehouseProduceProductService
{
    Task Accept(int id, int userId);
    Task Create(int planningId, int userId);
    Task Delete(int id);
    Task<CarDeliveryModel> GetCarDelivery(int? carId, string carName, int id);
    Task<WarehouseProduceProductGetDetailModel> GetDetail(int id, int year);
    Task<PagingResult<WarehouseProduceProductPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId);
    Task<PaymentProposalModel> GetPaymentProposal(int? carId, string carName, int id);
    Task NotAccept(int id, int userId);
    Task SetCarDelivery(CarDeliveryModel carDelivery, int id);
    Task SetPaymentProposal(PaymentProposalModel model, int? carId, string carName, int id, int userId);
    Task Update(WarehouseProduceProductGetDetailModel form, int userId);
}
