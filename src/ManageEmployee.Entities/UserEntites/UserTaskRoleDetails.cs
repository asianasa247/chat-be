namespace ManageEmployee.Entities.UserEntites;

public class UserTaskRoleDetails
{
    public int Id { get; set; }
    public int UserTaskRoleId { get; set; }// role task: 1 trách nhiệm, 2 Tham gia, 3 quan sat
    public int UserId { get; set; }
    public int? UserTaskId { get; set; }
}
