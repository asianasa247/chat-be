namespace ManageEmployee.DataTransferObject.Reports;

public class AccountBalanceReportParam
{
    public int? FromMonth { get; set; }
    public int? ToMonth { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string AccountCode { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string PreparedBy { get; set; } = string.Empty;
    public List<int> PrintType { get; set; } = new List<int>();
    public bool FillFullName { get; set; }
    public string? FileType { get; set; }
    public bool IsNoiBo { get; set; }
    public string StartAccount { get; set; }
}
