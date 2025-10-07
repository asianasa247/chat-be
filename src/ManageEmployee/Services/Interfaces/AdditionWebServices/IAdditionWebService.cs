using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.AdditionWebs;
using ManageEmployee.Entities;

namespace ManageEmployee.Services.Interfaces.AdditionWebServices;

public interface IAdditionWebService
{
    Task<IEnumerable<AdditionWeb>> GetAllAsync();
    Task<AdditionWeb> GetByIdAsync(int id);
    Task<AdditionWeb> AddOrUpdateAsync(AdditionWebModel model);
    Task<AdditionWeb> UpdateAsync(int id, AdditionWebModel model);
    Task DeleteAsync(int id);
    Task<AdditionWebCompanyResult> GetCompanyInfo(int id);
    Task<List<AdditionWebCompanyResult>> GetCompaniesInfo();
    Task<List<AdditionWebCompanyShortResult>> GetCompaniesShortInfo();
}
