using ManageEmployee.DataTransferObject.Web;

namespace ManageEmployee.Services.Interfaces.Webs;

public interface IWebOrderService
{
    Task Create(OrderViewModel request, int? userId = 0);

    Task<CommonWebResponse> GetByCustomer(WebOrderSearchModel searchModel);
}
