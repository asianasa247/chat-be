
using ManageEmployee.DataTransferObject.CompanyModels;
using ManageEmployee.DataTransferObject.LedgerModels.VitaxInvoiceModels;

namespace ManageEmployee.HttpClients
{
    public interface IVitaxOneClient
    {
        Task<VintaxInvoiceInResponse<T>> GetAsync<T>(string uri, string taxCode, string passwordAccountTax);
        Task<OtherCompanyInfomationModel> GetCompanyInfoAsync(string uri, string taxCode);
        Task Login(string newPassword, string taxCode);
    }
}
