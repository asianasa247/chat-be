namespace ManageEmployee.DataTransferObject.PagingRequest;

public class TransactionListParam
{
    public int Year { get; set; }
    public int FilterType { get; set; } = 1;//1 thang; 2 ngay
    public int? FromMonth { get; set; }
    public int? ToMonth { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string VoucherType { get; set; } = "PT";
    public string VoteMaker { get; set; } = string.Empty;
    public bool isCheckName { get; set; }
    public string FileType { get; set; } = string.Empty;
    public string? InvoiceNumber { get; set; }
    public string? InvoiceTaxCode { get; set; }
    public bool IsNoiBo { get; set; }

}
