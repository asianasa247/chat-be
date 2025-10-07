using Common.Models;

namespace Common.Helpers;

public static class TimeRangeFromRequest
{
    public static FilterTimeRangeParam TimeRange(DateTime? fromDate, DateTime? toDate, int? fromMonth, int? toMonth)
    {
        // Năm định khoản
        var year = DateTime.Now.Year;

        var from = new DateTime(year, 1, 1);
        var to = new DateTime(year, 12, 31, 23, 59, 59);

        if (fromDate.HasValue && toDate.HasValue)
        {
            if (fromDate.Value <=toDate.Value)
            {
                from = fromDate.Value;
                to = toDate.Value;
            }

        }
        else
        {
            if (fromMonth.HasValue && toMonth.HasValue)
            {
                int f = Math.Max(1, fromMonth.Value);

                if (f > 12) f = 12;

                int t = Math.Min(toMonth.Value, 12);

                if (t <= 0) t = 1;

                if (f <= t)
                {
                    from = new DateTime(year, f, 1);
                    var lastDate = DateTime.DaysInMonth(year, t);
                    to = new DateTime(year, t, lastDate, 23, 59, 59);
                }

            }
        }
        return new FilterTimeRangeParam(from, to);
    }
}
