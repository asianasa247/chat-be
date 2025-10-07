using Common.Helpers;
using ManageEmployee.DataTransferObject.Enums;

namespace ManageEmployee.DataTransferObject.TimeKeeperModels;

public class TimeKeepingInOuHistory : TimeFrame
{
    public int SymbolId { get; set; }
    public DateTime? CheckinTime { get; set; }
    public DateTime? CheckoutTime { get; set; }
    public string CheckinTrace { get; set; }
    public string CheckoutTrace { get; set; }
    public int? CheckinTargetId { get; set; }
    public int? CheckoutTargetId { get; set; }
    public int CheckInTimeThreshold { get; set; }
    public int CheckOutTimeThreshold { get; set; }
}

public class TimeFrame
{
    public string Name { get; set; }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}

public class InOutTrace
{
    public DateTime Date { get; set; }
    public string Data { get; set; }
}

public class InOutHistoryTimeline : TimeFrame
{
    public string Type { get; set; }
    public string Status { get; set; }
    public bool Success => SubmitTime != null;
    public DateTime? SubmitTime { get; set; }
    public bool IsCheckedIn { get; set; }

    public int CheckInTimeThreshold { get; set; }

    public int CheckOutTimeThreshold { get; set; }
    public string Progress
    {
        get
        {
            var ableInOutFrom = From.AddMinutes(-CheckInTimeThreshold);
            var ableInOutTo = To.AddMinutes(CheckOutTimeThreshold);

            // Passed
            if
            (
                Success ||
                ableInOutTo <= DateTime.Now
            )
            {
                return InOutTimeLineStatusEnum.Passed.GetDescription();
            }

            // InProgress
            var isInTimeRange = ableInOutFrom <= DateTime.Now && DateTime.Now <= ableInOutTo;
            if (
                isInTimeRange &&
                SubmitTime == null &&
                (Type == "IN" || Type == "OUT" && IsCheckedIn)
            )
            {
                return InOutTimeLineStatusEnum.InProgress.GetDescription();
            }

            // InComing
            return InOutTimeLineStatusEnum.InComing.GetDescription();
        }
    }
}