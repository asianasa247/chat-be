using ManageEmployee.Entities.UserEntites;

namespace ManageEmployee.DataTransferObject.UserModels;

public class UserTaskModeList
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? UserCreated { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public bool? ViewAll { get; set; }
    public int? ParentId { get; set; }
    public int? Status { get; set; }
    public bool? IsDeleted { get; set; }
    public string DueDateMode { get; set; } = "";
    public int? OrderNo { get; set; }
    public List<ResponsiblePerson>? ResponsiblePerson { get; set; }
    public ResponsiblePerson? ResponsibleUserCreated { get; set; }
    public int? Viewer { get; set; }
    public string? Activity { get; set; }
    public string? CreatePerson { get; set; }
    public string? Project { get; set; }
    public double? ActualHours { get; set; }
    public bool? IsChildren { get; set; }
    public bool? IsSupervisor { get; set; }
    public int IsStatusForManager { get; set; }
    public List<ResponsiblePerson>? ParticipantPersons { get; set; }
    public List<ResponsiblePerson>? SupervisorPersons { get; set; }
    public List<UserTaskCheckList>? CheckList { get; set; }
    public int MinCheckList { get; set; }
    public int MaxCheckList { get; set; }
    public bool IsExistTask { get; set; }
}
