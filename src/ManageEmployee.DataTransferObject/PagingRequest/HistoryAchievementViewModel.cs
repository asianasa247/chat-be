namespace ManageEmployee.DataTransferObject.PagingRequest;

public class HistoryAchievementViewModel : PagingRequestModel
{
    public int? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
