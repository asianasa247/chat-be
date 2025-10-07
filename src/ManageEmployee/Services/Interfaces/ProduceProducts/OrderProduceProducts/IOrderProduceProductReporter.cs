using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.ProduceProductModels;

namespace ManageEmployee.Services.Interfaces.ProduceProducts.OrderProduceProducts;

public interface IOrderProduceProductReporter
{
    Task<string> ExportPdf(int id);

    Task<string> ExportReportAsync(OrderProduceProductReportRequestModel param);

    Task<IEnumerable<OrderProduceProductReport>> ReportAsync(OrderProduceProductReportRequestModel param);
}
