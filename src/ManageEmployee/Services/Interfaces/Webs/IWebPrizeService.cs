
using ManageEmployee.DataTransferObject.Web;

namespace ManageEmployee.Services.Interfaces.Webs
{
    public interface IWebPrizeService
    {
        Task<List<WebPrizeModel>> Get();
    }
}
