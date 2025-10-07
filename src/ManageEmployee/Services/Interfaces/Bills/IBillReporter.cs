using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.Services.Interfaces.Bills;

public interface IBillReporter
{
    Task<BillReporterModel> ReportAsync(BillPagingRequestModel param);
    Task<object> ReportHomeAsync(RequestFilterDateModel query, int year);
}