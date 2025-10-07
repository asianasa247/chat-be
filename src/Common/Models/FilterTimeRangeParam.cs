namespace Common.Models;

public class FilterTimeRangeParam
{
    public FilterTimeRangeParam(DateTime from, DateTime to)
    {
        From = from;
        To = to;
    }
    public DateTime From { get; set; }
    public DateTime To { get; set; }
}
