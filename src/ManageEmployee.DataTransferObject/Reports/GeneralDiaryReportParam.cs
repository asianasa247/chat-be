namespace ManageEmployee.DataTransferObject.Reports;

public class GeneralDiaryReportParam
{
    public int FromMonth { get; set; } = 0;
    public int ToMonth { get; set; } = 0;
    public DateTime FromDate { get; set; } = DateTime.Now;
    public DateTime ToDate { get; set; } = DateTime.Now;
    public string FileType { get; set; } = string.Empty;
    public bool isCheckName { get; set; } = false;
    public string LedgerReportMaker { get; set; } = string.Empty;
}
