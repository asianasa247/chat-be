namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class OrderProduceProductReport
{
    public string? GoodName { get; set; }
    public string? CustomerName { get; set; }
    public double Quantity { get; set; }
    public double QuantityDelivered { get; set; }
    public double QuantityInProgress { get; set; }
    public List<OrderProduceProductReportDetail>? Items { get; set; }
}
