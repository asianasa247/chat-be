namespace Common.Helpers;

public static class ConvertCell
{
    public static int ConvertCellToInt(object? value)
    {
        if (value is null ||string.IsNullOrEmpty(value.ToString()))
            return 0;
        return int.Parse(value.ToString() ?? "0");
    }
    public static DateTime? ConvertCellToDatetime(object? value)
    {
        if (value is null || string.IsNullOrEmpty(value.ToString()))
            return null;
        return DateTime.Parse(value.ToString() ?? "0");
    }
    public static double ConvertCellToDecimal(object? value)
    {
        if (value is null || string.IsNullOrEmpty(value.ToString()))
            return 0;
        return double.Parse(value.ToString() ?? "0");
    }

    public static string? ConvertCellToString(object? value)
    {
        if (value  is null)
            return string.Empty;
        return value.ToString();
    }
}
