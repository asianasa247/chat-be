using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.DataTransferObject.CustomerModels;

public class CustomerQuoteSearchModel : PagingRequestModel
{
    public double? FromDate { get; set; }
    public double? ToDate { get; set; }
    public int CustomerId { get; set; }
}
