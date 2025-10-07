using ManageEmployee.DataTransferObject.SearchModels;

namespace ManageEmployee.Services.Interfaces.Customers;

public interface ICustomerReporter
{
    Task<string> ExportExcel(CustomersSearchViewModel param, int userId);

    Task<string> ImportExcel(List<CustomerImport> datas, int UserId, int type, string roles);

    Task<object> ChartBirthdayForCustomer(int userId, int type);
}
