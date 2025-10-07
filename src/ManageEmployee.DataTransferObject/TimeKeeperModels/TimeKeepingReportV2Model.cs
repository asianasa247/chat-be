namespace ManageEmployee.DataTransferObject.TimeKeeperModels;
public class TimeKeepingReportV2Model
{
    public string FullName { get; set; }
    public string Code { get; set; }
    public int UserId { get; set; }
    public string DepartmentName { get; set; }

    public double TotalWorkingDay { get; set; }

    public double TotalPaidLeave { get; set; }

    public double TotalUnPaidLeave { get; set; }
    public double TotalPaid => Math.Round(TotalPaidLeave + TotalWorkingDay + TotalOverTimeDay, 2);
    public double TotalWorkingHours { get; set; }
    public double TotalOverTimeWorkingHours { get; set; }
    public double TotalOverTimeDay { get; set; }
    public double TotalUnLicensed { get; set; }
    public IEnumerable<TimeKeepingHistoryByDateModel> Histories { get; set; }
}