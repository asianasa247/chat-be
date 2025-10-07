namespace ManageEmployee.DataTransferObject.TimeKeeperModels;

public class TimeKeepingHistoryByDateModel
{
    public TimeKeepingHistoryByDateModel(
        DateTime date,
        bool isMissingInOut = true,
        double workingHours = 0d,
        double overtimeHours = 0d,
        string symbolCode = "",
        double symbolHours = 0d)
    {
        Date = date;
        IsMissingInOut = isMissingInOut;
        WorkingHours = Math.Round(workingHours, 2);
        OvertimeHours = Math.Round(overtimeHours, 2);
        SymbolCode = symbolCode;
        SymbolHours = symbolHours;
    }

    public DateTime Date { get; set; }
    public DateOnly DateOnly => DateOnly.FromDateTime(Date);
    public double WorkingHours { get; set; }
    public double OvertimeHours { get; set; }
    public string SymbolCode { get; set; }
    public double SymbolHours { get; set; }
    private bool IsMissingInOut { get; set; }
    public bool IsDisplaySymbol => (!IsMissingInOut && WorkingHours >= SymbolHours) 
                                    || !string.IsNullOrWhiteSpace(SymbolCode);
}