

namespace ManageEmployee.DataTransferObject.TimeKeeperModels;

public record class OverTimeHistoryModel
{
    public double TotalHours { get; set; }
    public List<int> UserIds { get; set; }
    public DateTime Date { get; set; }
}
