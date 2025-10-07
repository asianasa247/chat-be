using ManageEmployee.DataTransferObject.CompanyModels;
using ManageEmployee.HttpClients;
using ManageEmployee.Services.Interfaces.Companies;

namespace ManageEmployee.Services.CompanyServices
{
    public class OtherCompanyInfoGetter : IOtherCompanyInfoGetter
    {
        private readonly IVitaxOneClient _httpClient;
        private readonly ICompanyService _companyService;

        public OtherCompanyInfoGetter(IVitaxOneClient httpClient, ICompanyService companyService)
        {
            _httpClient = httpClient;
            _companyService = companyService;
        }

        public async Task<OtherCompanyInfomationModel> GetInforCompany(string taxCode)
        {
            var company = await _companyService.GetCompany();
            var baseUrl = $"Invoices/search-basic-info-nnt?mst={taxCode}";
            var result = await _httpClient.GetCompanyInfoAsync(baseUrl, company.MST);
            return result;
        }
    }
}