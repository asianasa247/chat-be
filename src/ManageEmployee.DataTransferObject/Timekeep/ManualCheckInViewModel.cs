namespace ManageEmployee.DataTransferObject.Timekeep;

public class ManualCheckInViewModel
{
    public int UserId { get; set; }
    public string? Username { get; set; }
    public string? FullName { get; set; }
    public int DefaultSymbolId { get; set; }
    public string? DefaultSymbolCode { get; set; }
    public int TargetId { get; set; }
    public string? TargetName { get; set; }
    public int CheckInCount { get; set; }
    public int CheckInMethod { get; set; }
    public List<CheckInHistory>? CheckinHistories { get; set; }
}
