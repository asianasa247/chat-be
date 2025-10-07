using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.DataTransferObject.BillModels;

public class BillPagingRequestModel : RequestFilterDateModel
{
    public int UserId { get; set; }
}
