namespace ManageEmployee.DataTransferObject.PagingRequest;

public class LedgerReportParam
{
    public int? FromMonth { get; set; }
    public int? ToMonth { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string VoucherType { get; set; } = "PT";
    public string LedgerReportMaker { get; set; } = string.Empty;
    public string AccountCode { get; set; } = "111";
    public string AccountCodeDetail1 { get; set; } = string.Empty;
    public string AccountCodeDetail2 { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public bool isCheckName { get; set; }
    public int BookDetailType { get; set; }
    public int FilterType { get; set; } = 1;// 1: thang, 2: ngay
    public bool IsNoiBo { get; set; }
}
