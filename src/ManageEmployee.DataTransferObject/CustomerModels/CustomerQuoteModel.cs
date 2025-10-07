namespace ManageEmployee.DataTransferObject.CustomerModels;

public class CustomerQuoteModel
{
    public long Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime CreateDate { get; set; }
    public string Note { get; set; }
    public double? Quantity { get; set; }
    public double? TotalPrice { get; set; }
    public string CustomerName { get; set; }
}
