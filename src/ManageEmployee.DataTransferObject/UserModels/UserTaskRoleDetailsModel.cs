namespace ManageEmployee.DataTransferObject.UserModels;

public class UserTaskRoleDetailsModel
{
    public int Id { get; set; }
    public int? UserTaskRoleId { get; set; }
    public int? UserId { get; set; }
    public int? UserTaskId { get; set; }

}
