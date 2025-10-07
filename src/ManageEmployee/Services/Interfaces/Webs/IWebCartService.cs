using ManageEmployee.DataTransferObject.Requests;
using ManageEmployee.DataTransferObject.Response;
using ManageEmployee.DataTransferObject.Web;

namespace ManageEmployee.Services.Interfaces.Webs;

public interface IWebCartService
{
    Task<CommonWebResponse> Create(CartEditViewModel request, int userId);

    Task<CommonWebResponse> GetByCustomerId(int cusId);

    Task<List<ShoppingCartProductInfoResponse>> GetCartProductInfoAsync(List<ShoppingCartProductInfoRequest> cartProducts);

    Task<CommonWebResponse> Update(CartEditViewModel request, int userId);

    Task<CommonWebResponse> Delete(int id, int userId);

    Task<CommonWebResponse> DeleteAll(int userId);

    Task<int> CountCart(int userId);
}
