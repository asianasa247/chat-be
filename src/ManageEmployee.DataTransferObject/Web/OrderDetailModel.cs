namespace ManageEmployee.DataTransferObject.Web;

public class OrderDetailModel
{
    public int GoodId { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public int? AdultQuantity { get; set; }
    public int? ChildrenQuantity { get; set; }
}
