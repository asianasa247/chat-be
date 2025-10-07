namespace ManageEmployee.DataTransferObject.TimeKeeperModels;

public class TimeKeepingReportDateModel
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string StrFromDate => FromDate.ToString("dd/MM/yyyy");
    public string StrToDate => ToDate.ToString("dd/MM/yyyy");
    public DateTime LeaveFromDate { get; set; }
    public DateTime LeaveToDate { get; set; }
    public string StrLeaveFromDate => LeaveFromDate.ToString("dd/MM/yyyy");
    public string StrLeaveToDate => LeaveToDate.ToString("dd/MM/yyyy");
    public List<DateTime> Dates { get; set; }
}
