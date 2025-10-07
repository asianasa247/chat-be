namespace ManageEmployee.DataTransferObject.Reports;

public class LedgerDetail
{
    public DateTime RecordDate { get; set; }
    public string? VoucherNumber { get; set; }
    public DateTime VoucherDate { get; set; }
    public string? Description { get; set; }
    public string? CorrespondingAccount { get; set; }
    public string? CorrespondingDetail { get; set; }
    public double DebitArisingAmount { get; set; }
    public double CreditArisingAmount { get; set; }
    public double DebitBalance { get; set; }
    public double CreditBalance { get; set; }
    public double TotalBalance { get; set; }
}
