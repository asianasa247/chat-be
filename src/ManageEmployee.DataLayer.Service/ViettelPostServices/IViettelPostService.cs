using ManageEmployee.DataTransferObject.ViettelPostModels;

namespace ManageEmployee.DataLayer.Service.ViettelPostServices;

public interface IViettelPostService
{
    Task<string> LoginAsync(string username, string password);
    Task<ViettelPostOrderResponse> CreateOrderAsync(ViettelPostOrderRequest request, string token);
    Task<ViettelPostOrderResponse> GetPriceAsync(ViettelPostOrderRequest request, string token);
}