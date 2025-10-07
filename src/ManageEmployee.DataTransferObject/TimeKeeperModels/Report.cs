namespace ManageEmployee.DataTransferObject.TimeKeeperModels;
public class Report
{
    public string FullName { get; set; }
    public string Code { get; set; }
    public int UserId { get; set; }
    public string DepartmentName { get; set; }
    public List<GetForReport> Histories { get; set; }
    public int SymbolId { get; set; }

    public int TotalWorkingDay
    {
        get
        {
            if (Histories == null || Histories.Count <= 0)
                return 0;
            return Histories.GroupBy(x => x.DateTimeKeep.Day)
                .Count();
        }
    }
    public int TotalPaidLeave
    {
        get
        {
            if (Histories == null || Histories.Count <= 0)
                return 0;
            return Histories.Where(x => x.IsOverTime == 3).GroupBy(x => x.DateTimeKeep.Day)
                .Count();
        }
    }
    public int TotalUnPaidLeave
    {
        get
        {
            if (Histories == null || Histories.Count <= 0)
                return 0;
            return Histories.Where(x => x.IsOverTime == 4).GroupBy(x => x.DateTimeKeep.Day)
                .Count();
        }
    }
    public double TotalPaid
    {
        get
        {
            return TotalPaidLeave + TotalWorkingDay;
        }
    }

    public double TotalWorkingHours
    {
        get
        {
            if (Histories == null || Histories.Count <= 0)
                return 0;
            return Histories
                .Where(x => x.IsOverTime == 1)
                .Sum(x => x.TimeKeepSymbolTimeTotal);
        }
    }

    public double TotalOverTimeWorkingHours
    {
        get
        {
            if (Histories == null || Histories.Count <= 0)
                return 0;
            return Histories
                .Where(x => x.IsOverTime == 2) // 1 - BT; 2-TC; 3-P; 4-KP
                .Sum(x => x.TimeKeepSymbolTimeTotal);
        }
    }

    public List<OverTimeHistoryModel> OvertimesHistories { get; set; }

}