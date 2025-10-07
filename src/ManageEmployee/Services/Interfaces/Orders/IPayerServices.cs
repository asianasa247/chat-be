using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.CustomerModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.OrderEntities;

namespace ManageEmployee.Services.Interfaces.Orders;

public interface IPayerServices
{
    Task<IEnumerable<Payer>> GetAll();

    Task<IEnumerable<PayerModelView>> GetAll(string searchText = "");

    Task<List<Payer>> GetPage(int currentPage, int pageSize, string query = "", int payerType = 1);

    Task<List<Payer>> GetTaxCodes(int currentPage, int pageSize, string query = "");

    Task<int> CountPageTotal(string query = "", int payerType = 1);

    Task<int> CountTaxCodeTotal(string query = "");

    Task<string> Create(Payer entity);

    Task<string> Update(Payer entity);

    Task<string> Delete(long id);

    Task Delete(IEnumerable<long> ids);

    Task<PagingResult<Payer>> GetListPayerWithCustomerTax(PayerPagingationRequestModel param);
}
