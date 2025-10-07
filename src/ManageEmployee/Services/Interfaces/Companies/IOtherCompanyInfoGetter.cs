
using ManageEmployee.DataTransferObject.CompanyModels;

namespace ManageEmployee.Services.Interfaces.Companies;

public interface IOtherCompanyInfoGetter
{
    Task<OtherCompanyInfomationModel> GetInforCompany(string taxCode);
}
