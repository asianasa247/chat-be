// File: ManageEmployee/Services/Interfaces/Customers/ICustomerService.cs
// (KHÔNG cần thay đổi chữ ký interface — Create/Update đã nhận Customer có EventCustomerId)

using ManageEmployee.DataTransferObject.CustomerModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.Entities.CustomerEntities;

namespace ManageEmployee.Services.Interfaces.Customers;

public interface ICustomerService
{
    Task<IEnumerable<CustomerModelView>> GetListCustomer(int type, List<long> customerId, string? searchText = null);

    Task<PagingResult<CustomerModelPaging>> GetPaging(CustomersSearchViewModel param, int userId, int year);

    Task<CustomerGetterModel> GetById(int id, int year, bool isForBill = false);

    Task<Customer> Create(Customer param);

    Task<Customer> Update(Customer param, string roles);

    Task<string> Delete(int id);

    Task<List<TotalJobStatus>> GetTotalJobByUserId(int pageIndex, int pageSize, string keyword, int jobId, int statusId, int userId, int exportExcel = 0);

    Task<List<TotalJobStatus>> GetTotalStatusByUserId(int pageIndex, int pageSize, string keyword, int jobId, int statusId, int userId, int exportExcel = 0);

    Task<string> GetCodeCustomer(int type);

    Task<IEnumerable<CustomerWarning>> CustomerWarning();

    Task<double> CustomerCnById(int id, int year);

    Task<IEnumerable<SelectListModel>> GetListCustomerWithCodeName();

    Task ValidateCustomer(Customer param);

    Task UpdateUserCreate(ChangeUserCreateRequest request);

    Task SetAccountanForCustomer(int id, AccountanForCustomerModel form);
    Task SyncAccountToCustomer(int type);
    Task CreateAcountFromCash(AccountanForCustomerModel form, int id, int year);
    Task<Customer> GetBySocialProviderId(string provider, string providerId);
}
