using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.Entities.ProduceProductEntities;

namespace ManageEmployee.Services.Interfaces.ProduceProducts.OrderProduceProducts;

public interface IOrderProduceProductService
{
    Task Accept(int id, int userId, int year);

    Task Canceled(int id);

    Task<OrderProduceProduct> Create(OrderProduceProductCreateModel form, int userId);

    Task<OrderProduceProduct> CreateFromBill(int billId, int userId);

    Task Delete(int id);


    Task<OrderProduceProductModel> GetDetail(int id, int year);

    Task<PagingResult<OrderProduceProductPagingModel>> GetPaging(OrderProduceProductPagingRequestModel param, int userId);

    Task<string> GetProcedureNumber();

    Task NotAccept(int id, int userId);

    Task SetIsProduced(IEnumerable<int> detailIds);

    Task Update(OrderProduceProductModel form, int userId);
}
