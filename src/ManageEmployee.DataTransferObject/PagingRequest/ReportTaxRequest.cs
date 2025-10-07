namespace ManageEmployee.DataTransferObject.PagingRequest;

public class ReportTaxRequest
{
    public int? FromMonth { get; set; }
    public int? ToMonth { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string FileType { get; set; } = string.Empty;
    public bool isCheckName { get; set; }
}
