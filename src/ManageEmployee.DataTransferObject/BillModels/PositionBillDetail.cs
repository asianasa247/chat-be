namespace ManageEmployee.DataTransferObject.BillModels;

public class PositionBillDetail
{
    public int Id { get; set; }
    public string? Position { get; set; }
    public double Quantity { get; set; }
    public double QuantityReal { get; set; }
}
