namespace ManageEmployee.DataTransferObject.UserModels;

public class UserTaskCheckListModel
{
    public int Id { get; set; }
    public int? UserTaskId { get; set; }
    public string? Name { get; set; }
    public string? FileLink { get; set; }
    public bool? Status { get; set; }


}
