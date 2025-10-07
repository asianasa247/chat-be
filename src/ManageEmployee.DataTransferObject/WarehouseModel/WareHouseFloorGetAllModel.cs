namespace ManageEmployee.DataTransferObject.WarehouseModel;

public class WareHouseFloorGetAllModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public int WareHouseShelveId { get; set; }
}
