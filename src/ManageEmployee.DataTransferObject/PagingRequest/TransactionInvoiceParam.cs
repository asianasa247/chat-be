namespace ManageEmployee.DataTransferObject.PagingRequest;

public class TransactionInvoiceParam
{
    public int Year { get; set; }
    public int? FromMonth { get; set; }
    public int? ToMonth { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string VoucherType { get; set; } = "PT";
}
