namespace ManageEmployee.DataTransferObject.BillModels;

public class BillDetailQrInfo
{
    public string? QrCode { get; set; }
    public int Order { get; set; }
    public string? Position { get; set; }
    public List<PositionBillDetail>? Positions { get; set; }
    public DateTime? DateExpiration { get; set; }
    public double Quantity { get; set; }
    public double RealQuantity { get; set; }
    public bool IsVisible { get; set; }
}
