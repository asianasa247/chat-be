using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.DataTransferObject.InOut;

public class InOutHistoryFilterParams : PagingRequestModel
{
    public int? DepartmentId { get; set; }
    public int? TargetId { get; set; }
    public DateTime? DateTimeKeep { get; set; }
}
