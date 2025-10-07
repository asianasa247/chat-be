using ManageEmployee.Entities.UserEntites;

namespace ManageEmployee.DataTransferObject.UserModels;

/// <summary>
/// 0= đang mở
/// 1 = đang thực hiện
/// 2= Tạm hoãn
/// 3= Hoàn thành
/// </summary>
public class UserTaskModel
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? UserCreated { get; set; }//tạo bởi
    public DateTime? CreatedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public bool? ViewAll { get; set; }
    public int? ParentId { get; set; }
    public int? Status { get; set; }
    public bool IsDeleted { get; set; } = false;
    public int? DepartmentId { get; set; }
    //
    public int? TypeWorkId { get; set; } = 0;
    public string TypeWorkName { get; set; } = "";
    public double Point { get; set; }
    public bool isProject { get; set; } = false;
    public List<UserTaskCheckListModel>? CheckList { get; set; }
    public List<UserTaskRoleDetailsModel>? TaskRole { get; set; }//tham gia: UserTaskRoleId =2, quan sát UserTaskRoleId =3
    public List<UserTask>? ChildTask { get; set; }
    public List<UserTaskFileModel>? FileLink { get; set; }
    public int MinCheckList { get; set; }
    public int MaxCheckList { get; set; }
    public int? CustomerId { get; set; }

    // model responsible person
    public List<ResponsiblePerson>? ResponsiblePerson { get; set; }
    public List<ResponsiblePerson>? ParticipantPersons { get; set; }
    public List<ResponsiblePerson>? SupervisorPersons { get; set; }
    public ResponsiblePerson? ResponsibleUserCreated { get; set; }
}
