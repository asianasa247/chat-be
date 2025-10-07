namespace ManageEmployee.DataTransferObject.LedgerWarehouseModels;

public class LedgerWarehouseCreate
{
    public int Id { get; set; }
    public int GoodsId { get; set; }
    public int Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double TotalAmount { get; set; }
    public string? Note { get; set; }
}
