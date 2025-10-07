namespace ManageEmployee.DataTransferObject.WarehouseModel;

public class WareHouseShelvesGetAllModel
{
    public int Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public int WareHouseId { get; set; }
}
