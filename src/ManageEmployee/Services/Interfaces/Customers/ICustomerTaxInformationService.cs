using ManageEmployee.DataTransferObject.CustomerModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.CustomerEntities;

namespace ManageEmployee.Services.Interfaces.Customers;

public interface ICustomerTaxInformationService
{
    IEnumerable<CustomerTaxInformation> GetAll();
    PagingResult<CustomerTaxInformation> GetAll(int pageIndex, int pageSize, string keyword);
    Task<CustomerTaxInformationModel> GetById(int id);
    Task<CustomerTaxInformationModel> GetByCustomerId(int customerId);
    Task<CustomerTaxInformation> Create(CustomerTaxInformationModel param);
    Task<CustomerTaxInformation> Update(CustomerTaxInformationModel model, int customerId);
    Task<string> Delete(int id);
}
