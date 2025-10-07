using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.AdditionWebServices;

public interface IAdditionWebOrderService
{
    Task<PagingResult<OrderViewModelResponse>> SearchOrder(int webId, OrderSearchModel search);
}
