namespace ManageEmployee.DataTransferObject.UserModels;
public class UserTaskCommentModel //: UserTaskComment
{
    public int Id { get; set; }
    public int? UserTaskId { get; set; }
    public int? UserId { get; set; }
    public string? Type { get; set; }
    public string? Comment { get; set; }
    public int? ParentId { get; set; }
    public DateTime? CreatedDate { get; set; }
    public List<UserTaskFileModel>? FileLink { get; set; }
    public string? NameOfUser { get; set; }
    public bool IsAllowEdit { get; set; } = false;
    public string? Avatar { get; set; }
    public List<UserTaskRoleDetailsModel>? TaskRole { get; set; }
}
