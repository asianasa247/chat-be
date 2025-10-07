using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.DataTransferObject.SearchModels;

public class CustomersHistoryRequestModel : PagingRequestModel
{
    public int? JobId { get; set; }
    public int? Status { get; set; }
    public int? ExportExcel { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? CustomerId { get; set; }
}
