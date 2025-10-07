using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.CustomerModels;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Customers;

public interface ICustomerQuoteService
{
    Task<PagingResult<CustomerQuoteModel>> GetListCustomerQuoteHistory(CustomerQuoteSearchModel search);
    Task<List<CustomerQuote_DetailModel>> GetListCustomerQuoteDetail(long CustomerQuoteId);
    Task<string> ConvertToHTML_BaoGia(long CustomerQuoteId, string type, int CustomerId);
    Task<(object customerQuote, List<CustomerQuote_DetailModel> customerQuoteDetails)> GetDataBaoGia(long CustomerQuoteId);
    Task<object> ExportCustomerQuote(List<BillDetailModel> model, int customerId, int userId, int year);
}
