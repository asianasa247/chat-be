namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class OrderProduceProductReportDetail
{
    public string? CustomerName { get; set; }
    public string? GoodName { get; set; }
    public string? OrderProduceProductCode { get; set; }
    public double Quantity { get; set; }
    public double QuantityDelivered { get; set; }
    public double QuantityInProgress { get; set; }
}