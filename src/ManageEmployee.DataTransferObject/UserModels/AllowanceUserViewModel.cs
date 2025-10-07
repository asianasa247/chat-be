namespace ManageEmployee.DataTransferObject.UserModels;

public class AllowanceUserViewModel
{
    public int UserId { get; set; }
    public string? UserName { get; set; }
    public List<AllowanceUserDetailViewModel>? listItem { get; set; }
}
