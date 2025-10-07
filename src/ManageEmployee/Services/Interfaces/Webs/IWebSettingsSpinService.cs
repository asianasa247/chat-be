using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities.LotteryEntities;

namespace ManageEmployee.Services.Interfaces.Webs
{
    public interface IWebSettingsSpinService
    {
        Task<List<WebSpinWithCustomerModel>> GetAsync ();
        Task<List<SpinCustomerPrizeModel>> GetSpinCustomerPrizeAsync(int? spinId);
        Task<List<SettingsSpin>> GetListCurrentSpinAsync();
        Task<List<SettingsSpin>> GetListSpinAsync();
    }
}
