namespace ManageEmployee.DataTransferObject.InOutRemote;

public class InOutTime
{
    public string? UserCode { get; set; }
    public string? FullName { get; set; }
    public DateTime In { get; set; }
    public DateTime? Out { get; set; }
    public string InDate
    {
        get
        {
            return In.Date.ToString("dd-MM-yyyy");
        }
    }
    public string OutDate
    {
        get
        {
            if (Out.HasValue)
                return Out.Value.Date.ToString("dd-MM-yyyy");
            return "-";
        }
    }
    public string InTime
    {
        get
        {
            return In.TimeOfDay.ToString();
        }
    }
    public string OutTime
    {
        get
        {
            if (Out.HasValue)
                return Out.Value.TimeOfDay.ToString();
            return "-";
        }
    }

    public string TotalTime
    {
        get
        {
            if (Out.HasValue)
                return (Out.Value - In).Hours.ToString();
            return "-";
        }
    }
    public string? InStr { get; set; }
    public string? OutStr { get; set; }
    public string? SymbolCode { get; set; }
    public string? SymbolName { get; set; }
}
