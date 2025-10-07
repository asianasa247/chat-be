namespace ManageEmployee.DataTransferObject.InOutModels;

public class DateRangeFilter
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public IEnumerable<(int Month, int Year)> GetMonthsAndYears()
    {
        if (FromDate is null || ToDate is null)
        {
            var currentTime = FromDate ?? DateTime.Now;
            yield return (currentTime.Month, currentTime.Year);
            yield break;
        }

        var startDate = FromDate!.Value!;
        while (startDate <= ToDate)
        {
            yield return (startDate.Month, startDate.Year);
            startDate = startDate.AddMonths(1);
        }
    }

    public IEnumerable<(int Day, int Month, int Year)> GetDates()
    {
        if (FromDate is null || ToDate is null)
        {
            var currentTime = FromDate ?? DateTime.Now;
            yield return (currentTime.Day, currentTime.Month, currentTime.Year);
            yield break;
        }

        var startDate = FromDate!.Value!;
        while (startDate <= ToDate)
        {
            yield return (startDate.Day, startDate.Month, startDate.Year);
            startDate = startDate.AddDays(1);
        }
    }

    public bool IsBetween(DateTime time)
    {
        var dateOnly = DateOnly.FromDateTime(time);

        if (FromDate is null || ToDate is null)
        {
            var currentTime = FromDate ?? DateTime.Now;
            return dateOnly >= DateOnly.FromDateTime(currentTime);
        }

        return dateOnly >= DateOnly.FromDateTime(FromDate.Value) && dateOnly <= DateOnly.FromDateTime(ToDate.Value);
    }
}