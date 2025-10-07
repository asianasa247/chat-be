using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.DataTransferObject;

public class RelationShipViewModel : PagingRequestModel
{
    public int EmployeeId { get; set; }
}
