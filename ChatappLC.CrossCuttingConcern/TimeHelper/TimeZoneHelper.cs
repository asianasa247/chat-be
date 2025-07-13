namespace CrossCuttingConcerns.TimeHelper;

public static class TimeZoneHelper
{
    public static DateTime GetVietNamTimeNow() => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));
}
