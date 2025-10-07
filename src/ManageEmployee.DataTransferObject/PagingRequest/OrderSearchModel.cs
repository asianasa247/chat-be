using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.PagingRequest;

public class OrderSearchModel : PagingRequestModel
{
    public OrderStatus? status { get; set; }
    public double? fromDate { get; set; }
    public double? toDate { get; set; }
}
