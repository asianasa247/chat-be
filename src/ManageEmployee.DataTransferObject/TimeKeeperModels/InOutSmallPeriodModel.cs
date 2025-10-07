using Common.Helpers;

namespace ManageEmployee.DataTransferObject.TimeKeeperModels;
public class InOutSmallPeriodModel
{
    private readonly DateTime? _timeFrameFrom;
    private readonly DateTime? _timeFrameTo;
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public DateTime? TimeIn { get; set; }
    public DateTime? TimeOut { get; set; }
    public string SymbolCode { get; set; }

    public bool IsMissingInOut => TimeIn == null || TimeOut == null;

    private DateTime? AcceptedTimeIn => !IsMissingInOut
        ? TimeIn.DateTimeMax(TimeFrameFrom)
        : null;

    private DateTime? AcceptedTimeOut => !IsMissingInOut
        ? TimeOut.DateTimeMin(TimeFrameTo)
        : null;

    public double TotalHours {
        get
        {
            if (TimeOut != null && TimeIn != null)
            {
                if (TimeOut.Value.Year == TimeIn.Value.Year
                    && TimeOut.Value.Year == TimeIn.Value.Year
                    && TimeOut.Value.Month == TimeIn.Value.Month
                    && TimeOut.Value.Day == TimeIn.Value.Day
                    && TimeOut.Value.Hour == TimeIn.Value.Hour
                    && TimeOut.Value.Minute == TimeIn.Value.Minute)
                {
                    return SymbolHours;
                }
                else
                {
                    return Math.Round((TimeOut.Value - TimeIn.Value).TotalHours, 2);
                }
            }
            else
            {
                return 0;
            }
        }
     }

    public DateTime? TimeFrameFrom
    {
        get => _timeFrameFrom;
        init =>
            _timeFrameFrom = value.HasValue
                ? new DateTime(value.Value.Year, value.Value.Month, value.Value.Day, value.Value.Hour,
                    value.Value.Minute, 0)
                : null;
    }

    public DateTime? TimeFrameTo
    {
        get => _timeFrameTo;
        init =>
            _timeFrameTo = value.HasValue
                ? new DateTime(value.Value.Year, value.Value.Month, value.Value.Day, value.Value.Hour,
                    value.Value.Minute, 0)
                : null;
    }

    public bool Missing { get; set; }
    public int WorkType { get; set; }
    public double SymbolHours { get; set; }
}