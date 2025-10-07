namespace ManageEmployee.DataTransferObject.PagingRequest;

public class UpdateHistoryAchievementViewModel
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string? Description { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Note { get; set; }
    public DateTime Date { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
    public DateTime? DeleteAt { get; set; }
    public bool IsDelete { get; set; }
}