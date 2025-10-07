namespace ManageEmployee.DataTransferObject.InvoiceModel;

public class SummarizeInfo
{
    public double sumOfTotalLineAmountWithoutTax { get; set; }
    public double totalAmountWithoutTax { get; set; }
    public double totalTaxAmount { get; set; }
    public double totalAmountWithTax { get; set; }
    public double totalAmountWithTaxFrn { get; set; }
    public string? totalAmountWithTaxInWords { get; set; }
    public bool isTotalAmountPos { get; set; }
    public bool isTotalTaxAmountPos { get; set; }
    public bool isTotalAmtWithoutTaxPos { get; set; }
    public double discountAmount { get; set; }
    public double settlementDiscountAmount { get; set; }
    public bool isDiscountAmtPos { get; set; }
    public double totalAmountAfterDiscount { get; set; }
    public double taxPercentage { get; set; }
}
