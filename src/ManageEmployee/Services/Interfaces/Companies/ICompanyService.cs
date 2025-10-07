using ManageEmployee.DataTransferObject.CompanyModels;
using ManageEmployee.Entities.CompanyEntities;

namespace ManageEmployee.Services.Interfaces.Companies;

public interface ICompanyService
{
    Task<List<Company>> GetAll(int currentPage, int pageSize);

    Task<Company> GetCompany();

    Task<Company> GetById(int id);

    Task Create(Company param);

    Task<Company> Update(CompanyViewModel param);

    Task Delete(int id);
}
