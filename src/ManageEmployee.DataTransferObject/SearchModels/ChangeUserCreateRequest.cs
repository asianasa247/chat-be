namespace ManageEmployee.DataTransferObject.SearchModels;

public class ChangeUserCreateRequest
{
    public int? UserId { get; set; }
    public List<int>? CustomerIds { get; set; }
}
