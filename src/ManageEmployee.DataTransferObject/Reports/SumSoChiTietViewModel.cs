namespace ManageEmployee.DataTransferObject.Reports;

public class SumSoChiTietViewModel
{
    public string? DebitDetailCodeFirst { get; set; }
    public string? CreditDetailCodeFirst { get; set; }
    public string? DebitDetailCodeSecond { get; set; }
    public string? CreditDetailCodeSecond { get; set; }
    public string? DebitCode { get; set; }
    public string? CreditCode { get; set; }
    public double Amount { get; set; }
    public double OrginalCurrency { get; set; }
    public double Quantity { get; set; }
    public string? CreditWarehouse { get; set; }
    public string? DebitWarehouse { get; set; }
}
