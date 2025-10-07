using ManageEmployee.DataTransferObject.Enums;

namespace ManageEmployee.DataTransferObject.PagingRequest;

public class UserTaskRequestModel : PagingRequestModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<int>? Statuses { get; set; }
    public int? DepartmentId { get; set; }
    public int? ParentProject { get; set; }
    public int? ParentProjectId { get; set; }
    public int? UserCreatedId { get; set; }
    public int? CustomerId { get; set; }
    public bool IsExpired { get; set; }
    public TaskTypeEnum TaskType { get; set; }
}
