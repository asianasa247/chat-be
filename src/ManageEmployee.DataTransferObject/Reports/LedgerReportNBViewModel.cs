namespace ManageEmployee.DataTransferObject.Reports;

public class LedgerReportNBViewModel
{
    public string? Type { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public string? VoucherNumber { get; set; }
    public string? DebitCode { get; set; }
    public string? DebitCodeParent { get; set; }
    public string? CreditCode { get; set; }
    public string? CreditCodeParent { get; set; }
    public string? OrginalDescription { get; set; }
    public double Amount { get; set; }
    public bool IsDebit { get; set; }
    public double AmountDebit { get; set; }
    public double AmountCredit { get; set; }
    public DateTime? BookDate { get; set; }
    public DateTime? OrginalBookDate { get; set; }
}
