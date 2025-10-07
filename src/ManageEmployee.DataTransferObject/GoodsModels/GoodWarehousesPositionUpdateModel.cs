namespace ManageEmployee.DataTransferObject.GoodsModels;

public class GoodWarehousesPositionUpdateModel
{
    public string? Warehouse { get; set; }
    public int GoodWarehousesId { get; set; }
    public int WareHouseShelvesId { get; set; }
    public int WareHouseFloorId { get; set; }
    public int WareHousePositionId { get; set; }
    public double Quantity { get; set; }
}
