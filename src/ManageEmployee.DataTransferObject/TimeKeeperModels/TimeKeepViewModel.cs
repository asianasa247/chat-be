using ManageEmployee.DataTransferObject.PagingRequest;

namespace ManageEmployee.DataTransferObject.TimeKeeperModels;

public class TimeKeepViewModel : PagingRequestModel
{
    public int? DepartmentId { get; set; }
    public DateTime DateTimeKeep { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? TargetId { get; set; }
    public bool CheckCurrentUser { get; set; } = false;
}