using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.Enumerations;

namespace ManageEmployee.DataTransferObject.Web;

public class WebOrderSearchModel : PagingRequestModel
{
    public int CustomerId { get; set; }
    public OrderStatus? Status { get; set; }
}
