namespace ManageEmployee.DataTransferObject.InvoiceModel;

public class TaxBreakdown
{
    public decimal taxPercentage { get; set; }
    public decimal taxableAmount { get; set; }
    public decimal taxAmount { get; set; }
    public bool taxableAmountPos { get; set; }
    public bool taxAmountPos { get; set; }
    public string? taxExemptionReason { get; set; }
}
