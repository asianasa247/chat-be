namespace ManageEmployee.DataTransferObject.Reports;

public class AccoutantBalanceModelBase
{
    public string? title { get; set; }
    public string? code { get; set; }
    public string? action { get; set; }
    public string? codeAction { get; set; }
    public string? codeException { get; set; }
    public string? codegroup { get; set; }
    public string? subs { get; set; }
    public string? ghichu { get; set; }
    public double socuoinam { get; set; }
    public double sodaunam { get; set; }
    public int type { get; set; }
    public List<AccoutantBalanceModelBase>? items { get; set; }
    public string? duration { get; set; }
}
