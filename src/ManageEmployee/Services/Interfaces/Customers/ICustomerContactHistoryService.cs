using ManageEmployee.DataTransferObject.CustomerModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.SearchModels;
using ManageEmployee.Entities.CustomerEntities;

namespace ManageEmployee.Services.Interfaces.Customers;

public interface ICustomerContactHistoryService
{
    Task<IEnumerable<CustomerContactHistoryModel>> GetAll();

    Task<PagingResult<CustomerContactHistoryModel>> GetAllPaging(int pageIndex, int pageSize, string keyword);

    Task<PagingResult<CustomerContactHistoryPagingModel>> GetByCustomerId(CustomersHistoryRequestModel param);

    Task<CustomerContactHistoryDetailModel> Create(CustomerContactHistoryDetailModel param, int userId);

    Task<CustomerContactHistoryDetailModel> Update(CustomerContactHistoryDetailModel param, int userId);

    Task Delete(int id);
    Task<List<SelectListContactForCustomer>> GetListContactForCustomer(int customerId);
    Task AddContactForCustomer(SelectListContactForCustomer form, int customerId);
    Task<int> CountCustomerContact();
    Task<IEnumerable<CustomerContactHistoryNotificationModel>> GetAllCustomerContact();
    Task<CustomerContactHistoryDetailModel> GetDetail(int id);
}
