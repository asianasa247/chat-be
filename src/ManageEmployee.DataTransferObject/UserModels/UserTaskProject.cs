namespace ManageEmployee.DataTransferObject.UserModels;

public class UserTaskProject
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? UserCreated { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public int? ParentId { get; set; }// dự án cha
    public int? Status { get; set; }//0: đang mở, 1: đang tiến hành, 2: tạm hoãn, 3 hoàn thành
    public int? DepartmentId { get; set; }
    public int TypeWorkId { get; set; } = 0;
    public string? TypeWorkName { get; set; }
    public bool IsChildren { get; set; }
}
