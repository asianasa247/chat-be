namespace ManageEmployee.DataTransferObject.Reports;

public class FinalStandardDetailModel
{
    public string? DebitCode { get; set; }
    public string? DebitCodeWareHouse { get; set; }
    public string? DebitCodeDetail1 { get; set; }
    public string? DebitCodeDetail2 { get; set; }
    public string? CreditCode { get; set; }
    public string? CreditCodeWareHouse { get; set; }
    public string? CreditCodeDetail1 { get; set; }
    public string? CreditCodeDetail2 { get; set; }
    public double? Quantity { get; set; }
    public double? UnitPrice { get; set; }
    public double? Amount { get; set; }
    public double? PercentRatio { get; set; }
    public string? CurrentMonth { get; set; }
    public string? Type { get; set; }// debitToCredit
}
