namespace ManageEmployee.Entities.GoodsEntities;

public class GoodWarehousesPosition
{
    public int Id { get; set; }
    public int GoodWarehousesId { get; set; }
    public int WareHouseShelvesId { get; set; }
    public int WareHouseFloorId { get; set; }
    public int WareHousePositionId { get; set; }
    public double Quantity { get; set; }
    public string? Warehouse { get; set; }

}
