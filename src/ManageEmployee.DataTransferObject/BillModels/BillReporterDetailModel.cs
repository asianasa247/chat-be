namespace ManageEmployee.DataTransferObject.BillModels;

public class BillReporterDetailModel
{
    public string? CustomerName { get; set; }
    public double QuantityRemaining { get; set; }
    public double AmountRemaining { get; set; }
    public double QuantitySold { get; set; }
    public double AmountSold { get; set; }
    public double QuantityRefund { get; set; }
    public double AmountRefund { get; set; }
    public double TotalAmount { get; set; }
}
