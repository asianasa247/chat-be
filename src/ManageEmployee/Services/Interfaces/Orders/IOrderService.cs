using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Orders;

public interface IOrderService
{
    Task<PagingResult<OrderViewModelResponse>> SearchOrder(OrderSearchModel search);

    Task<string> Update(OrderViewModelResponse requests, int userId, int year);

    Task<IEnumerable<OrderViewModelResponse>> GetListOrderNew();

    Task<List<OrderDetailViewModel>> GetDetail(int id);
}
