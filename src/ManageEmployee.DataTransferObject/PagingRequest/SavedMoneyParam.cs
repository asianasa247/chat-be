namespace ManageEmployee.DataTransferObject.PagingRequest;

public class SavedMoneyReportParam
{
    public int? FromMonth { get; set; }
    public int? ToMonth { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string VoucherType { get; set; } = "PT";
    public string VoteMaker { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public bool isCheckName { get; set; }
    public int PreviousYear { get; set; }
}
