namespace ManageEmployee.DataTransferObject.InvoiceModel;

public class ItemInfo
{
    public int lineNumber { get; set; }
    public int selection { get; set; }
    public string? itemCode { get; set; }
    public string? itemName { get; set; }
    public string? unitCode { get; set; }
    public string? unitName { get; set; }
    public double unitPrice { get; set; }
    public double quantity { get; set; }
    public double itemTotalAmountWithoutTax { get; set; }
    public double taxPercentage { get; set; }
    public double taxAmount { get; set; }
    public bool isIncreaseItem { get; set; }
    public string? itemNote { get; set; }
    public string? batchNo { get; set; }
    public string? expDate { get; set; }
    public double discount { get; set; }
    public double discount2 { get; set; }
    public double itemDiscount { get; set; }
    public double itemTotalAmountAfterDiscount { get; set; }
    public double itemTotalAmountWithTax { get; set; }
    public string? customTaxAmount { get; set; }
}
