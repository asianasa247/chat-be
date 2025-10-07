
namespace ManageEmployee.DataTransferObject.TimeKeeperModels;

public class LeaveReportModel
{
    public int UserId { get; set; }
    public string SymbolCode { get; set; }
    public DateTime Date { get; set; }
    public DateOnly DateOnly => DateOnly.FromDateTime(Date);
    public double TotalHours { get; set; }
    public double SymbolHours { get; set; }
    public DateTime ToDate { get; set; }
    public DateTime FromDate { get; set; }
    public bool IsFinish { get; set; }
    public bool IsLicensed { get; set; }
}
