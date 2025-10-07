namespace ManageEmployee.DataTransferObject.LedgerWarehouseModels;

public class LedgerRequestDinhKhoanThue
{
    public string? OrginalVoucherNumber { get; set; }
    public DateTime? OrginalBookDate { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? InvoiceTaxCode { get; set; }
    public int IsInternal { get; set; }
}
