using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Web;

namespace ManageEmployee.Services.Interfaces.Goods
{
    public interface IGoodCustomerService
    {
        Task AddAsync(int goodId, int customerId);
        Task<PagingResult<WebGoodModel>> GetGoodForCustomer(PagingRequestModel searchModel, int customerId);
        Task RemoveAsync(int goodId, int customerId);
    }
}
